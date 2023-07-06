using Common;
using Common.Enums;
using Common.Extensions;
using Common.Helpers;
using Common.Interfaces;
using Common.Models.Common;
using Common.Models.Users;
using Infrastructure;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{

    public class UsersService : IUsersService
    {
        private readonly IRequestContext _requestContext;
        private readonly IGenericUnitOfwork<LmyFrameworkDBContext> _unitOfWork;
        private readonly IGenericRepository<Users> _repoUsers;
        private readonly IGenericRepository<UserRoles> _repoUserRoles;
        private readonly IGenericRepository<Roles> _repoRoles;
        IConfiguration _configuration;

        public UsersService(
            IRequestContext requestContext,
            IGenericUnitOfwork<LmyFrameworkDBContext> unitOfWork,
            IGenericRepository<Users> repoUsers,
            IGenericRepository<UserRoles> repoUserRoles,
            IGenericRepository<Roles> repoRoles,
            IConfiguration configuration)
        {
            _requestContext = requestContext;
            _unitOfWork = unitOfWork;
            _repoUsers = repoUsers;
            _repoUserRoles = repoUserRoles;
            _repoRoles = repoRoles;
            _configuration = configuration;
        }

        private string getToken(Users user)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Name, user.ID));

            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.RoleID));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AppSettings:TokenSecret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["AppSettings:TokenIssuer"],
                audience: _configuration["AppSettings:TokenIssuer"],
                claims: claims,
                expires: DateTime.Now.AddMonths(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UserModel> SignInUser(SignInModel signInModel)
        {
            UserModel model = new UserModel();

            try
            {
                var user = await _repoUsers
                .Query(x => !x.IsDeleted && x.UserName.ToLower() == signInModel.UserName.ToLower())
                .Include(x => x.UserRoles.Where(x => !x.IsDeleted))
                .FirstOrDefaultAsync();
                if (user != null && Hasher.ValidateHash(user.Password, signInModel.Password))
                {
                    model.ID = user.ID;
                    model.UserName = user.UserName;
                    model.Roles = user.UserRoles.Select(x => x.RoleID).ToArray();
                    model.Token = getToken(user);
                    model.AddSuccess(ApiMessages.Success);
                }
                else
                {
                    model.AddError(ApiMessages.FormatApiMessage("invalidUsernNameOrPassword"));
                }
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();

                throw;
            }

            return model;
        }

        public async Task<LookupsModel> GetLookups()
        {
            LookupsModel model = new LookupsModel();

            try
            {
                Mapper.FillList(_repoRoles.Query().ToList(), model.Roles);
            }
            catch (Exception ex)
            {
                throw;
            }

            return model;
        }

        public async Task<ListModel<UserModel>> Search(PagingSortingModel<SearchModel> searchModel)
        {
            ListModel<UserModel> model = new ListModel<UserModel>();

            try
            {
                if (searchModel.Data == null)
                {
                    searchModel.Data = new SearchModel();
                }

                var query = _repoUsers
                        .Query(x => !x.IsDeleted)
                        .Include(x => x.UserRoles.Where(x => !x.IsDeleted))
                        .Where(x =>
                              (string.IsNullOrEmpty(searchModel.Data.FirstName) || x.FirstName.ToLower().Contains(searchModel.Data.FirstName.ToLower())) &&
                              (string.IsNullOrEmpty(searchModel.Data.LastName) || x.LastName.ToLower().Contains(searchModel.Data.LastName.ToLower())) &&
                              (string.IsNullOrEmpty(searchModel.Data.Email) || x.Email.ToLower().Contains(searchModel.Data.Email.ToLower())) &&
                              (string.IsNullOrEmpty(searchModel.Data.UserName) || x.UserName.ToLower().Contains(searchModel.Data.UserName.ToLower())) &&
                              (searchModel.Data.Roles == null || x.UserRoles.Any(r => searchModel.Data.Roles.Any(searchRoole => searchRoole == r.RoleID)))
                              );


                query = Common.CommonUtilities.GetOrderBy(query, SortDirection.desc, x => x.ModifiedDate);

                if (searchModel.SortInfo != null)
                {
                    switch (searchModel.SortInfo.column)
                    {
                        case "firstName":
                            query = Common.CommonUtilities.GetOrderBy(query, searchModel.SortInfo.dir, x => x.FirstName);
                            break;
                        case "lastName":
                            query = Common.CommonUtilities.GetOrderBy(query, searchModel.SortInfo.dir, x => x.LastName);
                            break;
                        case "email":
                            query = Common.CommonUtilities.GetOrderBy(query, searchModel.SortInfo.dir, x => x.Email);
                            break;
                        case "userName":
                            query = Common.CommonUtilities.GetOrderBy(query, searchModel.SortInfo.dir, x => x.UserName);
                            break;
                    }
                }

                model.TotalRecordsCount = await query.CountAsync();

                var lst = await query.ToListAsync();

                foreach (var user in lst)
                {
                    UserModel userModel = new UserModel();

                    Mapper.Fill(user, userModel);

                    userModel.Roles = user.UserRoles.Select(x => x.RoleID).ToArray();
                    userModel.Password = null;
                    model.List.Add(userModel);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return model;
        }

        public async Task<UserModel> Get(string id)
        {
            UserModel model = new UserModel();

            try
            {
                Users user = await _repoUsers.Query(x => x.ID == id)
                    .Include(x => x.UserRoles.Where(x => !x.IsDeleted))
                    .FirstOrDefaultAsync();

                Mapper.Fill(user, model);

                model.Roles = user.UserRoles.Select(x => x.RoleID).ToArray();
                model.Password = null;
            }
            catch (Exception ex)
            {
                throw;
            }

            return model;
        }

        private bool IsValid(UserModel userModel)
        {
            userModel.Errors.Clear();
            var lst = _repoUsers.Query(x =>
                    x.IsDeleted == false &&
                    x.ID != userModel.ID &&
                    (
                        x.Email.ToLower() == userModel.Email.ToLower() ||
                        x.UserName.ToLower() == userModel.UserName.ToLower()
                    )
                )
                .Select(x => new { x.Email, x.UserName })
                .ToList();

            if (lst.Any(x => x.Email.ToLower() == userModel.Email.ToLower()))
            {
                userModel.AddError(ApiMessages.FormatApiMessage("user.duplicateEmail"));
            }

            if (lst.Any(x => x.UserName.ToLower() == userModel.UserName.ToLower()))
            {
                userModel.AddError(ApiMessages.FormatApiMessage("user.duplicateUserName"));
            }

            if (userModel.HasError)
            {
                return false;
            }

            return true;
        }

        public async Task<BaseModel> Add(UserModel userModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                if (!IsValid(userModel))
                {
                    return userModel;
                }

                _unitOfWork.BeginTransaction();
                Users user = new Users();

                Mapper.Fill(userModel, user, x => x.CreatedBy, x => x.CreationDate, x => x.ModifiedBy, x => x.ModifiedDate);

                user.ID = Guid.NewGuid().ToString();

                user.Password = Hasher.HashString(userModel.Password);

                await _repoUsers.InsertAsync(user);

                foreach (var role in userModel.Roles)
                {
                    UserRoles userRole = new UserRoles();
                    userRole.ID = Guid.NewGuid().ToString();
                    userRole.UserID = user.ID;
                    userRole.RoleID = role;

                    await _repoUserRoles.InsertAsync(userRole);
                }

                await _unitOfWork.SaveChangesAsync(_requestContext.CurrentUserID);
                _unitOfWork.Commit();

                model.AddSuccess(ApiMessages.Success);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();

                throw;
            }

            return model;
        }

        public async Task<BaseModel> Edit(UserModel userModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                if (!IsValid(userModel))
                {
                    return userModel;
                }

                _unitOfWork.BeginTransaction();
                Users user = await _repoUsers.Query(x => x.ID == userModel.ID)
                    .Include(x => x.UserRoles.Where(x => !x.IsDeleted))
                    .FirstOrDefaultAsync();

                Mapper.Fill(userModel, user,
                    x => x.CreatedBy,
                    x => x.CreationDate,
                    x => x.ModifiedBy,
                    x => x.ModifiedDate, x => x.Password);

                if (!string.IsNullOrEmpty(userModel.Password))
                {
                    user.Password = Hasher.HashString(userModel.Password);
                }

                await _repoUsers.UpdateAsync(user);

                await user.UserRoles.ToList().ForEachAsync(async x =>
                 {
                     x.IsDeleted = true;
                     await _repoUserRoles.UpdateAsync(x);
                 });

                foreach (var role in userModel.Roles)
                {
                    UserRoles userRole = new UserRoles();
                    userRole.ID = Guid.NewGuid().ToString();
                    userRole.UserID = user.ID;
                    userRole.RoleID = role;

                    await _repoUserRoles.InsertAsync(userRole);
                }

                await _unitOfWork.SaveChangesAsync(_requestContext.CurrentUserID);
                _unitOfWork.Commit();

                model.AddSuccess(ApiMessages.Success);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();

                throw;
            }

            return model;
        }

        public async Task<BaseModel> Delete(string id)
        {
            BaseModel model = new BaseModel();

            try
            {
                _unitOfWork.BeginTransaction();

                Users user = await _repoUsers.Query(x => x.ID == id)
                    .Include(x => x.UserRoles.Where(x => !x.IsDeleted))
                    .FirstOrDefaultAsync();

                user.IsDeleted = true;

                await user.UserRoles.ToList().ForEachAsync(async x =>
                {
                    x.IsDeleted = true;
                    await _repoUserRoles.UpdateAsync(x);
                });

                await _repoUsers.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync(_requestContext.CurrentUserID);

                _unitOfWork.Commit();

                model.AddSuccess(ApiMessages.Success);
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();

                throw;
            }

            return model;
        }

        public async Task<BaseModel> ValidateToken(string token)
        {
            BaseModel model = new BaseModel();

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:TokenSecret"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidIssuer= _configuration["AppSettings:TokenIssuer"],
                    ValidAudience= _configuration["AppSettings:TokenIssuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId =  jwtToken.Claims.First(x => x.Type == ClaimTypes.Name).Value;
                var roles = string.Join(",", jwtToken.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList());

                model.AddSuccess(ApiMessages.Success+$" userID ={userId}, roles={roles}");
            }
            catch (Exception ex)
            {
                _unitOfWork.RollBack();

                throw;
            }

            return model;
        }
    }
}
