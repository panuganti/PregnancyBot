using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PregnancyLibrary.DataContracts;
using System;
using System.Threading.Tasks;

namespace PregnancyLibrary
{
    [Serializable]
    public class DailyDialog : IDialog<DailyDialog>
    {
        private Datastore _store;
        private string _userId;

        #region Start
        public DailyDialog(Datastore store, string userId)
        {
            _store = store;
            _userId = userId;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await Task.Run(() => context.Wait(MessageReceivedAsync)); // There is a message in context and hence will get called immediately.
        }
        #endregion Start

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            IMessageActivity message = await argument; 
            string userId = message.From.Id;
            // TODO: Detect User's intent from the message..
            context.Wait(MessageReceivedAsync);
        }

        private async Task SendTodaysArticle(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            await context.PostAsync("Your baby is {0} weeks {1} days today");
        }


        #region TODO
        /*
        private void StartDoctorAppointmentDialogAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // TODO:
        }

        private void SendTodaysTips(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // TODO:
        }


        private void SendWeeklyArticle(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // TODO:
        }
        */
        #endregion TODO
    }
}
