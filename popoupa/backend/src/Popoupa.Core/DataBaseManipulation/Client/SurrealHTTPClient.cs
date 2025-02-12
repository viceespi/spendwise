

using System.Net.Http.Headers;
using System.Text;
using Npgsql.Replication;

namespace Popoupa.Core.DataBaseManipulation.Client
{
    public class SurrealHTTPClient
    {
        public SurrealHTTPClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("NS", "popoupa");
            client.DefaultRequestHeaders.Add("DB", "popoupadb");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var byteArray = Encoding.ASCII.GetBytes("popoupa:popoupa100%");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            Client = client;
        }
        public HttpClient Client { get; set; }
    }
}