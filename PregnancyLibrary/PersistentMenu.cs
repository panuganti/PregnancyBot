using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
                var requestUri = string.Format("https://graph.facebook.com/v2.6/me/thread_settings?access_token={0}", _accessToken);
                client.DefaultRequestHeaders.Accept.Clear();

                var menu = GetPersistentMenu();
                string json = JsonConvert.SerializeObject(menu);
                var response = await client.PostAsJsonAsync(requestUri, json);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
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
        public string setting_type { get; set; }
        [DataMember]
        public string thread_state { get; set; }
        [DataMember]
        public Call_To_Actions[] call_to_actions { get; set; }
    }

    [DataContract]
    public class Call_To_Actions
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string payload { get; set; }
        [DataMember]
        public string url { get; set; }
        [DataMember]
        public string webview_height_ratio { get; set; }
        [DataMember]
        public bool messenger_extensions { get; set; }
    }
}
