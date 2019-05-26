using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading.Tasks;
using LuisBot.Table;
//using Microsoft.ApplicationInsights;
namespace LuisBot.Dialogs
{
    [Serializable]
    public class FeedbackDialog : IDialog<IMessageActivity>
    {
        private readonly string _luisAppId;
        private readonly QnAAnswer _qnAAnswer;

        public FeedbackDialog(string luisAppId, QnAAnswer qnAAnswer)
        {
            // keep track of data associated with feedback
            _luisAppId = luisAppId;
            _qnAAnswer = qnAAnswer;
        }

        public async Task StartAsync(IDialogContext context)
        {
            var feedback = ((Activity)context.Activity).CreateReply("Esta resposta foi útil?");

            feedback.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    //new CardAction(){ Title = "👍", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
                    //new CardAction(){ Title = "👎", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }

                    new CardAction(){ Title = "Gostei 👍", Type=ActionTypes.PostBack, Value=$"yes-positive-feedback" },
                    new CardAction(){ Title = "Não gostei 👎", Type=ActionTypes.PostBack, Value=$"no-negative-feedback" }
                }
            };

            await context.PostAsync(feedback);

            context.Wait(this.MessageReceivedAsync);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var userFeedback = await result;

            if (userFeedback.Text.Contains("yes-positive-feedback") || userFeedback.Text.Contains("no-negative-feedback"))
            {
                // create telemetry client to post to Application Insights 
                //TelemetryClient telemetry = new TelemetryClient();

                if (userFeedback.Text.Contains("yes-positive-feedback"))
                {                   

                    //telemetry.TrackEvent("Yes-Vote", properties);
                }
                else if (userFeedback.Text.Contains("no-negative-feedback"))
                {
                    // post feedback to App Insights
                }

                await context.PostAsync("Obrigado pelo retorno!");

                context.Done<IMessageActivity>(null);

                //registra o Log
                await Logger.Instance.LogarLuisQna
                    (
                        _luisAppId,
                        context,
                        result,
                        _qnAAnswer,
                        userFeedback
                    );
            }
            else
            {
                // no feedback, return to QnA dialog
                context.Done<IMessageActivity>(userFeedback);
            }
        }
    }
}