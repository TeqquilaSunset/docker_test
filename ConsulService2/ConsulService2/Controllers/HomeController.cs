using ConsulService2.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;

namespace ConsulService2.Controllers
{
    public class HomeController : Controller
    {
        //private readonly IConnection _rabbitConnection;
        //private readonly IModel _channel;

        //public HomeController()
        //{
        //    var factory = new ConnectionFactory()
        //    {
        //        HostName = "localhost",
        //        Port = 5672,
        //        UserName = "rmuser",
        //        Password = "rmpassword"
        //    };
        //    _rabbitConnection = factory.CreateConnection();
        //    _channel = _rabbitConnection.CreateModel();

        //    _channel.QueueDeclare(queue: "predictionsQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
        //}

        [HttpGet("/healthCheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpGet("/index")]
        public async Task<IActionResult> Index()
        {
            //try
            //{
            //    var consumer = new EventingBasicConsumer(_channel);
            //    consumer.Received += (model, ea) =>
            //    {
            //        var body = ea.Body.ToArray();
            //        var prediction = Encoding.UTF8.GetString(body);

            //        ViewData["Prediction"] = prediction;
            //    };
            //    var ea = _channel.BasicGet(queue: "predictionsQueue", autoAck: true);

            //    if (ea != null)
            //    {
            //        var body = ea.Body.ToArray();
            //        var prediction = Encoding.UTF8.GetString(body);

            //        ViewData["Prediction"] = prediction;
            //    }
            //    else
            //    {
            //        ViewData["Prediction"] = "Очередь пуста";
            //    }

            //    return View();
            //}
            //catch (Exception ex)
            //{
            //    ViewData["Prediction"] = "Не удалось получить предсказание";
            //    return View();
            //}

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
