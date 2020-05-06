using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PikaStatus.Models;

namespace PikaStatus.Services.Helpers
{
    internal static class HttpClientHelper
    {
        private static HttpClient _client;

        internal static void ConfigureClient(string baseUrl, string mediaType)
        {
            if (_client != null)
            {
                return;
            }

            _client = new HttpClient {BaseAddress = new Uri(baseUrl)};
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(mediaType));
        }

        internal static async Task<ApiMessage<MessageEntity>> GetSingleMessageAsync(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            var apiMessage = new ApiMessage<MessageEntity>();
            if (response.IsSuccessStatusCode)
            {
                apiMessage = JsonConvert
                    .DeserializeObject<ApiMessage<MessageEntity>>(
                        await response.Content.ReadAsStringAsync()
                        );
            }
            return apiMessage;
        }

        internal static async Task<ApiMessage<List<MessageEntity>>> GetMessagesAsync(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            var apiMessage = new ApiMessage<List<MessageEntity>>();
            if (response.IsSuccessStatusCode)
            {
                apiMessage = JsonConvert
                    .DeserializeObject<ApiMessage<List<MessageEntity>>>(
                        await response.Content.ReadAsStringAsync()
                    );
            }
            return apiMessage;
        }

        internal static async Task<ApiMessage<MessageEntity>> GetLatestMessage(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            var apiMessage = new ApiMessage<MessageEntity>();
            if (response.IsSuccessStatusCode)
            {
                apiMessage = JsonConvert
                    .DeserializeObject<ApiMessage<MessageEntity>>(
                        await response.Content.ReadAsStringAsync()
                    );
            }
            return apiMessage;
        }
        
        internal static async Task<ApiMessage<List<string>>> GetSystems(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            var apiMessage = new ApiMessage<List<string>>();
            if (response.IsSuccessStatusCode)
            {
                apiMessage = JsonConvert
                    .DeserializeObject<ApiMessage<List<string>>>(
                        await response.Content.ReadAsStringAsync()                     
                    );
            }
            return apiMessage;
        }
        
        internal static async Task<ApiMessage<string>> GetSystemStateText(string endpoint)
        {
            var response = await _client.GetAsync(endpoint);
            var apiMessage = new ApiMessage<string>();
            if (response.IsSuccessStatusCode)
            {
                var c = await response.Content.ReadAsStringAsync();
                apiMessage = JsonConvert
                    .DeserializeObject<ApiMessage<string>>(
                                           c
                    );
            }
            return apiMessage;
        }

        internal static void Dispose()
        {
            _client.CancelPendingRequests();
            _client = null;
        }

        internal static bool IsDisposed()
        {
            return _client != null;
        }
    }
}