using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace PregnancyLibrary
{
    public class PersistentMenuHandler
    {
        private static string _accessToken = "EAAJHTXVZBiIQBAGP0rzpsDjRdtyar7RwxoAqGL7gOseVVF9ZA9NwZCuNerM8gV3DnzdUyjRwvlA4OR5qHTbGdm0Jh9tyINkNxNJCoUCH3WySlqz1VJO47mNqnDVPJJSdsCuuZBpiDdYrSPVyNqUtZByN5QRlZCROTAHW7AR55W4wZDZD";
        /*
 * Persistent Menu
 * ===============
curl -X POST -H "Content-Type: application/json" -d '{
  "setting_type" : "call_to_actions",
  "thread_state" : "existing_thread",
  "call_to_actions":[
    {
      "type":"postback",
      "title":"Help",
      "payload":"DEVELOPER_DEFINED_PAYLOAD_FOR_HELP"
    },
    {
      "type":"postback",
      "title":"Start a New Order",
      "payload":"DEVELOPER_DEFINED_PAYLOAD_FOR_START_ORDER"
    },
    {
      "type":"web_url",
      "title":"Checkout",
      "url":"http://petersapparel.parseapp.com/checkout",
      "webview_height_ratio": "full",
      "messenger_extensions": true
    },
    {
      "type":"web_url",
      "title":"View Website",
      "url":"http://petersapparel.parseapp.com/"
    }
  ]
}' "https://graph.facebook.com/v2.6/me/thread_settings?access_token=PAGE_ACCESS_TOKEN" 


curl -X DELETE -H "Content-Type: application/json" -d '{
  "setting_type":"call_to_actions",
  "thread_state":"existing_thread"
}' "https://graph.facebook.com/v2.6/me/thread_settings?access_token=PAGE_ACCESS_TOKEN"    

*/
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
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
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
                var requestUri = string.Format("https://graph.facebook.com/v2.6/me/thread_settings?access_token={0}", _accessToken);
                client.DefaultRequestHeaders.Accept.Clear();
                var response = await client.DeleteAsync(requestUri);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }


    [DataContract]
    public class PersistentMenu
    {
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string setting_type { get; set; }
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string thread_state { get; set; }
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Call_To_Actions[] call_to_actions { get; set; }
    }

    [DataContract]
    public class Call_To_Actions
    {
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string type { get; set; }
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string title { get; set; }
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string payload { get; set; }
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string url { get; set; }
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string webview_height_ratio { get; set; }
        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? messenger_extensions { get; set; }
    }
}
