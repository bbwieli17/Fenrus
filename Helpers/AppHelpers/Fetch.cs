using System.Diagnostics;
using System.Net.Http.Headers;
using Jint;

namespace Fenrus.Helpers.AppHelpers;

/// <summary>
/// Fetch helper
/// </summary>
public class Fetch
{
    public class FetchArgs
    {
        public Engine Engine { get; set; }
        public string AppUrl { get; set; }
        public object Parameters { get; set; }
        public Action<string> Log { get; set; }
    }
    
    /// <summary>
    /// Gets an instance of the fetch helper
    /// </summary>
    public static async Task<object> Execute(FetchArgs args)
    {
        try
        {
            var engine = args.Engine;
            var appUrl = args.AppUrl;
            var parameters = args.Parameters;

            var handler = new HttpClientHandler();
            handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            using HttpClient client = new HttpClient(handler);
            var request = new HttpRequestMessage();
            request.Method = HttpMethod.Get;
            string url;
            int timeout = 10;
            if (parameters is string str)
            {
                url = str;
                request.Headers.Add("Accept", "application/json");
            }
            else
            {
                var fp = JsonSerializer.Deserialize<FetchParameters>(
                    JsonSerializer.Serialize(parameters)
                    , new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                url = fp.Url;
                if (fp.Timeout > 0)
                {
                    timeout = fp.Timeout;
                }

                if (string.IsNullOrEmpty(fp.Body) == false)
                {
                    request.Content = new StringContent(fp.Body);
                }
                request.Headers.Clear();
                fp.Headers ??= new();
                if (fp.Headers.ContainsKey("Accept") == false)
                    fp.Headers.Add("Accept", "application/json");
                foreach (var header in fp.Headers)
                {
                    try
                    {
                        if (header.Key == "Content-Type")
                            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value);
                        else
                            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    catch (Exception)
                    {
                    }
                }
                

                request.Method = fp.Method?.ToLower() switch
                {
                    "post" => HttpMethod.Post,
                    "put" => HttpMethod.Put,
                    "delete" => HttpMethod.Delete,
                    "patch" => HttpMethod.Patch,
                    _ => HttpMethod.Get
                };
            }

            if (url.StartsWith("http") == false)
            {
                if (appUrl.EndsWith('/') == false)
                    url = appUrl + '/' + url;
                else
                    url = appUrl + url;
            }

            request.RequestUri = new Uri(url);

            var (success, content) = Send(client, request, timeout);
            if (success == false)
                throw new Exception("Timeout");

            var trimmed = content.Trim();
            if (trimmed.StartsWith("{") || trimmed.StartsWith("["))
            {
                try
                {
                    engine.SetValue("temp_json", content);
                    var parsed = engine.Evaluate("JSON.parse(temp_json)").ToObject();
                    return parsed;
                }
                catch (Exception ex)
                {
                    return content;
                }
            }

            if (trimmed == "true") return true;
            if (trimmed == "false") return false;
            if (double.TryParse(trimmed, out double dbl))
                return dbl;
            return content;
        }
        catch (Exception ex)
        {
            return new
            {
                exception = true,
                error = true,
                message = ex.Message
            };
        }
    }

    /// <summary>
    /// Sends a request and returns the result
    /// </summary>
    /// <param name="client">the HTTP Client</param>
    /// <param name="request">the message to send</param>
    /// <param name="timeout">the timeout in seconds for the request</param>
    /// <returns>the result of the request</returns>
    private static (bool Success, string Content) Send(HttpClient client, HttpRequestMessage request,
        int timeout)
    {
        bool success = false;
        string content = string.Empty;
        bool done = false;

        var send = Task.Run(() =>
        {
            client.Timeout = TimeSpan.FromSeconds(timeout);
            var cts = new CancellationTokenSource();

            var result = client.SendAsync(request, cts.Token).Result;
            if (done)
                return;
            content = result.Content.ReadAsStringAsync(cts.Token).Result;
            success = true;
        });

        Task.WhenAny(send, Task.Delay(timeout * 1_000)).Wait();
        done = true;
        return (success, content);
    }

    /// <summary>
    /// Fetch parameters
    /// </summary>
    class FetchParameters
    {
        /// <summary>
        /// Gets or sets the URL to get
        /// </summary>
        public string Url { get; set;}
        /// <summary>
        /// Gets or sets timeout in seconds
        /// </summary>
        public int Timeout { get; set; }
        
        /// <summary>
        /// Gets or sets the request Method
        /// </summary>
        public string Method { get; set; }
        
        /// <summary>
        /// Gets or sets request headers
        /// </summary>
        public Dictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the body of the request
        /// </summary>
        public string Body { get; set; }
    }
}