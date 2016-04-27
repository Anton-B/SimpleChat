using System;

namespace ChatUtils
{
    public class OperationSuccessfulEventArgs : EventArgs
    {
        public string Message { get; }

        public OperationSuccessfulEventArgs(string message)
        {
            this.Message = message;
        }
    }
}
