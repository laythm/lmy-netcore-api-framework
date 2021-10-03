using System;
using System.Threading.Tasks;
using Common.Enums;
using Common.Models.Common;
using Common.Models.Projects;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = RolesEnum.Admin)]
    public class ProjectsController : BaseController
    {
        IProjectsService _projectsService;
        IHttpContextAccessor _httpContextAccessor;
        ILogger<UsersController> _logger;

        public ProjectsController(IProjectsService projectsService,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UsersController> logger) :
           base()
        {
            _projectsService = projectsService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpPost("Search")]
        public async Task<ListModel<ProjectModel>> Search(PagingSortingModel<SearchModel> searchModel)
        {
            ListModel<ProjectModel> model = new ListModel<ProjectModel>();

            try
            {
                model = await _projectsService.Search(searchModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }

        [HttpGet("Get")]
        public async Task<ProjectModel> Get(string id)
        {
            ProjectModel model = new ProjectModel();

            try
            {
                model = await _projectsService.Get(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }

        [HttpPost("Add")]
        public async Task<BaseModel> Add(ProjectModel userModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                model = await _projectsService.Add(userModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                model.AddError(ApiMessages.Error);
            }

            return model;
        }

        [HttpPost("Edit")]
        public async Task<BaseModel> Edit(ProjectModel userModel)
        {
            BaseModel model = new BaseModel();

            try
            {
                model = await _projectsService.Edit(userModel);
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
                model = await _projectsService.Delete(id);
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
