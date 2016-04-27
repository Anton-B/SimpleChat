using System;

namespace ChatUtils
{
    public class ErrorEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ErrorEventArgs(Exception exception)
        {
            this.Exception = exception;
        }
    }
}
