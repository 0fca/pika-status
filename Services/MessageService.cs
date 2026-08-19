using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Pika.Domain.Status.Data;
using Pika.Domain.Status.Models;
using PikaStatus.Enums.Status;
using PikaStatus.Services.Helpers;

namespace PikaStatus.Services
{
    public class MessageService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheTtl = TimeSpan.FromMinutes(2);

        public MessageService(IConfiguration configuration, IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
            HttpClientHelper.ConfigureClient(configuration.GetConnectionString("StatusApiBase")
                ?? throw new InvalidOperationException("Connection string 'StatusApiBase' is not configured."),
                MediaTypeNames.Application.Json);
        }

        private async Task<T> GetCachedAsync<T>(string cacheKey, Func<Task<T>> factory)
        {
            if (_cache.TryGetValue(cacheKey, out T cachedValue))
            {
                return cachedValue;
            }

            var value = await factory();
            _cache.Set(cacheKey, value, _cacheTtl);
            Console.WriteLine(value);
            return value;
        }

        public async Task<Tuple<string, bool>> GetOverallStatus()
        {
            var endpoint = _configuration.GetConnectionString("OverallStatusEndpoint");
            if (string.IsNullOrEmpty(endpoint))
            {
                Serilog.Log.Warning("[MessageService] OverallStatusEndpoint connection string is null or empty.");
                return new Tuple<string, bool>("Cloud status is temporarily unavailable.", false);
            }

            return await GetCachedAsync($"status_overall_{endpoint}", async () =>
           {
               Serilog.Log.Information("[MessageService] Fetching overall status from endpoint: {Endpoint}", endpoint);
               var statusMessage = await HttpClientHelper.GetSingleMessageAsync(endpoint);
               var message = statusMessage.Messages != null && statusMessage.Messages.Count > 0
                   ? statusMessage.Messages.First()
                   : "Cloud status is temporarily unavailable.";
               Serilog.Log.Information("[MessageService] Overall status fetch result: {Status}", statusMessage.Status);
               return new Tuple<string, bool>(message, statusMessage.Status == Status.Success);
           });
        }

        public async Task<Tuple<Stack<string>, bool>> GetOverallStatusDetailed()
        {
            var endpoint = _configuration.GetConnectionString("OverallStatusEndpoint");
            if (string.IsNullOrEmpty(endpoint))
            {
                return new Tuple<Stack<string>, bool>(new Stack<string>(["Cloud status is temporarily unavailable."]), false);
            }

            return await GetCachedAsync($"status_detailed_{endpoint}", async () =>
           {
               var statusMessage = await HttpClientHelper.GetSingleMessageAsync(endpoint);
               return new Tuple<Stack<string>, bool>(statusMessage.Messages, statusMessage.Status == Status.Success);
           });
        }

        public async Task<Tuple<bool, List<MessageEntity>>> GetMessages(string systemName, int count = 25, int offset = 0, MessageType? messageType = null)
        {
            var endpointFormat = _configuration.GetConnectionString("MessagesEndpoint");
            if (string.IsNullOrEmpty(endpointFormat))
            {
                return new Tuple<bool, List<MessageEntity>>(false, []);
            }

            var baseUrl = string.Format(endpointFormat, systemName);

            var queryParameters = new List<string>
            {
                "order=1",
                $"offset={offset}",
                $"count={count}"
            };

            if (messageType.HasValue)
            {
                queryParameters.Add($"messageType={(int)messageType.Value}");
            }

            var url = string.Concat(baseUrl, "?", string.Join("&", queryParameters));
            if (string.IsNullOrEmpty(url))
            {
                return new Tuple<bool, List<MessageEntity>>(false, []);
            }

            string cacheKey = $"messages_{systemName}_{count}_{offset}_{messageType}";
            return await GetCachedAsync(cacheKey, async () =>
           {
               var message = await HttpClientHelper.GetMessagesAsync(url);
               var items = message.Data ?? [];
               return new Tuple<bool, List<MessageEntity>>(message.Status == Status.Success, items);
           });
        }

        public async Task<Tuple<bool, List<IssueEntity>>> GetIssues(string name, int id)
        {
            var endpointFormat = _configuration.GetConnectionString("IssuesEndpoint");
            if (string.IsNullOrEmpty(endpointFormat))
            {
                return new Tuple<bool, List<IssueEntity>>(false, []);
            }

            var url = string.Format(endpointFormat, name, id);
            string cacheKey = $"issues_{name}_{id}";

            return await GetCachedAsync(cacheKey, async () =>
           {
               var message = await HttpClientHelper.GetIssuesAsync(url);
               return new Tuple<bool, List<IssueEntity>>(message.Status == Status.Success && message.Data != null, message.Data ?? []);
           });
        }

        public async Task<Tuple<bool, string>> GetLatestMessage(string systemName)
        {
            var endpointFormat = _configuration.GetConnectionString("MessagesEndpoint");
            if (string.IsNullOrEmpty(endpointFormat))
            {
                return new Tuple<bool, string>(false, "No messages available for this system.");
            }

            var baseUrl = string.Format(endpointFormat, systemName);
            var url = string.Concat(baseUrl, "?order=1&offset=0&count=1");
            string cacheKey = $"latest_message_{systemName}";

            return await GetCachedAsync(cacheKey, async () =>
           {
               var apiMessage = await HttpClientHelper.GetMessagesAsync(url);
               var latestMessage = apiMessage.Data != null && apiMessage.Data.Count > 0
                   ? apiMessage.Data.First().Message
                   : "No messages available for this system.";
               return new Tuple<bool, string>(apiMessage.Status == Status.Success && apiMessage.Data != null && apiMessage.Data.Count > 0, latestMessage);
           });
        }

        public async Task<Tuple<bool, IList<string>>> GetAllSystems()
        {
            var baseUrl = _configuration.GetConnectionString("SystemsEndpoint");
            if (string.IsNullOrEmpty(baseUrl))
            {
                return new Tuple<bool, IList<string>>(false, []);
            }

            string cacheKey = $"systems_list_{baseUrl}";
            return await GetCachedAsync(cacheKey, async () =>
           {
               var apiMessage = await HttpClientHelper.GetSystems(baseUrl);
               return new Tuple<bool, IList<string>>(apiMessage.Status == Status.Success && apiMessage.Data != null, apiMessage.Data ?? []);
           });
        }

        public async Task<Tuple<bool, string>> GetSystemStateText(string systemName)
        {
            var endpointFormat = _configuration.GetConnectionString("SystemTextStateEndpoint");
            if (string.IsNullOrEmpty(endpointFormat))
            {
                return new Tuple<bool, string>(false, "Unknown");
            }

            var baseUrl = string.Format(endpointFormat, systemName);
            string cacheKey = $"system_state_{systemName}";

            return await GetCachedAsync(cacheKey, async () =>
           {
               var apiMessage = await HttpClientHelper.GetSystemStateText(baseUrl);
               return new Tuple<bool, string>(apiMessage.Status == Status.Success, apiMessage.Data ?? "Unknown");
           });
        }
    }
}
