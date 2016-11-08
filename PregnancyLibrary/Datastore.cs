using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Net.Http;

namespace PregnancyLibrary
{
    [Serializable]
    public class Datastore
    {
        #region Singleton
        public static Datastore Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Datastore();
                }
                return _instance;
            }
        }

        public static Datastore _instance;

        #endregion Singleton

        string url = "http://localhost:54909";

        #region UserProfile
        private string _profilesDir = "profiles";
        private string _conversationStatusDir = "conversation";

        public async Task<bool> UserProfileExistsAsync(string fbId)
        {
            string serverPath = string.Format("{0}/user/{1}",url, fbId);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetJsonResponseAsync(serverPath);
            }
            return true;
        }

        public async Task<bool> InitUserProfile(string fbId, User user)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostEntityAsync<User, bool>(url, user);
                return response;
            }
        }

        public async Task<bool> UpdateUserInfo(string fbId, BotToUserMilestones milestone)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostEntityAsync<BotToUserMilestones, bool>(url, milestone);
                return response;
            }
        }

        public async Task<User> GetUserProfile(string fbid)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetEntityAsync<User>(url);
                return response;
            }
        }

        #endregion UserProfile

    }
}
