using System;
using System.Threading;
using System.Threading.Tasks;
using ChatUtils;

namespace ChatCore
{
    public class Chat
    {
        public event EventHandler<ErrorEventArgs> ErrorOccured;
        public event EventHandler<ChatCreatedEventArgs> ChatCreated;
        public event EventHandler ConnectionStarted;
        public event EventHandler<OperationSuccessfulEventArgs> OperationSuccessful;
        public string Name { get; private set; } = "Незнакомец";
        public Server Server { get; private set; }
        public Client Client { get; private set; }
        private SynchronizationContext syncContext;

        public Chat(SynchronizationContext context)
        {
            this.syncContext = context;
        }

        private void OnChatCreated(string info)
        {
            ChatCreated?.Invoke(this, new ChatCreatedEventArgs(info));
        }

        private void OnConnectionStarted()
        {
            ConnectionStarted?.Invoke(this, EventArgs.Empty);
        }

        private void OnOperationSuccessful(string message)
        {
            OperationSuccessful?.Invoke(this, new OperationSuccessfulEventArgs(message));
        }

        private void OnErrorOccured(Exception exception)
        {
            ErrorOccured?.Invoke(this, new ErrorEventArgs(exception));
        }

        public void ChangeName(string newName)
        {
            if (newName.Length < 2)
            {
                OnErrorOccured(new Exception("Ошибка изменения имени: Имя слишком короткое."));
                return;
            }
            this.Name = newName;
            OnOperationSuccessful("Имя успешно изменено.");
        }

        public async void CreateChat()
        {
            try
            {
                Server = new Server(syncContext);
                Client = Server.Client;
                OnOperationSuccessful("Чат создан!");
                OnChatCreated(string.Format("Ваш IP-адрес: {0}\nПорт: {1}\nОжидание собеседника...",
                    Server.IP, Server.Port));
                await Task.Run(() => Server.Start(Name));
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception(string.Format("Ошибка создания чата: {0}", ex.Message)));
            }
        }

        public async void JoinChat(string IPAddress)
        {
            try
            {
                Client = new Client(syncContext, Name);
                OnConnectionStarted();
                await Task.Run(() => Client.Connect(IPAddress));
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception(string.Format("Ошибка подключения к чату: {0}", ex.Message)));
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
    }
}
