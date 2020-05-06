using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PikaStatus.Models;
using PikaStatus.Services.Helpers;

namespace PikaStatus.Services
{
    public class MessageService
    {
        private readonly IConfiguration _configuration;
        public MessageService(IConfiguration configuration)
        {
            _configuration = configuration;
            HttpClientHelper.ConfigureClient(configuration.GetConnectionString("StatusApiBase"), 
                MediaTypeNames.Application.Json);
        }

        public async Task<Tuple<string, bool>> GetOverallStatus()
        {
            var statusMessage = await HttpClientHelper
                .GetSingleMessageAsync(_configuration.GetConnectionString("OverallStatusEndpoint"));
            return new Tuple<string, bool>(statusMessage.Messages.Pop(), statusMessage.Status);
        }
        
        public async Task<Tuple<Stack<string>, bool>> GetOverallStatusDetailed()
        {
            var statusMessage = await HttpClientHelper
                .GetSingleMessageAsync(_configuration.GetConnectionString("OverallStatusEndpoint"));
            return new Tuple<Stack<string>, bool>(statusMessage.Messages, statusMessage.Status);
        }

        public async Task<Tuple<bool, List<MessageEntity>>> GetMessages(string systemName)
        {
            var url = string.Format(_configuration.GetConnectionString($"MessagesEndpoint"), systemName);
            if (string.IsNullOrEmpty(url))
            {
                return new Tuple<bool, List<MessageEntity>>(false, null);
            }
            var message = await HttpClientHelper
                .GetMessagesAsync(url);
            return new Tuple<bool, List<MessageEntity>>(message.Status, message.Data);
        }

        public async Task<Tuple<bool, string>> GetLatestMessage(string systemName)
        {
            var baseUrl = string.Format(_configuration.GetConnectionString($"MessagesEndpoint"), systemName);
            if (string.IsNullOrEmpty(baseUrl))
            {
                return new Tuple<bool, string>(false, null);
            }
            var apiMessage = await HttpClientHelper.GetMessagesAsync(string
                .Concat(baseUrl, "?order=1&offset=0&count=1"));
            return new Tuple<bool, string>(apiMessage.Status, apiMessage.Data.Last().Message);
        }

        public async Task<Tuple<bool, IList<string>>> GetAllSystems()
        {
            var baseUrl =_configuration.GetConnectionString($"SystemsEndpoint");
            if (string.IsNullOrEmpty(baseUrl))
            {
                return new Tuple<bool, IList<string>>(false, new List<string>());
            }
            var apiMessage = await HttpClientHelper.GetSystems(baseUrl);
            return new Tuple<bool, IList<string>>(apiMessage.Status, apiMessage.Data);
        }

        public async Task<Tuple<bool, string>> GetSystemStateText(string systemName)
        {
            var baseUrl = string.Format(_configuration.GetConnectionString($"SystemTextStateEndpoint"), systemName);
            if (string.IsNullOrEmpty(baseUrl))
            {
                return new Tuple<bool, string>(false, "Unknown");
            }
            var apiMessage = await HttpClientHelper.GetSystemStateText(baseUrl);
            return new Tuple<bool, string>(apiMessage.Status, apiMessage.Data);
        }
    }
}