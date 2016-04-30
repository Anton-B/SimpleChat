using System;
using System.Threading;
using System.Collections.ObjectModel;
using ChatUtils;

namespace ChatCore
{
    public class Chat
    {
        private SynchronizationContext syncContext;
        private Server server;
        private Client client;

        public Chat(SynchronizationContext context)
        {
            this.syncContext = context;
        }

        public string Name { get; private set; } = "Незнакомец";
        public bool IsChatOwner { get; private set; }
        public string InterlocutorName { get; internal set; }
        public string InterlocutorIP { get; internal set; }
        public string InterlocutorPort { get; internal set; }
        public ObservableCollection<string> IncomingMessages { get; internal set; } = new ObservableCollection<string>();

        public event EventHandler<ErrorEventArgs> ErrorOccured;
        public event EventHandler<ChatCreatedEventArgs> ChatCreated;
        public event EventHandler SuccessJoining;

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
                server = new Server(this, syncContext);
                client = server.Client;
                IsChatOwner = true;
                OnChatCreated(server.IP, server.Port);
                server.StartAsync(Name);
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception(string.Format("Ошибка создания чата: {0}", ex.Message), ex));
            }
        }

        public void JoinChat(string IPAddress)
        {
            try
            {
                client = new Client(this, syncContext, Name);
                IsChatOwner = false;
                client.ConnectAsync(IPAddress);
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception(string.Format("Ошибка подключения к чату: {0}", ex.Message), ex));
            }
        }

        public void SendMessage(string message)
        {
            client.SendMessage(message);
        }

        public void Reboot()
        {
            server?.Dispose();
            IncomingMessages.Clear();
            server = null;
            client = null;
        }

        internal void OnSuccessJoining()
        {
            SuccessJoining?.Invoke(this, EventArgs.Empty);
        }

        internal void OnErrorOccured(Exception exception)
        {
            ErrorOccured?.Invoke(this, new ErrorEventArgs(exception));
        }

        private void OnChatCreated(string ip, string port)
        {
            ChatCreated?.Invoke(this, new ChatCreatedEventArgs(ip, port));
        }
    }
}
