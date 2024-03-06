using MassTransit.Initializers;
using Microsoft.EntityFrameworkCore;

namespace ConsulService1.Services
{
    public class PredictionService : IPredictionsGenerator
    {
        private readonly AppDbContext _dbContext;
        private List<string>? predictionWordList = null;
        Random random = new Random();

        public PredictionService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNewPredictionWordAsync(string newWord)
        {
            if(predictionWordList == null)
            {
                await InitListPrediction();
            }
            await _dbContext.Predictions.AddAsync(new Models.Prediction() { PredictionWord = newWord });
            predictionWordList!.Add(newWord);
            _dbContext.SaveChanges();
        }

        public string GeneratePrediction()
        {
            if(predictionWordList == null || predictionWordList.Count == 0)
            {
                return "Похоже у нас нет предсказаний";
            }
            var randInt = random.Next(0, predictionWordList.Count);
            var resultWord = predictionWordList[randInt];
            return $"Тебя ждет {resultWord} день.";
        }

        private async Task InitListPrediction()
        {
            var result = await _dbContext.Predictions.Select(x => x.PredictionWord).ToListAsync();
            if(result.Count != 0)
            {
                predictionWordList = result!;
            }
            else
            {
                predictionWordList = new List<string>();
            }
            
        }
    }
}
