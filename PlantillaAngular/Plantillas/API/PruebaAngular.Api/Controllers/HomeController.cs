using PruebaAngular.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;

namespace PruebaAngular.Api.Controllers
{
    /// <summary>
    /// Controller con ejemplos varios.
    /// </summary>
    [AllowAnonymous]
    public class HomeController : Controller
    {
        IConfiguration configuration;

        ILogger<HomeController> _logger;
        public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
        {

            this.configuration = configuration;

            _logger = logger;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {

            string envName = Environment.GetEnvironmentVariable("ENVIRONMENT");
            if (envName == "Prod" || envName == "Prod")
            {
                return Ok();
            }
            return new RedirectResult("~/swagger");


        }
    }
}
