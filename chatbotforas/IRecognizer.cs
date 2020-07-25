using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;


namespace chatbotforas
{
    public interface IRecognizer
    {
        LuisRecognizer recognizer { get;}

    }

}
