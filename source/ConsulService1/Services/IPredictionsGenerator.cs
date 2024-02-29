using Microsoft.AspNetCore.SignalR;

namespace ConsulService1.Services
{
    public interface IPredictionsGenerator
    {
        public string GeneratePrediction();
        public void AddNewPredictionWord(string newWord);
    }
}
