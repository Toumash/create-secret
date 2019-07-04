using RestSharp;
using System;
using System.Collections.Generic;

namespace createsecret
{
    class SecretGenerator
    {
        const string secret_url = "https://onetimesecret.com/secret/{0}";

        static void Main(string[] args)
        {
            const string bitly_token = "a2218c030f8e740a5889264c984540830153b8dc";

            string secret = null;
            if (args.Length >0 )
            {
                secret = args[0];
            }
            else if (Environment.UserInteractive)
            {
                Console.Write("secret: ");
                secret = Console.ReadLine();

            }
            else
            {
                Console.WriteLine($"USAGE: {args[0]} your-secret-here");
                return;
            }

            var secretLink = GenerateSecret(secret_url, secret);
            Console.Write("processing...");
            var shortenLink = Shorten(bitly_token, secretLink);
            Console.WriteLine("ok\n" + shortenLink);
        }

        private static string GenerateSecret(string secret_url, string secret)
        {
            var client = new RestClient("https://onetimesecret.com/api/v1/share");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Host", "onetimesecret.com");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddParameter("application/x-www-form-urlencoded", $"secret={secret}", ParameterType.RequestBody);
            var response = client.Execute<SecretDto>(request);
            var secretlink = string.Format(secret_url, response.Data.secret_key);
            return secretlink;
        }

        private static string Shorten(string bitly_token, string link)
        {
            var client = new RestClient("https://api-ssl.bitly.com/v4/shorten");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("accept-encoding", "gzip, deflate");
            request.AddHeader("Host", "api-ssl.bitly.com");
            request.AddHeader("Cache-Control", "no-cache");
            request.AddHeader("Accept", "*/*");
            request.AddHeader("Authorization", "Bearer " + bitly_token);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefined", $"{{\"long_url\": \"{link}\"}}", ParameterType.RequestBody);
            var response = client.Execute<RootObject>(request);
            var shortLink = response.Data.link;
            return shortLink;
        }
    }
    public class References
    {
        public string group { get; set; }
    }

    public class RootObject
    {
        public DateTime created_at { get; set; }
        public string id { get; set; }
        public string link { get; set; }
        public List<object> custom_bitlinks { get; set; }
        public string long_url { get; set; }
        public bool archived { get; set; }
        public List<object> tags { get; set; }
        public List<object> deeplinks { get; set; }
        public References references { get; set; }
    }

    class SecretDto
    {
        public string custid { get; set; }
        public string metadata_key { get; set; }
        public string secret_key { get; set; }
        public int ttl { get; set; }
        public int metadata_ttl { get; set; }
        public int secret_ttl { get; set; }
        public string state { get; set; }
        public int updated { get; set; }
        public int created { get; set; }
        public List<object> recipient { get; set; }
        public bool passphrase_required { get; set; }
    }
}
