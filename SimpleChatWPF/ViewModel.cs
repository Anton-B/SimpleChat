using System;
using System.Threading;
using System.Collections.ObjectModel;
using ChatUtils;
using ChatCore;

namespace ChatViewModel
{
    public class ViewModel
    {
        private Chat chat;
        private string userName;
        private bool isConnected;

        public ViewModel(SynchronizationContext context)
        {
            chat = new Chat(context);
            chat.ChatCreated += Chat_ChatCreated;
            chat.ErrorOccured += Chat_ErrorOccured;
            chat.SuccessJoining += Chat_SuccessJoining;
            chat.IncomingMessages.CollectionChanged += IncomingMessages_CollectionChanged;
            userName = chat.Name;
        }
        
        public string UserName
        {
            get { return userName; }
            set
            {
                if (chat.ChangeName(value))
                {
                    userName = chat.Name;
                    Messages.Add(new Message(null, "Имя успешно изменено.", MessageType.Success));
                }
                else
                    Messages.Add(new Message(null, "ВНИМАНИЕ! Имя слишком короткое, длина имени должна превышать один символ.", MessageType.Info));
            }
        }
        public bool IsConnected { get; private set; }
        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();

        private void Chat_ChatCreated(object sender, ChatCreatedEventArgs e)
        {
            Messages.Add(new Message(null, "Чат создан!", MessageType.Success));
            Messages.Add(new Message(null, string.Format("Ваш IP-адрес: {0}\nПорт: {1}\nОжидание собеседника...",
                e.IP, e.Port), MessageType.Info));
        }

        private void Chat_ErrorOccured(object sender, ErrorEventArgs e)
        {
            IsConnected = false;
            chat.Reboot();
            Messages.Add(new Message(null, e.Exception.Message, MessageType.Error));
        }

        private void Chat_SuccessJoining(object sender, EventArgs e)
        {
            if (chat.IsChatOwner)
                Messages.Add(new Message(null, string.Format("К вам подключился {0}.\n\tIP-адрес собеседника: {1}\n\tПорт: {2}\nНачните общение:",
                    chat.InterlocutorName, chat.InterlocutorIP, chat.InterlocutorPort), MessageType.Info));
            else
                Messages.Add(new Message(null, string.Format("Вы успешно подключились к собеседнику по имени {0}.\nНачните общение: ",
                    chat.InterlocutorName), MessageType.Info));
            IsConnected = true;
        }

        private void IncomingMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (chat.IncomingMessages == null || e.NewStartingIndex < 0)
                return;
            var newMessage = chat.IncomingMessages[e.NewStartingIndex];
            Messages.Add(new Message(chat.InterlocutorName, newMessage, MessageType.Input));
        }

        public void CreateChat()
        {
            chat.CreateChat();
        }

        public void JoinChat(string ipAddress)
        {
            chat.JoinChat(ipAddress);
        }

        public void SendMessage(string message)
        {
            chat.SendMessage(message);
        }
    }
}
