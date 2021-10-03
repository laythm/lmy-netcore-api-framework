using Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Api.Services.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        ILogger _logger;
        IRequestContext _requestContext;
        public HomeController(ILogger<HomeController> logger, IRequestContext requestContext)
        {
            _logger = logger;
            _requestContext = requestContext;
        }

        [HttpGet]
        public string Index()
        {

            return "app Is Running... user id =" + _requestContext.CurrentUserID;
        }
    }
}