using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace WebApp.Entreo.Client.Services
{
    public interface ICustomHttpHelper
    {
        Task DeleteAsync(string url);
        Task<T> DeleteAsync<T>(string url);
        Task<T> GetAsync<T>(string url);
        Task<byte[]> GetByteArrayAsync(string url);
        Task<string> GetStringAsync(string url);
        Task PostAsync(string url, object value);
        Task<T> PostAsync<T>(string url, object value, string mediaType = "application/json");
        Task PutAsync(string url, object value);
        Task<T> PutAsync<T>(string url, object value);
    }

    public class CustomHttpHelper : ICustomHttpHelper
    {
        private readonly HttpClient _httpClient;
        //private readonly AuthenticationStateProvider _authStateProvider;
        //private readonly ILocalStorageService _localStorage;
        private const string Apibase = "api";
        public CustomHttpHelper(HttpClient httpClient/*, AuthenticationStateProvider authStateProvider, ILocalStorageService localStorage*/)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue
            {
                NoCache = true,
                NoStore = true,
                MaxAge = new TimeSpan(0)
            };
            //_authStateProvider = authStateProvider;
            //_localStorage = localStorage;
        }

        private async Task SetToken()
        {
            //var token = await _localStorage.GetItemAsStringAsync("authToken");
            //if (!string.IsNullOrWhiteSpace(token))
            //{
            //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            //}
        }

        public async Task<T> GetAsync<T>(string url)
        {
            await SetToken();
            string requestUri = $"{Apibase}/{url}{(url.IndexOf("&") != -1 ? "&" : "?")}{DateTime.Now.Ticks}";

            HttpResponseMessage result = await _httpClient.GetAsync(requestUri);
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    return await result.Content.ReadFromJsonAsync<T>();
                }
                catch (Exception ex)
                {
                    if (ex.Message.StartsWith("'<' is an invalid start of a value."))
                        throw new Exception("The API call response does not contain a JSON format");

                    throw ExceptionFromResponse(result, ex);
                }
            }
            else
            {
                throw ExceptionFromResponse(result);
            }
        }

        public async Task<string> GetStringAsync(string url)
        {
            await SetToken();
            string requestUri = $"{Apibase}/{url}{(url.IndexOf("&") != -1 ? "&" : "?")}{DateTime.Now.Ticks}";

            HttpResponseMessage result = await _httpClient.GetAsync(requestUri);
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    return await result.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    throw ExceptionFromResponse(result, ex);
                }
            }
            else
            {
                throw ExceptionFromResponse(result);
            }
        }

        public async Task<byte[]> GetByteArrayAsync(string url)
        {
            await SetToken();
            string requestUri = $"{Apibase}/{url}{(url.IndexOf("&") != -1 ? "&" : "?")}{DateTime.Now.Ticks}";

            HttpResponseMessage result = await _httpClient.GetAsync(requestUri);
            if (result.IsSuccessStatusCode)
            {
                try
                {
                    return await result.Content.ReadAsByteArrayAsync();
                }
                catch (Exception ex)
                {
                    throw ExceptionFromResponse(result, ex);
                }
            }
            else
            {
                throw ExceptionFromResponse(result);
            }
        }

        public async Task<T> PostAsync<T>(string url, object value, string mediaType = "application/json")
        {
            await SetToken();
            string requestUri = $"{Apibase}/{url}";

            HttpResponseMessage result;
            if (mediaType == "application/json")
                result = await _httpClient.PostAsJsonAsync(requestUri, value);
            else
                result = await _httpClient.PostAsync(requestUri, (HttpContent)value);

            if (result.IsSuccessStatusCode)
                return await result.Content.ReadFromJsonAsync<T>();
            else
                throw ExceptionFromResponse(result);
        }

        public async Task PostAsync(string url, object value)
        {
            await SetToken();

            HttpResponseMessage result = await _httpClient.PostAsJsonAsync($"{Apibase}/{url}", value);
            if (result.IsSuccessStatusCode)
                return;
            else
                throw ExceptionFromResponse(result);
        }

        public async Task PutAsync(string url, object value)
        {
            await SetToken();

            HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{Apibase}/{url}", value);
            if (result.IsSuccessStatusCode)
                return;
            else
                throw ExceptionFromResponse(result);
        }

        public async Task<T> PutAsync<T>(string url, object value)
        {
            await SetToken();

            HttpResponseMessage result = await _httpClient.PutAsJsonAsync($"{Apibase}/{url}", value);
            if (result.IsSuccessStatusCode)
                return await result.Content.ReadFromJsonAsync<T>();
            else
                throw ExceptionFromResponse(result);
        }

        public async Task DeleteAsync(string url)
        {
            await SetToken();

            HttpResponseMessage result = await _httpClient.DeleteAsync($"api/{url}");
            if (result.IsSuccessStatusCode)
                return;
            else
                throw ExceptionFromResponse(result);
        }


        public async Task<T> DeleteAsync<T>(string url)
        {
            await SetToken();

            HttpResponseMessage result = await _httpClient.DeleteAsync($"api/{url}");
            if (result.IsSuccessStatusCode)
                return await result.Content.ReadFromJsonAsync<T>();
            else
                throw ExceptionFromResponse(result);
        }

        public static Exception ExceptionFromResponse(HttpResponseMessage res, Exception innerexception = null)
        {
            string message = string.Empty;
            try
            {
                //TODO
                message = res.Content.ReadFromJsonAsync<string>().Result;

                return new Exception(message, innerexception);
            }
            catch (Exception ex)
            {
                return new Exception("Some error occured. Please refresh the page or contact support for help. The technical details of this error are logged.", ex);
            }
        }

    }
}
