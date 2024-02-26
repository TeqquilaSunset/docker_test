namespace ConsulService1.Services
{
    public class PredictionService : IPredictionsGenerator
    {
        private List<string> stageOneList = new List<string>()
        {
            "хороший", "замечательный", "великолепный",
            "потрясный", "ужасный", "отвратиетельный", "наихудший",
            "обычный", "нормальный",
        };
        Random random = new Random();

        public void AddPrediction(string newWord)
        {
            stageOneList.Add(newWord);
        }

        public string CreatePrediction()
        {
            var randInt = random.Next(0, stageOneList.Count);
            var resultWord = stageOneList[randInt];
            return $"Тебя ждет {resultWord} день.";
        }
    }
}
