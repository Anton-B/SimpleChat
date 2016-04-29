using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatUtils;
using System.Threading.Tasks;

namespace ChatCore
{
    public class Server : IDisposable
    {
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

        internal Client Client { get { return client; } }
        internal string IP { get { return ip.ToString(); } }
        internal string Port { get { return port.ToString(); } }

        public event EventHandler<ErrorEventArgs> ErrorOccured;

        internal async void StartAsync(string userName)
        {
            await Task.Run(() => Start(userName));
        }

        private void Start(string userName)
        {
            try
            {
                listener.Start();
                client.Name = userName;
                client.StartConnection(listener.AcceptTcpClient());
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception("Ошибка чата.", ex));
                listener.Stop();
            }
        }

        private void OnErrorOccured(Exception exception)
        {
            context.Post(o => ErrorOccured?.Invoke(this, new ErrorEventArgs(exception)), null);
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
