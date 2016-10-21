
using Google.Apis.Auth.OAuth2;
using Google.Apis.Http;
using Google.Apis.Services;
using Google.Apis.Storage.v1;
using Google.Apis.Storage.v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using Google.Apis.Download;
using System.Text;

namespace CloudStorage
{
    public class GoogleCloud
    {
        private const string projectId = "wwwarchishainnovatorscom";
        private const string bucketName = "www.archishainnovators.com";
        private readonly string _rootDir;
        private readonly string _appName;

        #region Singleton
        public static GoogleCloud GetInstance(string rootDir, string appName)
        {
            if (_instance == null)
            {
                var publicACL = new List<ObjectAccessControl>
                {
                    new ObjectAccessControl
                    {
                        Role = "OWNER",
                        Entity = "allUsers"
                    }
                };
                IConfigurableHttpClientInitializer credentials = GetApplicationDefaultCredentials();
                var storageService = new StorageService(
            new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
                ApplicationName = appName, // NewsSwipes
            });
                _instance = new GoogleCloud(rootDir, appName, publicACL, storageService);
                return _instance;
            }
            else { return _instance; }
        }

        private static IConfigurableHttpClientInitializer GetApplicationDefaultCredentials()
        {
            string docPath;
            if (File.Exists(Path.Combine(Environment.CurrentDirectory, "wwwarchishainnovatorscom-eaed27291ff7.json")))
            {
                docPath = Path.Combine(Environment.CurrentDirectory, "wwwarchishainnovatorscom-eaed27291ff7.json");
            }
            else
            {
                docPath = HttpContext.Current.Server.MapPath("/bin/wwwarchishainnovatorscom-eaed27291ff7.json");
            }
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", docPath, EnvironmentVariableTarget.Process);

            try
            {
                GoogleCredential credential =
                    GoogleCredential.GetApplicationDefaultAsync().Result;
                if (credential.IsCreateScopedRequired)
                {
                    credential = credential.CreateScoped(new[] {
                    StorageService.Scope.DevstorageReadWrite
                });
                }
                return credential;
            }
            catch (AggregateException exception)
            {
                throw new Exception(String.Join("\n", exception.Flatten().InnerExceptions.Select(t => t.Message)));
            }
        }

        public GoogleCloud(string rootDir, string appName, List<ObjectAccessControl> publicACL, StorageService storageService)
        {
            _rootDir = rootDir;
            _appName = appName;
            _publicACL = publicACL;
            _storageService = storageService;
        }

        private static GoogleCloud _instance;
        #endregion Singleton

        public async Task<bool> UploadImageAsync(string filename, string imageUrl, string dir, bool isPublic = false)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    byte[] data = webClient.DownloadData(imageUrl);
                    MemoryStream mem = new MemoryStream(data);
                    return await UploadAsync(filename, mem, "image/jpeg", dir, isPublic);
                }
                catch (Exception e)
                {
                    throw e; // TODO: Log exception
                }
            }
        }

        public async Task<bool> UploadStorageInfoAsync(string filename, string data, string dir, bool isPublic = false)
        {
            MemoryStream mem = new MemoryStream();
            using (StreamWriter writer = new StreamWriter(mem))
            {
                writer.Write(data);
                writer.Flush();
                mem.Position = 0;
                return await UploadAsync(filename, mem, "text/plain", dir, isPublic);
            }
        }

        private async Task<bool> UploadAsync(string filename, MemoryStream mem, string datatype, string dirName, bool isPublic)
        {
            string file = String.Format("{0}/{1}/{2}", _rootDir, dirName, filename);
            Google.Apis.Storage.v1.Data.Object fileobj;
            if (isPublic)
            {
                fileobj = new Google.Apis.Storage.v1.Data.Object() { Name = file, Acl = _publicACL };
            }
            else
            {
                fileobj = new Google.Apis.Storage.v1.Data.Object() { Name = file };
            }
            await _storageService.Objects.Insert(fileobj, bucketName, mem, datatype).UploadAsync();
            return true;
        }

        public async Task<bool> FileExistsAsync(string filename, string dirName)
        {
            var request = _storageService.Objects.List(bucketName);
            var children = await request.ExecuteAsync();
            var objectName = String.Format("{0}/{1}/{2}", _rootDir, dirName, filename);
            return children.Items.Any(c => c.Name == objectName);
        }

        public async Task DeleteFileAsync(string filename, string dirName)
        {
            var objectName = String.Format("{0}/{1}/{2}", _rootDir, dirName, filename);
            ObjectsResource.DeleteRequest request = _storageService.Objects.Delete(bucketName, objectName);
            var str = await request.ExecuteAsync();
        }

        public async Task<string> GetAllTextAsync(string filename, string dirName)
        {
            using (var stream = new MemoryStream())
            {
                var objectName = String.Format("{0}/{1}/{2}", _rootDir, dirName, filename);
                ObjectsResource.GetRequest request = _storageService.Objects.Get(bucketName, objectName);
                IDownloadProgress progress = await request.DownloadAsync(stream);
                return Encoding.UTF8.GetString(stream.GetBuffer());
            }
        }
        private readonly StorageService _storageService;
        private readonly List<ObjectAccessControl> _publicACL;
    }
}
