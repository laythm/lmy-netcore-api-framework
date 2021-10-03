using Common;
using Common.Enums;
using Common.Extensions;
using Common.Helpers;
using Common.Interfaces;
using Common.Models.Common;
using Common.Models.Projects;
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

    public class ProjectsService : IProjectsService
    {
        DbContext _dbContext;
        private readonly IGenericRepository<Projects> _repoProjects;

        IConfiguration _configuration;

        public ProjectsService(DbContext dbContext,
            IGenericRepository<Projects> repoProjects,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _repoProjects = repoProjects;
            _configuration = configuration;
        }

        public async Task<ListModel<ProjectModel>> Search(PagingSortingModel<SearchModel> searchModel)
        {
            ListModel<ProjectModel> model = new ListModel<ProjectModel>();

            try
            {
                if (searchModel.Data == null)
                {
                    searchModel.Data = new SearchModel();
                }

                var query = _repoProjects
                        .Query(x => !x.IsDeleted)
                        .Where(x =>
                              (string.IsNullOrEmpty(searchModel.Data.Title) || x.Title.ToLower().Contains(searchModel.Data.Title.ToLower())) &&
                              (string.IsNullOrEmpty(searchModel.Data.Description) || x.Description.ToLower().Contains(searchModel.Data.Description.ToLower()))
                          );


                query = Common.CommonUtilities.GetOrderBy(query, SortDirection.desc, x => x.ModifiedDate);

                if (searchModel.SortInfo != null)
                {
                    switch (searchModel.SortInfo.column)
                    {
                        case "title":
                            query = Common.CommonUtilities.GetOrderBy(query, searchModel.SortInfo.dir, x => x.Title);
                            break;
                        case "description":
                            query = Common.CommonUtilities.GetOrderBy(query, searchModel.SortInfo.dir, x => x.Description);
                            break;
                    }
                }

                model.TotalRecordsCount = await query.CountAsync();

                query = query.Skip((searchModel.Page - 1) * searchModel.PageSize).Take(searchModel.PageSize);

                Mapper.FillList(await query.ToListAsync(), model.List);
            }
            catch (Exception ex)
            {
                throw;
            }

            return model;
        }

        public async Task<ProjectModel> Get(string id)
        {
            ProjectModel model = new ProjectModel();

            try
            {
                Projects project = await _repoProjects.Query(x => x.ID == id).FirstOrDefaultAsync();

                Mapper.Fill(project, model);
            }
            catch (Exception ex)
            {
                throw;
            }

            return model;
        }

        private bool IsValid(ProjectModel projectModel)
        {
            bool exist = _repoProjects
                .Query(x =>
                    x.IsDeleted == false &&
                    x.ID != projectModel.ID &&
                    x.Title.ToLower() == projectModel.Title.ToLower())
                .Any();
            if (exist)
            {
                projectModel.AddError(ApiMessages.FormatApiMessage("project.duplicate"));
            }

            if (projectModel.HasError)
            {
                return false;
            }

            return true;
        }

        public async Task<BaseModel> Add(ProjectModel projectModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                if (!IsValid(projectModel))
                {
                    return projectModel;
                }

                _dbContext.Database.BeginTransaction();
                Projects project = new Projects();

                Mapper.Fill(projectModel, project, x => x.CreatedBy, x => x.CreationDate, x => x.ModifiedBy, x => x.ModifiedDate);

                project.ID = Guid.NewGuid().ToString();

                await _repoProjects.InsertAsync(project);

                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();

                model.AddSuccess(ApiMessages.Success);
            }
            catch (Exception ex)
            {
                _dbContext.Database.RollbackTransaction();

                throw;
            }

            return model;
        }

        public async Task<BaseModel> Edit(ProjectModel projectModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                if (!IsValid(projectModel))
                {
                    return projectModel;
                }

                _dbContext.Database.BeginTransaction();
                Projects project = await _repoProjects.Query(x => x.ID == projectModel.ID).FirstOrDefaultAsync();

                Mapper.Fill(projectModel, project, x => x.CreatedBy, x => x.CreationDate, x => x.ModifiedBy, x => x.ModifiedDate);

                await _repoProjects.UpdateAsync(project);

                await _dbContext.SaveChangesAsync();
                await _dbContext.Database.CommitTransactionAsync();

                model.AddSuccess(ApiMessages.Success);
            }
            catch (Exception ex)
            {
                _dbContext.Database.RollbackTransaction();

                throw;
            }

            return model;
        }

        public async Task<BaseModel> Delete(string id)
        {
            BaseModel model = new BaseModel();

            try
            {
                _dbContext.Database.BeginTransaction();

                Projects project = await _repoProjects.Query(x => x.ID == id).FirstOrDefaultAsync();
                project.IsDeleted = true;

                await _repoProjects.UpdateAsync(project);
                await _dbContext.SaveChangesAsync();

                await _dbContext.Database.CommitTransactionAsync();

                model.AddSuccess(ApiMessages.Success);
            }
            catch (Exception ex)
            {
                _dbContext.Database.RollbackTransaction();

                throw;
            }

            return model;
        }
    }
}
