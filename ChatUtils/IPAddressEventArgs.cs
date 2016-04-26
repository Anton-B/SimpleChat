using System;

namespace ChatUtils
{
    public class IPAddressEventArgs : EventArgs
    {
        public string IPAddress { get; }

        public IPAddressEventArgs(string ipAddress)
        {
            this.IPAddress = ipAddress;
        }
    }
}
