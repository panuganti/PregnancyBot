using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
#pragma warning disable 649

namespace PregnancyLibrary
{
    [Serializable]
    public class DailyVitals
    {
        [Prompt("Please enter Date of your Last Menustral Period.\n This is essential for tuning the bot to you and we cannot skip")]
        [Template(TemplateUsage.NotUnderstood, "Once you enter date, I'll respond to your other questions", "Sorry, I can understand only Date format", "Date only Please", "Enter it in mm/dd/yyyy format")]
        public DateTime LastMenustralPeriod { get; set; }

        public static IDialog<UserInfoForm> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(UserInfoForm.BuildForm, FormOptions.PromptInStart));
        }
        public static IForm<UserInfoForm> BuildForm()
        {
            OnCompletionAsyncDelegate<UserInfoForm> processOrder = async (context, state) =>
            {
                // Store in database..
                await context.PostAsync(String.Format("Great! You are now {0} Weeks and {1} days pregnant", (int)(DateTime.Now - state.LastMenustralPeriod).Days/7, (DateTime.Now - state.LastMenustralPeriod).Days % 7));
            };

            return new FormBuilder<UserInfoForm>()
                    .AddRemainingFields()
                    .OnCompletion(processOrder)
                    .Build();
        }
    };
}