using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace piwebapi_cs_streetview
{
    public class PIWebAPIClient
    {
        private HttpClient client;

        public PIWebAPIClient(string username, string password) {
            client = new HttpClient();
            string authInfo = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", username, password)));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
            client.Timeout = new TimeSpan(0, 0, 10);
        }

        public async Task<JObject> GetAsync(string url) {
            HttpResponseMessage response = await client.GetAsync(url);
            string content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = " Response is not succefully: " + (int)response.StatusCode;
                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
            JObject result = JObject.Parse(content);
            return JObject.Parse(content);
        }

        public void Dispose() {
            client.Dispose();
        }
    }
}
