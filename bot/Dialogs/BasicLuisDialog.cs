using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;
using LuisBot.Dialogs;

namespace Microsoft.Bot.Sample.LuisBot
{
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        // LUIS Settings
        static string LUIS_appId = "fda23a32-eda2-47cf-abd6-27eedf840dc7";
        static string LUIS_apiKey = "b9a97aee1b4d424dacbc503d1a593e4e";
        static string LUIS_hostRegion = "westus.api.cognitive.microsoft.com";
    
        // QnA Maker global settings
        // assumes all KBs are created with same Azure service
        static string qnamaker_endpointKey = "cad67ee3-6cda-4daf-82ba-4dd1cdf003ca";
        static string qnamaker_endpointDomain = "qna-megasena";
    
        // QnA Maker Human Resources Knowledge base
        static string HR_kbID = "170e745f-e858-4488-ba07-30c88ddb7936";
    
        // QnA Maker Finance Knowledge base
        static string Finance_kbID = "d42789b1-1da4-466b-96c3-74f5db3f12c3";
    
        // Instantiate the knowledge bases
        public QnAMakerService hrQnAService = new QnAMakerService("https://" + qnamaker_endpointDomain + ".azurewebsites.net", HR_kbID, qnamaker_endpointKey);
        public QnAMakerService financeQnAService = new QnAMakerService("https://" + qnamaker_endpointDomain + ".azurewebsites.net", Finance_kbID, qnamaker_endpointKey);
    
        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(
            LUIS_appId,
            LUIS_apiKey,
            domain: LUIS_hostRegion)))
        {
        }      

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            HttpClient client = new HttpClient();
            await this.ShowLuisResult(context, result);
        }
    
        // HR Intent
        [LuisIntent("QnA-Megasena")]
        public async Task HumanResourcesIntent(IDialogContext context, LuisResult result)
        {
            // Ask the HR knowledge base
            var qnaMakerAnswer = await hrQnAService.GetAnswer(result.Query);
            await context.PostAsync($"{qnaMakerAnswer}");
            context.Wait(MessageReceived);
        }
        
        // Finance intent
        [LuisIntent("QnA-FGTS")]
        public async Task FinanceIntent(IDialogContext context, LuisResult result)
        {
            // Ask the finance knowledge base
            var qnaMakerAnswer = await financeQnAService.GetAnswer(result.Query);
            await context.PostAsync($"{qnaMakerAnswer}");
            context.Wait(MessageReceived);
        }
        
        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }
    }
}