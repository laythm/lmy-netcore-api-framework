using Common.Enums;
using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Code
{
    public class RequestContext : IRequestContext
    {
        IHttpContextAccessor _httpContextAccessor;
        public RequestContext(IServiceProvider svp)
        {            
            _httpContextAccessor = (IHttpContextAccessor)svp.GetService(typeof(IHttpContextAccessor)); 
        }

        public string CurrentUserID
        {
            get
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    return _httpContextAccessor.HttpContext.User.Identity.Name;
                }

                return null;
            }
        }

        public string[] CurrentUserRoles
        {
            get
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    return _httpContextAccessor.HttpContext.User.Claims.Select(x => x.Value).ToArray();
                }

                return null;
            }
        }
    }
}
