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

        private async Task<bool> FileExistsAsync(string dirName, string filename)
        {
            string serverPath = string.Format("{0}/user/FileExists/{1}/{2}", url, dirName, filename);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetJsonResponseAsync(serverPath);
                return bool.Parse(response);
            }
        }

        public async Task<bool> UserProfileExistsAsync(string fbId)
        {
            return await FileExistsAsync(fbId, _userprofileFilename);
        }

        private async Task<bool> StoreEntityAsync<T>(string dirName, string filename, T entity)
        {
            string serverPath = string.Format("{0}/user/StoreEntity/{1}/{2}", url, dirName, filename);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostEntityAsync(serverPath, entity);
                return true;
            }
        }

        public async Task<bool> SaveUserProfile(string fbId, User user)
        {
            return await StoreEntityAsync(fbId, _userprofileFilename, user);
        }

        public async Task<User> GetUserProfile(string fbId)
        {
            string serverPath = string.Format("{0}/user/GetUserProfile/{1}", url, fbId);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetEntityAsync<User>(serverPath);
                return response;
            }
        }

        public async Task<HttpResponseMessage> DeleteUserProfileAsync(string fbId)
        {
            string serverPath = string.Format("{0}/user/DeleteDirectory/{1}", url, fbId);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(serverPath);
                return response;
            }
        }

        private async Task<HttpResponseMessage> DeleteFileAsync(string fbId, string filename)
        {
            string serverPath = string.Format("{0}/user/DeleteFile/{1}", url, fbId, filename);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(serverPath);
                return response;
            }
        }

        #endregion UserProfile

        public async Task<bool> UpdateUserInfo(string fbId, BotToUserMilestones milestone)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostEntityAsync<BotToUserMilestones, bool>(url, milestone);
                return response;
            }
        }

        private string _userprofileFilename = "userprofile";

    }
}
