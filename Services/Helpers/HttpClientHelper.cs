using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PikaStatus.Enums.Status;
using Pika.Domain.Status.Data;
using PikaStatus.Models;

namespace PikaStatus.Services.Helpers
{
    internal static class HttpClientHelper
    {
        private static HttpClient? _client;
        private const int MaxRetries = 11;

        private static int Fibonacci(int n)
        {
            if (n <= 0) return 0;
            if (n == 1) return 1;
            int a = 0, b = 1;
            for (int i = 2; i <= n; i++)
            {
                int temp = a + b;
                a = b;
                b = temp;
            }
            return b;
        }

        private static async Task<ApiMessage<T>> ExecuteWithRetryAsync<T>(Func<Task<ApiMessage<T>>> action)
        {
            int attempt = 0;

            while (attempt < MaxRetries)
            {
                try
                {
                    var result = await action();
                    if (result != null && result.Status != Status.Unknown)
                    {
                        return result;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"[HttpClientHelper] Request failed on attempt {attempt + 1}: {e.Message}");
                }

                attempt++;
                if (attempt < MaxRetries)
                {
                    int delaySeconds = Fibonacci(attempt);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }

            return new ApiMessage<T> { Status = Status.Unknown };
        }

        internal static void ConfigureClient(string baseUrl, string mediaType)
        {
            if (_client != null) return;

            _client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }

        internal static async Task<ApiMessage<MessageEntity>> GetSingleMessageAsync(string endpoint)
        {
            return await ExecuteWithRetryAsync<MessageEntity>(async () =>
            {
                try
                {
                    var response = await _client!.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiMessage<MessageEntity>>(await response.Content.ReadAsStringAsync())!;
                    }
                    return new ApiMessage<MessageEntity> { Status = Status.Unknown };
                }
                catch
                {
                    return new ApiMessage<MessageEntity> { Status = Status.Unknown };
                }
            });
        }

        internal static async Task<ApiMessage<List<MessageEntity>>> GetMessagesAsync(string endpoint)
        {
            return await ExecuteWithRetryAsync<List<MessageEntity>>(async () =>
            {
                try
                {
                    var response = await _client!.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiMessage<List<MessageEntity>>>(await response.Content.ReadAsStringAsync())!;
                    }
                    return new ApiMessage<List<MessageEntity>> { Status = Status.Unknown };
                }
                catch
                {
                    return new ApiMessage<List<MessageEntity>> { Status = Status.Unknown };
                }
            });
        }

        internal static async Task<ApiMessage<List<IssueEntity>>> GetIssuesAsync(string endpoint)
        {
            return await ExecuteWithRetryAsync<List<IssueEntity>>(async () =>
            {
                try
                {
                    var response = await _client!.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiMessage<List<IssueEntity>>>(await response.Content.ReadAsStringAsync())!;
                    }
                    return new ApiMessage<List<IssueEntity>> { Status = Status.Unknown };
                }
                catch
                {
                    return new ApiMessage<List<IssueEntity>> { Status = Status.Unknown };
                }
            });
        }

        internal static async Task<ApiMessage<List<string>>> GetSystems(string endpoint)
        {
            return await ExecuteWithRetryAsync<List<string>>(async () =>
            {
                try
                {
                    var response = await _client!.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiMessage<List<string>>>(await response.Content.ReadAsStringAsync())!;
                    }
                    return new ApiMessage<List<string>> { Status = Status.Unknown };
                }
                catch
                {
                    return new ApiMessage<List<string>> { Status = Status.Unknown };
                }
            });
        }

        internal static async Task<ApiMessage<string>> GetSystemStateText(string endpoint)
        {
            return await ExecuteWithRetryAsync<string>(async () =>
            {
                try
                {
                    var response = await _client!.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiMessage<string>>(await response.Content.ReadAsStringAsync())!;
                    }
                    return new ApiMessage<string> { Status = Status.Unknown };
                }
                catch
                {
                    return new ApiMessage<string> { Status = Status.Unknown };
                }
            });
        }

        internal static async Task<ApiMessage<MessageEntity>> GetLatestMessage(string endpoint)
        {

            return await ExecuteWithRetryAsync<MessageEntity>(async () =>
            {
                try
                {
                    var response = await _client!.GetAsync(endpoint);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<ApiMessage<MessageEntity>>(await response.Content.ReadAsStringAsync())!;
                    }
                    return new ApiMessage<MessageEntity> { Status = Status.Unknown };
                }
                catch
                {
                    return new ApiMessage<MessageEntity> { Status = Status.Unknown };
                }
            });
        }
    }
}

