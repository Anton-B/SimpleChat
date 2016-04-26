using System;
using System.Threading;
using System.Threading.Tasks;
using ChatUtils;

namespace ChatCore
{
    public class Chat
    {
        public event EventHandler<NewMessageEventArgs> NewMessage;
        public event EventHandler SuccessJoinig;
        public string Name { get; private set; } = "Незнакомец";
        private Server server;
        private Client client;
        private SynchronizationContext syncContext;

        public Chat(SynchronizationContext context)
        {
            this.syncContext = context;
        }

        private void ShowNewMessage(string userName, string message, MessageType messageType)
        {
            NewMessage?.Invoke(this, new NewMessageEventArgs(userName, message, messageType));
        }

        public void ChangeName(string newName)
        {
            if (newName.Length < 2)
            {
                ShowNewMessage(null, "Ошибка изменения имени: Имя слишком короткое.", MessageType.Error);
                return;
            }
            this.Name = newName;
            ShowNewMessage(null, "Имя успешно изменено.", MessageType.Success);
        }

        public async void CreateChat()
        {
            ShowNewMessage(null, "Создание чата...", MessageType.Info);
            try
            {
                server = new Server(syncContext);
                server.NewMessage += ServerAndClient_NewMessage;
                client = server.Client;
                client.NewMessage += ServerAndClient_NewMessage;
                client.SuccessJoining += Client_InterlocutorJoined;
                ShowNewMessage(null, "Чат создан!", MessageType.Success);
                ShowNewMessage(null, string.Format("Ваш IP-адрес: {0}\nПорт: {1}\nОжидание собеседника...",
                    server.IP, server.Port), MessageType.Info);
                await Task.Run(() => server.Start(Name));
            }
            catch (Exception ex)
            {
                ShowNewMessage(null, string.Format("Ошибка создания чата: {0}", ex.Message), MessageType.Error);
            }
        }

        private void ServerAndClient_NewMessage(object sender, NewMessageEventArgs e)
        {
            ShowNewMessage(e.UserName, e.Message, e.Type);
        }

        private void Client_InterlocutorJoined(object sender, EventArgs e)
        {
            SuccessJoinig?.Invoke(sender, e);
        }

        public async void JoinChat(string IPAddress)
        {
            try
            {
                client = new Client(syncContext, Name);
                client.NewMessage += ServerAndClient_NewMessage;
                client.SuccessJoining += Client_InterlocutorJoined;
                await Task.Run(() => client.Connect(IPAddress));
            }
            catch (Exception ex)
            {
                ShowNewMessage(null, string.Format("Ошибка подключения к чату: {0}", ex.Message), MessageType.Error);
            }
        }

        public void SendMessage(string message)
        {
            client.SendMessage(message);
        }

        public void Reboot()
        {
            server?.Dispose();
            server = null;
            client = null;
        }
    }
}
