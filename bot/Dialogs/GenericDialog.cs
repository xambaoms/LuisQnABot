using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Linq;
using LuisBot.Table;
using System.Collections.Generic;
using Microsoft.Bot.Connector;
using System.Configuration;

namespace LuisBot.Dialogs
{
    //Dialog generico que utiliza 
    //as informacoes dos INTENTS do LUIS de forma dinamica
    [Serializable]
    public class GenericDialog : LuisDialog<object>
    {
        //Gerenciador com as informacoes do LUIS vinculadas a um QnA
        //private readonly LuisManager luisManager;
        
        public double Threshlod
        {
            get
            {
                var strThreshold = ConfigurationManager.AppSettings["QnaThreshold"];

                double i = 0;
                if (double.TryParse(strThreshold, out i))
                {
                    if (i > 0 && i < 100)
                    {
                        return i;
                    }
                }

                return 60D; 
            }
        }

        private readonly string LuisAppId;

        private readonly Dictionary<string, QnAMakerService> QnaServices;
        //ctor
        public GenericDialog(
            string luisAppId,
            string luisApiKey,
            string luisHostRegion,
            Dictionary<string, QnAMakerService> qnaServices
            )
            : base(new LuisService(new LuisModelAttribute(
            luisAppId,
            luisApiKey,
            domain: luisHostRegion)))
        {
            LuisAppId = luisAppId;
            QnaServices = qnaServices;

        }

        //Metodo generico
        [LuisIntent("")]
        public async Task GenericIntent(IDialogContext context, LuisResult result)
        {
            //resposta do Qna
            QnAAnswer qnAAnswer = null;

            //variavel de controle do intent 
            string intent = String.Empty;

            //verifica se o LUIS rtornou algumointent
            if (result.Intents.Count > 0)
                intent = result.Intents.First().Intent;

            //verifica se existe esse content nas onfiguracoes do servico
            if (QnaServices.ContainsKey(intent))
            {
                //rcupera o servico do QnA
                var qnaService = QnaServices[intent];

                //recupera a respost do servico de QnA
                qnAAnswer = await qnaService.GetAnswer(result.Query);

                //seta a resposta padrao 
                string qnaMakerAnswer = "Nenhum resultado foi encontrado.";

                //verifica se existe resposta do Qna
                if (qnAAnswer.answers.Count > 0)
                {
                    qnaMakerAnswer = qnAAnswer.answers[0].answer;

                    //Retorna a resposta
                    await context.PostAsync($"{qnaMakerAnswer}");

                    //Exibe o feedback se o score for inferior ao threshold
                    if (qnAAnswer.answers[0].score < Threshlod)
                        context.Call(new FeedbackDialog(LuisAppId, qnAAnswer), ResumeAfterFeedback);
                }
                else
                {
                    //Retorna a resposta
                    await context.PostAsync($"{qnaMakerAnswer}");

                    //Espera ate que a mensagem chegue por completo
                    context.Wait(MessageReceived);
                }
            }
            else //se nao existe intent
            {
                //seta a rsposta do Luis
                await this.ShowLuisResult(context, result);
            }

            //registra o Log
            await Logger.Instance.LogarLuisQna
                (
                    LuisAppId,
                    context,
                    result,
                    qnAAnswer
                );
        }


        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            context.Wait(MessageReceived);
        }

        private async Task ResumeAfterFeedback(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (await result != null)
            {
                await MessageReceived(context, result);
            }
            else
            {
                context.Done<IMessageActivity>(null);
            }
        }
    }
}