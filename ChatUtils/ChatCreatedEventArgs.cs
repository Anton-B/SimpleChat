using System;

namespace ChatUtils
{
    public class ChatCreatedEventArgs : EventArgs
    {
        public string IP { get; }
        public string Port { get; }

        public ChatCreatedEventArgs(string ip, string port)
        {
            this.IP = ip;
            this.Port = port;
        }
    }
}
