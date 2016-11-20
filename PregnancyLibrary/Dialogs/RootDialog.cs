using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using PregnancyLibrary.DataContracts;
using System;
using System.Threading.Tasks;

namespace PregnancyLibrary
{
    [Serializable]
    public class RootDialog : IDialog<RootDialog>
    {
        private Datastore _store;
        private string _userId;

        #region Start
        public RootDialog(Datastore store, string userId)
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
            IMessageActivity message = await argument; // Dummy just to enable async
            string userId = message.From.Id;
            
            #region First Time User
            if (!(await _store.UserProfileExistsAsync(userId)))
            {
                await SendIntroductionMessages(context, message);
                context.Call(UserInfoForm.MakeRootDialog(), PostUserInfoForm);
                return;
            }
            #endregion First Time User
            await context.PostAsync(string.Format("User profile exists")); // TODO: Delete

            // Today's vitals
            PromptDialog.Confirm(context, AfterResetAsync, "Would you like to enter your vitals now?",
                                                        "Didn't get that!", promptStyle: PromptStyle.None);
            // TODO: Detect User's intent from the message..
            context.Wait(MessageReceivedAsync);
        }

        // TODO:
        public async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> argument)
        {
            var confirm = await argument;
            if (confirm)
            {
                await context.PostAsync("Moving to Vitals Dialog.");
                context.Call<DailyDialog>(new DailyDialog(_store, _userId),AfterVitalsAsync);
                return;
            }
            else
            {
                await context.PostAsync("Staying in Root Dialog");
            }
            context.Wait(MessageReceivedAsync);
        }

        public async Task AfterVitalsAsync(IDialogContext context, IAwaitable<DailyDialog> result)
        {
            await context.PostAsync("Vitals Completed. Back to Root Dialog");
            context.Wait(MessageReceivedAsync);
        }

        private async Task SendIntroductionMessages(IDialogContext context, IMessageActivity message)
        {
            await context.PostAsync(string.Format("Hi {0} :), My name is Emma. I'll be your friend during your pregnancy.", message.From.Name));
            await context.PostAsync(string.Format("I'll help you track all the vital information and will try to answer any questions you might have."));
        }

        private async Task PostUserInfoForm(IDialogContext context, IAwaitable<UserInfoForm> result)
        {
            UserInfoForm userInfo = await result;
            
            var user = await _store.GetUserProfileAsync(_userId);
            if (user == null)
            {
                user = new User();
                user.Id = _userId;
                user.StartTime = DateTime.Now;
            }
            user.LMPDate = userInfo.LastMenustralPeriod;
            var succ = await _store.SaveUserProfile(_userId, user);
            await context.PostAsync(string.Format("Stored LMP as {0}", userInfo.LastMenustralPeriod));
            // TODO: Record milestone
            context.Wait(MessageReceivedAsync);
        }

        #region TODO
        /*
        private async Task PostIntroduction(IDialogContext context, IAwaitable<Introduction> result)
        {
            // TODO: Store in backend that introduction is done.
            var r = await result;
            await _store.UpdateUserInfo(r.fbId, new BotToUserMilestones { UserId = r.fbId, DateMilestoneRecorded = DateTime.Now, Type = BotToUserMilestonesTypes.Introduction });
            context.Wait(MessageReceivedAsync);
        }
        */

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
        #endregion TODO
    }
}
