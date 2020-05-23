using System;

namespace PikaStatus.Models
{
    public enum MessageType
    {
        Unknown = int.MaxValue,
        None = -1,
        Issue = 0,
        Info = 1
    }
}