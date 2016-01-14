using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OneSignalFacade
{
    public class OneSignalService
    {
        private string appId;
        private string apiKey;
        private string url;

        public OneSignalService(string url, string appId, string apiKey)
        {
            this.appId = appId;
            this.apiKey = apiKey;
            this.url = url;
        }

        public async Task CreateNotification(string obj)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Post,
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jsonn"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", apiKey);

                try
                {
                    var response = await client.SendAsync(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine();
                    }
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the URI of the created resource.
                        Uri gizmoUrl = response.Headers.Location;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.InnerException);
                }
            }
        }
    }
}