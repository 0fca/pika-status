using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Pika.Domain.Status.Data;
using Pika.Domain.Status.Models;
using PikaStatus.Services.Helpers;

namespace PikaStatus.Services
{
    public class MessageService
    {
        private readonly IConfiguration _configuration;
        public MessageService(IConfiguration configuration)
        {
            _configuration = configuration;
            HttpClientHelper.ConfigureClient(configuration.GetConnectionString("StatusApiBase")
                ?? throw new InvalidOperationException("Connection string 'StatusApiBase' is not configured."),
                MediaTypeNames.Application.Json);
        }

        public async Task<Tuple<string, bool>> GetOverallStatus()
        {
            var endpoint = _configuration.GetConnectionString("OverallStatusEndpoint");
            if (string.IsNullOrEmpty(endpoint))
            {
                return new Tuple<string, bool>("Cloud status is temporarily unavailable.", false);
            }

            var statusMessage = await HttpClientHelper
                .GetSingleMessageAsync(endpoint);
            var message = statusMessage.Messages.Count > 0
                ? statusMessage.Messages.Pop()
                : "Cloud status is temporarily unavailable.";
            return new Tuple<string, bool>(message, statusMessage.Status);
        }
        
        public async Task<Tuple<Stack<string>, bool>> GetOverallStatusDetailed()
        {
            var endpoint = _configuration.GetConnectionString("OverallStatusEndpoint");
            if (string.IsNullOrEmpty(endpoint))
            {
                return new Tuple<Stack<string>, bool>(new Stack<string>(["Cloud status is temporarily unavailable."]), false);
            }

            var statusMessage = await HttpClientHelper
                .GetSingleMessageAsync(endpoint);
            return new Tuple<Stack<string>, bool>(statusMessage.Messages, statusMessage.Status);
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
            var message = await HttpClientHelper
                .GetMessagesAsync(url);
            var items = message.Data ?? [];
            return new Tuple<bool, List<MessageEntity>>(message.Status, items);
        }
        
        public async Task<Tuple<bool, List<IssueEntity>>> GetIssues(string name, int id)
        {
            var endpointFormat = _configuration.GetConnectionString("IssuesEndpoint");
            if (string.IsNullOrEmpty(endpointFormat))
            {
                return new Tuple<bool, List<IssueEntity>>(false, []);
            }

            var url = string.Format(endpointFormat, name, id);
            var message = await HttpClientHelper
                .GetIssuesAsync(url);
            return new Tuple<bool, List<IssueEntity>>(message.Status && message.Data != null, message.Data ?? []);
        }

        public async Task<Tuple<bool, string>> GetLatestMessage(string systemName)
        {
            var endpointFormat = _configuration.GetConnectionString("MessagesEndpoint");
            if (string.IsNullOrEmpty(endpointFormat))
            {
                return new Tuple<bool, string>(false, "No messages available for this system.");
            }

            var baseUrl = string.Format(endpointFormat, systemName);
            var apiMessage = await HttpClientHelper.GetMessagesAsync(string
                .Concat(baseUrl, "?order=1&offset=0&count=1"));
            var latestMessage = apiMessage.Data != null && apiMessage.Data.Count > 0
                ? apiMessage.Data.First().Message
                : "No messages available for this system.";
            return new Tuple<bool, string>(apiMessage.Status && apiMessage.Data != null && apiMessage.Data.Count > 0, latestMessage);
        }

        public async Task<Tuple<bool, IList<string>>> GetAllSystems()
        {
            var baseUrl =_configuration.GetConnectionString("SystemsEndpoint");
            if (string.IsNullOrEmpty(baseUrl))
            {
                return new Tuple<bool, IList<string>>(false, []);
            }
            var apiMessage = await HttpClientHelper.GetSystems(baseUrl);
            return new Tuple<bool, IList<string>>(apiMessage.Status && apiMessage.Data != null, apiMessage.Data ?? []);
        }

        public async Task<Tuple<bool, string>> GetSystemStateText(string systemName)
        {
            var endpointFormat = _configuration.GetConnectionString("SystemTextStateEndpoint");
            if (string.IsNullOrEmpty(endpointFormat))
            {
                return new Tuple<bool, string>(false, "Unknown");
            }

            var baseUrl = string.Format(endpointFormat, systemName);
            var apiMessage = await HttpClientHelper.GetSystemStateText(baseUrl);

            return new Tuple<bool, string>(apiMessage.Status, apiMessage.Data ?? "Unknown");
        }
    }
}