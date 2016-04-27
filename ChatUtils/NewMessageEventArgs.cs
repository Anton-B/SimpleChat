using System;

namespace ChatUtils
{
    public class NewMessageEventArgs : EventArgs
    {
        public Message Message { get; }

        public NewMessageEventArgs(Message message)
        {
            this.Message = message;
        }
    }
}
