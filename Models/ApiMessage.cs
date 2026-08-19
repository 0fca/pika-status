using PikaStatus.Enums.Status;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PikaStatus.Models
{
    public class ApiMessage<T> : IPayload<T>
    {
        public T? Data { get; set; }
        public Stack<string> Messages { get; set; } = new Stack<string>(["There was a problem reading status from system mainframe API"]);
        
        [JsonIgnore]
        public Status Status { get; set; } = Status.Success;

        [JsonProperty("status")]
        private bool? _rawStatus;

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (_rawStatus.HasValue)
            {
                Status = _rawStatus.Value ? Status.Success : Status.Error;
            }
        }

        public void AddMessage(string message)
        {
            this.Messages.Push(message);
        }

        public string GetLastAddedMessage()
        {
            return this.Messages.Pop();
        }

        public Stack<string> GetMessages()
        {
            return this.Messages;
        }
    }
}
