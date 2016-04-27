using System;

namespace ChatUtils
{
    public class SuccessJoiningEventArgs : EventArgs
    {
        public string Message { get; }

        public SuccessJoiningEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
