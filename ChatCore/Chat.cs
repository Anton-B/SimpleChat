using System;
using System.Threading;
using System.Threading.Tasks;
using ChatUtils;

namespace ChatCore
{
    public class Chat
    {
        private SynchronizationContext syncContext;

        public Chat(SynchronizationContext context)
        {
            this.syncContext = context;
        }

        public string Name { get; private set; } = "Незнакомец";
        public Server Server { get; private set; }
        public Client Client { get; private set; }

        public event EventHandler<ErrorEventArgs> ErrorOccured;
        public event EventHandler<ChatCreatedEventArgs> ChatCreated;
        public event EventHandler ConnectionStarted;

        public bool ChangeName(string newName)
        {
            if (newName.Length < 2)
                return false;
            this.Name = newName;
            return true;
        }

        public void CreateChat()
        {
            try
            {
                Server = new Server(syncContext);
                Client = Server.Client;
                OnChatCreated(Server.IP, Server.Port);
                Server.StartAsync(Name);
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception(string.Format("Ошибка создания чата: {0}", ex.Message), ex));
            }
        }

        public async void JoinChat(string IPAddress)
        {
            try
            {
                Client = new Client(syncContext, Name);
                OnConnectionStarted();
                await Task.Run(() => Client.ConnectAsync(IPAddress));
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception(string.Format("Ошибка подключения к чату: {0}", ex.Message), ex));
            }
        }

        public void SendMessage(string message)
        {
            Client.SendMessage(message);
        }

        public void Reboot()
        {
            Server?.Dispose();
            Client?.IncomingMessagesCollection.Clear();
            Server = null;
            Client = null;
        }

        private void OnChatCreated(string ip, string port)
        {
            ChatCreated?.Invoke(this, new ChatCreatedEventArgs(ip, port));
        }

        private void OnConnectionStarted()
        {
            ConnectionStarted?.Invoke(this, EventArgs.Empty);
        }
        private void OnErrorOccured(Exception exception)
        {
            ErrorOccured?.Invoke(this, new ErrorEventArgs(exception));
        }
    }
}
