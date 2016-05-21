using System;

namespace ChatUtils
{
    public class Message : EventArgs
    {
        public Message(string ownerName, string content, MessageType messageType)
        {
            this.OwnerName = ownerName;
            this.Content = content;
            this.MessageType = messageType;
        }

        public string OwnerName { get; }
        public string Content { get; }
        public MessageType MessageType { get; }
    }
}
