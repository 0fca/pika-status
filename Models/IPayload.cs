using System.Collections.Generic;

namespace PikaStatus.Models
{
    public interface IPayload<T>
    {
        void AddMessage(string message);
        string GetLastAddedMessage();
        Stack<string> GetMessages();
    }
}