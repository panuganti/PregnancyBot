using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace PregnancyLibrary
{
    [Serializable]
    public class IntroductionDialog : IDialog<IntroductionDialog>
    {
        private Datastore _store;
        private string _userId;

        public IntroductionDialog(Datastore store, string userId)
        {
            _store = store;
            _userId = userId;
        }

        public async Task StartAsync(IDialogContext context)
        {
            await Task.Run(() => context.Wait(MessageReceivedAsync)); // There is a message in context and hence will get called immediately.
        }

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
            context.Wait(MessageReceivedAsync);
        }

        private async Task PostUserInfoForm(IDialogContext context, IAwaitable<UserInfoForm> result)
        {
            UserInfoForm userInfo = await result;
            var user = await _store.GetUserProfile(_userId);
            user.LMPDate = userInfo.LastMenustralPeriod;
            var succ = await _store.SaveUserProfile(_userId, user);
            await context.PostAsync(string.Format("Stored LMP as {0}", userInfo.LastMenustralPeriod));
            context.Wait(MessageReceivedAsync);
        }
    }
}
