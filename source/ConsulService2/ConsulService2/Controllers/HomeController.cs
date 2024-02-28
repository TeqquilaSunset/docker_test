using ConsulService2.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using MassTransit.RabbitMqTransport;
using ConsulService2.Models;
using MassTransit;
using Core.Models;
using ConsulService2.Helpers;
namespace ConsulService2.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IHttpRequest _httpRequset;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IConfiguration _configuration;
        public HomeController(IHttpRequest httpRequset, IPublishEndpoint publishEndpoint, IConfiguration configuration)
        {
            _httpRequset = httpRequset;
            _publishEndpoint = publishEndpoint;
            _configuration = configuration;
        }

        [HttpGet("/healthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet("/index")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _httpRequset.GetStringPredictionAsync();
                ViewData["Prediction"] = result;
                return View();
            }
            catch (Exception ex)
            {
                ViewData["Prediction"] = "Не удалось получить предсказание";
                return View();
            }
        }
        
        [HttpPut("/addword")]
        public async Task<IActionResult> AddWord(NewPrediction obj)
        { 
            await _publishEndpoint.Publish<NewPrediction>(obj);
            return Ok();
        }

        [HttpGet("/consul")]
        public async Task<IActionResult> TextFromConsul()
        {
            //var consulDemoKey = await ConsulKeyValueProvider.GetValueAsync<ConsulDemoKey>(key: "servise1");

            //if (consulDemoKey != null && consulDemoKey.IsEnabled)
            //{
            //    return Ok(consulDemoKey);
            //}
            var result = _configuration["Message"];
            return Ok(result);
        }
    }
}
