using Microsoft.AspNetCore.SignalR;

namespace ConsulService1.Services
{
    public interface IPredictionsGenerator
    {
        public string CreatePrediction();
        public void AddPrediction(string newWord);
    }
}
