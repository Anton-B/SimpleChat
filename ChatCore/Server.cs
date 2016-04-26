using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatUtils;

namespace ChatCore
{
    class Server : IDisposable
    {
        public Client Client { get { return client; } }
        public string IP { get { return ip.ToString(); } }
        public string Port { get { return port.ToString(); } }
        public event EventHandler<NewMessageEventArgs> NewMessage;
        private SynchronizationContext context;
        private TcpListener listener;
        private Client client;
        private IPAddress ip;
        private int port;

        public Server(SynchronizationContext context)
        {
            listener = null;
            this.context = context;
            client = new Client(context);
            if ((ip = GetLocalIPAddress()) == null)
                throw new Exception("Локальный IP не найден.");
            port = 81;
            listener = new TcpListener(ip, port);
        }

        public void Start(string userName)
        {
            try
            {
                listener.Start();
                client.Name = userName;
                while (true)
                    client.StartConnection(listener.AcceptTcpClient());
            }
            catch (Exception ex)
            {
                ShowNewMessage(null, "Ошибка создания чата.", MessageType.Error);
                listener.Stop();
            }
        }

        private void ShowNewMessage(string userName, string message, MessageType messageType)
        {
            context.Post(o => NewMessage?.Invoke(this, new NewMessageEventArgs(userName, message, messageType)), null);
        }

        private IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                    return ip;
            return null;
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
