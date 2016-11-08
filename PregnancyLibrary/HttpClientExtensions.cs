using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PregnancyLibrary
{
    public static class HttpClientExtensions
    {
        private const string ContentType = "application/json";

        public static async Task<T> GetEntityAsync<T>(this HttpClient client, string serverPath) where T: class
        {
            return await GetEntitySharedAsync<T>(client, serverPath);
        }

        public static async Task<T> TryGetEntityAsync<T>(this HttpClient client, string serverPath) where T : class
        {
            return await GetEntitySharedAsync<T>(client, serverPath, tryGet:true);
        }

        public static async Task<T> ListEntityAsync<T>(this HttpClient client, string serverPath, string continuationToken, int batchSize)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, serverPath))
            {
                request.Headers.Add("ContinuationToken", continuationToken);
                request.Headers.Add("BatchSize", batchSize.ToString(CultureInfo.InvariantCulture));

                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    await EnsureSuccessStatusCodeAsync(response);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(responseBody, new StringEnumConverter());
                }
            }
        }

        public static async Task<Tuple<IEnumerable<T>, string>> ListHistorySinceAsync<T>(this HttpClient client,
            string serverPath, string continuationToken, int batchSize)
        {
            return await ListEntityAsync<Tuple<IEnumerable<T>, string>>(client, serverPath, continuationToken, batchSize);
        }

        public static async Task<T> PostEntityAsync<T>(this HttpClient client, string serverPath, T entity)
        {
            return await PostEntityAsync<T, T>(client, serverPath, entity);
        }

        public static async Task<TRet> PostEntityAsync<T, TRet>(this HttpClient client, string serverPath, T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            return await PostJsonAsync<TRet>(client, serverPath, json);
        }

        public static async Task PostEntityNoReturnAsync<T>(this HttpClient client, string serverPath, T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            await PostJsonNoReturnAsync(client, serverPath, json);
        }

        public static async Task<TRet> PostJsonAsync<TRet>(this HttpClient client, string serverPath, string json,
            bool ensureStrictSuccess = false)
        {
            using (HttpContent content = new StringContent(json, Encoding.UTF8, ContentType))
            {
                using (HttpResponseMessage response = await client.PostAsync(serverPath, content))
                {
                    await EnsureSuccessStatusCodeAsync(response, ensureStrictSuccess);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TRet>(responseBody, new StringEnumConverter());
                }
            }
        }

        private static async Task PostJsonNoReturnAsync(this HttpClient client, string serverPath, string json)
        {
            using (HttpContent content = new StringContent(json, Encoding.UTF8, ContentType))
            {
                using (HttpResponseMessage response = await client.PostAsync(serverPath, content))
                {
                    await EnsureSuccessStatusCodeAsync(response);
                }
            }
        }

        public static async Task<T> PutEntityAsync<T>(this HttpClient client, string serverPath, T entity)
        {
            return await PutEntityAsync<T, T>(client, serverPath, entity);
        }

        public static async Task<TRet> PutEntityAsync<T, TRet>(this HttpClient client, string serverPath, T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            using (HttpContent content = new StringContent(json, Encoding.UTF8, ContentType))
            {
                using (HttpResponseMessage response = await client.PutAsync(serverPath, content))
                {
                    await EnsureSuccessStatusCodeAsync(response);

                    string responseBody = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<TRet>(responseBody, new StringEnumConverter());
                }
            }
        }

        public static async Task PutEntityNoReturnAsync<T>(this HttpClient client, string serverPath, T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            using (HttpContent content = new StringContent(json, Encoding.UTF8, ContentType))
            {
                using (HttpResponseMessage response = await client.PutAsync(serverPath, content))
                {
                    await EnsureSuccessStatusCodeAsync(response);
                }
            }
        }

        public static async Task DeleteEntityAsync(this HttpClient client, string serverPath, string etag)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, serverPath))
            {
                request.Headers.Add("Etag", etag);

                using (HttpResponseMessage response = await client.SendAsync(request))
                {
                    await EnsureSuccessStatusCodeAsync(response);
                }
            }
        }

        /// <summary>
        /// Throws an exception if the response.IsSuccessStatusCode property for the HTTP response is false.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="ensureStrictSuccess">If true, throw exception unless status code is 200 (OK)</param>
        /// <returns></returns>
        public static async Task EnsureSuccessStatusCodeAsync(this HttpResponseMessage response, bool ensureStrictSuccess = false)
        {
            if (response.StatusCode == HttpStatusCode.OK || (!ensureStrictSuccess && response.IsSuccessStatusCode))
            {
                return;
            }

            string detailContent = null;
            if (response.Content != null)
            {
                try
                {
                    detailContent = await response.Content.ReadAsStringAsync();
                }
                catch (Exception)
                {
                }
                response.Content.Dispose();
            }
            /*
            if (response.StatusCode == HttpStatusCode.PreconditionFailed)
            {
                throw new EtagMismatchException(string.Empty, detailContent, inner: null);
            }
            */

            throw new HttpRequestException(
                string.Format("Response status code does not indicate success: {0} ({1}).\n{2}",
                                (int) response.StatusCode, response.ReasonPhrase, detailContent));
        }

        public static async Task<string> GetJsonResponseAsync(this HttpClient client, string serverPath, bool tryGet = false)
        {
            HttpStatusCode? lastHttpStatusCode = null;
            try
            {
                using (HttpResponseMessage response = await client.GetAsync(serverPath))
                {
                    if (tryGet && response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return null;
                    }

                    await EnsureSuccessStatusCodeAsync(response);
                    lastHttpStatusCode = response.StatusCode;

                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
            }
            catch (HttpRequestException)
            {
                string statusCodeMsg = "N/A";
                if (lastHttpStatusCode.HasValue)
                {
                    statusCodeMsg = lastHttpStatusCode.Value.ToString();
                }
                throw new HttpRequestException(string.Format("Got exception while reading response from server {0} with status code {1}", serverPath, statusCodeMsg));
            }
        }

        private static async Task<T> GetEntitySharedAsync<T>(this HttpClient client, string serverPath, bool tryGet = false) where T : class
        {
            string responseBody = await GetJsonResponseAsync(client, serverPath, tryGet);
            return responseBody == null ? null : JsonConvert.DeserializeObject<T>(responseBody, new StringEnumConverter());
        }

        public static async Task<T> GetValueAsync<T>(this HttpClient client, string serverPath) where T : struct
        {
            string responseBody = await GetJsonResponseAsync(client, serverPath, tryGet: false);
            return JsonConvert.DeserializeObject<T>(responseBody, new StringEnumConverter());
        }

        // TODO: Andrepro. can't be async in current form (.. of c# :) )
        public static IEnumerable<T> GetEnumerable<T>(this HttpClient client, string serverPath) where T : class
        {
            using (Stream stream = client.GetStreamAsync(serverPath).Result)
            {
                using (var reader = new StreamReader(stream))
                {
                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var serializer = new JsonSerializer();
                        serializer.Converters.Add(new StringEnumConverter());

                        if (!jsonReader.Read() || jsonReader.TokenType != JsonToken.StartArray)
                        {
                            throw new HttpRequestException("Expected start of array");
                        }
                        while (jsonReader.Read() && jsonReader.TokenType != JsonToken.EndArray)
                        {
                            yield return serializer.Deserialize<T>(jsonReader);
                        }

                        if (jsonReader.TokenType != JsonToken.EndArray)
                        {
                            throw new HttpRequestException("Array is not complete");
                        }
                    }
                }
            }
        }
    }
}
