using System;

namespace ChatUtils
{
    public class ChatCreatedEventArgs : EventArgs
    {
        public ChatCreatedEventArgs(string ip, string port)
        {
            this.IP = ip;
            this.Port = port;
        }

        public string IP { get; }
        public string Port { get; }
    }
}
