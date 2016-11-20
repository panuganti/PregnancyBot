using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using System.Net.Http;
using PregnancyLibrary.DataContracts;

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

        /*
        private string _profilesDir = "profiles";
        private string _conversationStatusDir = "conversation";
        */
        #region Generics

        private async Task<bool> FileExistsAsync(string dirName, string filename)
        {
            string serverPath = string.Format("{0}/user/FileExists/{1}/{2}", url, dirName, filename);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetJsonResponseAsync(serverPath);
                return bool.Parse(response);
            }
        }

        private async Task<bool> StoreEntityAsync<T>(string dirName, string filename, T entity)
        {
            string serverPath = string.Format("{0}/user/StoreEntity/{1}/{2}", url, dirName, filename);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostEntityAsync<T, bool>(serverPath, entity);
                return true;
            }
        }

        private async Task<T> GetFileAsync<T>(string fbId, string filename) where T:class
        {
            string serverPath = string.Format("{0}/user/GetEntity/{1}/{2}", url, fbId, filename);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetEntityAsync<T>(serverPath);
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

        public async Task<HttpResponseMessage> DeleteDirectoryAsync(string fbId)
        {
            string serverPath = string.Format("{0}/user/DeleteDirectory/{1}", url, fbId);
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(serverPath);
                return response;
            }
        }

        #endregion Generics

        #region UserProfile
        public async Task<bool> SaveUserProfile(string fbId, User user)
        {
            return await StoreEntityAsync(fbId, _userprofileFilename, user);
        }

        public async Task<bool> UserProfileExistsAsync(string fbId)
        {
            return await FileExistsAsync(fbId, _userprofileFilename);
        }

        public async Task<User> GetUserProfileAsync(string fbId)
        {
            return await GetFileAsync<User>(fbId, _userprofileFilename);
        }

        public async Task<bool> UpdateUserInfo(string fbId, BotToUserMilestones milestone)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.PostEntityAsync<BotToUserMilestones, bool>(url, milestone);
                return response;
            }
        }

        #endregion UserProfile

        #region Daily
        public async Task<string> GetDailyTip()
        {
            return await GetFileAsync<string>(_dailyTipsFolder, "");
        }

        #endregion Daily

        private string _dailyTipsFolder = "dailytips";
        private string _userprofileFilename = "userprofile";

    }
}
