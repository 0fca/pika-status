using System;
using System.Collections.Generic;
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
            var url = _configuration.GetConnectionString($"Messages{systemName}");
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
            var baseUrl = _configuration.GetConnectionString($"Messages{systemName}");
            if (string.IsNullOrEmpty(baseUrl))
            {
                return new Tuple<bool, string>(false, null);
            }
            var apiMessage = await HttpClientHelper.GetMessagesAsync(string
                .Concat(baseUrl, "?order=1&offset=0&count=1"));
            return new Tuple<bool, string>(apiMessage.Status, apiMessage.Data[0].Message);
        }
    }
}