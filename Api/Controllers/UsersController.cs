using System;
using System.Threading.Tasks;
using Common.Enums;
using Common.Models.Common;
using Common.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Services.Interfaces;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme, Roles = RolesEnum.Admin)]
    public class UsersController : BaseController
    {
        IUsersService _userService;
        IHttpContextAccessor _httpContextAccessor;
        ILogger<UsersController> _logger; 

        public UsersController(IUsersService userService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UsersController> logger) :
           base()
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("SignInUser")]
        [AllowAnonymous]
        public async Task<UserModel> SignInUser(SignInModel signInModel)
        {
            UserModel model = new UserModel();

            try
            {
                model = await _userService.SignInUser(signInModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }

        [HttpPost("Search")]
        public async Task<ListModel<UserModel>> Search(PagingSortingModel<SearchModel> searchModel)
        {
            ListModel<UserModel> model = new ListModel<UserModel>();

            try
            {
                model = await _userService.Search(searchModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }


        [HttpGet("GetLookups")]
        public async Task<LookupsModel> GetLookups()
        {
            LookupsModel model = new LookupsModel();

            try
            {
                model = await _userService.GetLookups();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }

        [HttpGet("Get")]
        public async Task<UserModel> Get(string id)
        {
            UserModel model = new UserModel();

            try
            {
                model = await _userService.Get(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }

        [HttpPost("Add")]
        public async Task<BaseModel> Add(UserModel userModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                model = await _userService.Add(userModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }

        [HttpPost("Edit")]
        public async Task<BaseModel> Edit(UserModel userModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                model = await _userService.Edit(userModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }


        [HttpGet("Delete")]
        public async Task<BaseModel> Delete(string id)
        {
            BaseModel model = new BaseModel();

            try
            {
                model = await _userService.Delete(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }
    }
}
