using ConsulService2.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace ConsulService2.Controllers
{
    public class HomeController : Controller
    {

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
                HttpRequset httpRequset = new HttpRequset();
                var result = await httpRequset.GetStirngPrediction();
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
