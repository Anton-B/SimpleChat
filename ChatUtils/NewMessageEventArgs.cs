using System;

namespace ChatUtils
{
    public class NewMessageEventArgs : EventArgs
    {
        public string Message { get; }

        public NewMessageEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
