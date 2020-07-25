using Antlr4.Runtime;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Configuration;



namespace chatbotforas
{
    public class BotServices : IRecognizer
    {
        public BotServices(IConfiguration configuration)
        {
            // Read the setting for cognitive services (LUIS, QnA) from the appsettings.json
            // If includeApiResults is set to true, the full response from the LUIS api (LuisResult)
            // will be made available in the properties collection of the RecognizerResult

            var luisApplication = new LuisApplication(
                configuration["LuisAppId"],
                configuration["LuisAPIKey"],
               $"https://{configuration["LuisAPIHostName"]}");

            var recognizerOptions = new LuisRecognizerOptionsV2(luisApplication)
            {
                IncludeAPIResults = true,
                PredictionOptions = new LuisPredictionOptions()
                {
                    IncludeAllIntents = true,
                    IncludeInstanceData = true
                }
            };

            recognizer = new LuisRecognizer(recognizerOptions);

        }
        public LuisRecognizer recognizer { get; private set; }
    }
}

