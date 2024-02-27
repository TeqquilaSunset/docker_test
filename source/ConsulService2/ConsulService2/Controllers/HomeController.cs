using ConsulService2.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using MassTransit.RabbitMqTransport;
using ConsulService2.Models;
using MassTransit;
using Core.Models;
namespace ConsulService2.Controllers
{
    [ApiController]
    public class HomeController : Controller
    {
        private readonly IHttpRequest _httpRequset;
        private readonly IPublishEndpoint _publishEndpoint;
        public HomeController(IHttpRequest httpRequset, IPublishEndpoint publishEndpoint)
        {
            _httpRequset = httpRequset;
            _publishEndpoint = publishEndpoint;
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
    }
}
