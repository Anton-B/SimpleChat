using System;

namespace ChatUtils
{
    public class NewMessageEventArgs : EventArgs
    {
        public string UserName { get; }
        public string Message { get; }
        public MessageType Type {get;}

        public NewMessageEventArgs(string userName, string message, MessageType type)
        {
            this.UserName = userName;
            this.Message = message;
            this.Type = type;
        }
    }
}
