using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace PregnancyLibrary
{
    [Serializable]
    public class RootDialog : IDialog<RootDialog>
    {
        public RootDialog(Datastore store)
        {
            _store = store;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await Task.Run(() => context.Wait(MessageReceivedAsync)); // There is a message in context and hence will get called immediately.
        }

        /*
        private void StartDoctorAppointmentDialogAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // TODO:
        }

        private void SendTodaysTips(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // TODO:
        }

        private void SendTodaysArticle(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // TODO:
        }

        private void SendWeeklyArticle(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // TODO:
        }
        */
        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument; // Dummy just to enable async
            string userId = message.From.Id;

            #region First Time User
            if (!(await _store.UserProfileExistsAsync(userId)))
            {
                context.Call(UserInfoForm.MakeRootDialog(), PostUserInfoForm);
                //context.Call(new Introduction(), PostIntroduction);
            }
            #endregion First Time User
            // TODO: Detect User's intent from the message..

            #region Daily Vitals

            #endregion Daily Vitals 

            #region Daily Tips
            // Also use Scheduler to send in the tips

            #endregion Daily Tips


            #region FAQs

            #endregion FAQs
        }

        /*
        private async Task PostIntroduction(IDialogContext context, IAwaitable<Introduction> result)
        {
            // TODO: Store in backend that introduction is done.
            var r = await result;
            await _store.UpdateUserInfo(r.fbId, new BotToUserMilestones { UserId = r.fbId, DateMilestoneRecorded = DateTime.Now, Type = BotToUserMilestonesTypes.Introduction });
            context.Wait(MessageReceivedAsync);
        }
        */

        private async Task PostUserInfoForm(IDialogContext context, IAwaitable<UserInfoForm> result)
        {
            UserInfoForm userInfo = await result;
            // TODO: Store in Datastore
            //await context.PostAsync(string.Format("Stored LMP as {0}", userInfo.LastMenustralPeriod));
            context.Wait(MessageReceivedAsync);
        }
        private readonly Datastore _store;
    }

    /*
     * Persistent Menu
     * ===============
     * curl -X POST -H "Content-Type: application/json" -d '{"setting_type" : "call_to_actions","thread_state" : "existing_thread","call_to_actions":[{"type":"postback","title":"TITLE1","payload":"action?action1"}, {"type":"postback","title":"TITLE2","payload":"action?action2"},{"type":"postback","title":"TITLE3","payload":"action?action3"}]}' "https://graph.facebook.com/v2.6/me/thread_settings?access_token={YOUR ACCESS TOKEN}"
     * curl -X DELETE -H "Content-Type: application/json" -d '{"setting_type":"call_to_actions","thread_state":"existing_thread"}' "https://graph.facebook.com/v2.6/me/thread_settings?access_token={YOUR ACCESS TOKEN}"
    */
}
