using ConsulService2.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace ConsulService2.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpRequest _httpRequset;
        public HomeController(IHttpRequest httpRequset)
        {
            _httpRequset = httpRequset;
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
    }
}
