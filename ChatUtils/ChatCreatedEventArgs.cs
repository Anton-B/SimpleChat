using System;

namespace ChatUtils
{
    public class ChatCreatedEventArgs : EventArgs
    {
        public string Message { get; }

        public ChatCreatedEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
