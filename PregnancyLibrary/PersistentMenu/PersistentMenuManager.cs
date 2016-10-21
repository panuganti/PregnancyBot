using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace PregnancyLibrary
{
    public class PersistentMenuHandler
    {
        private static string _accessToken = "EAAJHTXVZBiIQBAGP0rzpsDjRdtyar7RwxoAqGL7gOseVVF9ZA9NwZCuNerM8gV3DnzdUyjRwvlA4OR5qHTbGdm0Jh9tyINkNxNJCoUCH3WySlqz1VJO47mNqnDVPJJSdsCuuZBpiDdYrSPVyNqUtZByN5QRlZCROTAHW7AR55W4wZDZD";
        public static async Task AddPersistentMenuAsync()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri("https://graph.facebook.com");
                    var requestUri = string.Format("v2.6/me/thread_settings?access_token={0}", _accessToken);
                    var menu = GetPersistentMenu();
                    string json = JsonConvert.SerializeObject(menu, Formatting.Indented);
                    HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    var response = await client.PostAsync(requestUri, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception(e.Message);
                }
            }
        }

        private static PersistentMenu GetPersistentMenu()
        {
            var callToActions = new Call_To_Actions[]
            {
                new Call_To_Actions
                {
                    type = "postback",
                    title = "Home",
                    payload = "home"
                },
                new Call_To_Actions
                {
                    type = "postback",
                    title = "Reset",
                    payload = "reset"
                }
            };
            PersistentMenu menu = new PersistentMenu()
            {
                setting_type = "call_to_actions",
                thread_state = "existing_thread",
                call_to_actions = callToActions
            };
            return menu;
        }

        public static async Task DeletePersistentMenuAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://graph.facebook.com");
                var requestUri = string.Format("v2.6/me/thread_settings?access_token={0}", _accessToken);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                var response = await client.DeleteAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}