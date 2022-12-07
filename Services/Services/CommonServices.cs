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

    public class CommonServices : ICommonService
    {
        private readonly IRequestContext _requestContext;
        private readonly IGenericUnitOfwork<LmyFrameworkDBContext> _unitOfWork;
        private readonly IGenericRepository<ClientErrors> _repoClientErrors;

        IConfiguration _configuration;

        public CommonServices(
            IRequestContext requestContext,
            IGenericUnitOfwork<LmyFrameworkDBContext> unitOfWork,
            IGenericRepository<ClientErrors> repoClientErrors,
            IConfiguration configuration)
        {
            _requestContext = requestContext;
            _unitOfWork = unitOfWork;
            _repoClientErrors = repoClientErrors;
            _configuration = configuration;
        }
 
        public async Task LogClientError(ClientErrorModel clientErrorModel)
        {
            ClientErrors error = new ClientErrors();

            Mapper.Fill(clientErrorModel, error);
            error.ID = Guid.NewGuid().ToString();
            error.CreationDate = DateTime.UtcNow;
            await _repoClientErrors.InsertAsync(error);
             
            await _unitOfWork.SaveChangesAsync(_requestContext.CurrentUserID);
        }
    }
}
