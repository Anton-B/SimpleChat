using System;
using System.Threading;
using System.ComponentModel;
using System.Collections.ObjectModel;
using ChatUtils;
using ChatCore;

namespace ChatViewModel
{
    public class ViewModel : INotifyPropertyChanged
    {
        public ViewModel(SynchronizationContext context)
        {
            Chat = new Chat(context);
            Chat.ChatCreated += Chat_ChatCreated;
            Chat.ErrorOccured += Chat_ErrorOccured;
            Chat.SuccessJoining += Chat_SuccessJoining;
            Chat.IncomingMessages.CollectionChanged += IncomingMessages_CollectionChanged;
            UserName = Chat.Name;
        }

        public Chat Chat { get; }
        public bool IsConnected { get; private set; }
        public string UserName { get; private set; }
        public ObservableCollection<Message> Messages { get; private set; } = new ObservableCollection<Message>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Chat_ChatCreated(object sender, ChatCreatedEventArgs e)
        {
            Messages.Add(new Message(null, "Чат создан!", MessageType.Success));
            Messages.Add(new Message(null, string.Format("Ваш IP-адрес: {0}\nПорт: {1}\nОжидание собеседника...",
                e.IP, e.Port), MessageType.Info));
        }

        private void Chat_ErrorOccured(object sender, ErrorEventArgs e)
        {
            IsConnected = false;
            Chat.Reboot();
            Messages.Add(new Message(null, e.Exception.Message, MessageType.Error));
        }

        private void Chat_SuccessJoining(object sender, EventArgs e)
        {
            if (Chat.IsChatOwner)
                Messages.Add(new Message(null, string.Format("К вам подключился {0}.\n\tIP-адрес собеседника: {1}\n\tПорт: {2}\nНачните общение:",
                    Chat.InterlocutorName, Chat.InterlocutorIP, Chat.InterlocutorPort), MessageType.Info));
            else
                Messages.Add(new Message(null, string.Format("Вы успешно подключились к собеседнику по имени {0}.\nНачните общение: ",
                    Chat.InterlocutorName), MessageType.Info));
            IsConnected = true;
            OnPropertyChanged("IsConnected");
        }

        private void IncomingMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Chat.IncomingMessages == null || e.NewStartingIndex < 0)
                return;
            var newMessage = Chat.IncomingMessages[e.NewStartingIndex];
            Messages.Add(new Message(Chat.InterlocutorName, newMessage, MessageType.Input));
        }

        public void ChangeName(string newName)
        {
            if (Chat.ChangeName(newName))
            {
                UserName = Chat.Name;
                Messages.Add(new Message(null, "Имя успешно изменено.", MessageType.Success));
            }
            else
                Messages.Add(new Message(null, "ВНИМАНИЕ! Имя слишком короткое, длина имени должна превышать один символ.", MessageType.Info));
        }

        public void CreateChat()
        {
            Chat.CreateChat();
        }

        public void JoinChat(string ipAddress)
        {
            Chat.JoinChat(ipAddress);
        }

        public void SendMessage(string message)
        {
            Chat.SendMessage(message);
        }
    }
}
