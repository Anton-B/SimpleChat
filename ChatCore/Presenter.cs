using System;
using ChatUtils;

namespace ChatCore
{
    public class Presenter
    {
        private IView view;
        private Chat model;

        public Presenter(IView view)
        {
            this.view = view;
            this.view.NewName += View_NewName;
            this.view.CreatingChat += View_CreatingChat;
            this.view.JoiningChat += View_JoiningChat;
            this.view.NewMessage += View_NewMessage;
            this.model = new Chat(this.view.Context);
            this.model.ChatCreated += Model_ChatCreated;
            this.model.ConnectionStarted += Model_ConnectionStarted;
            this.model.ErrorOccured += Model_ErrorOccured;
            this.view.UserName = model.Name;
        }

        private void SubscribeToServerEvents()
        {
            model.Server.ErrorOccured += Model_ErrorOccured;
        }

        private void SubscribeToClientEvents()
        {
            model.Client.SuccessJoining += Client_SuccessJoining;
            model.Client.IncomingMessagesCollection.CollectionChanged += IncomingMessagesCollection_CollectionChanged;
            model.Client.ErrorOccured += Model_ErrorOccured;
        }

        private void View_NewName(object sender, NewNameEventArgs e)
        {
            if (model.ChangeName(e.NewName))
            {
                view.UserName = model.Name;
                view.ShowNewMessage(null, "Имя успешно изменено.", MessageType.Success);
            }
            else
                view.ShowNewMessage(null, "ВНИМАНИЕ! Имя слишком короткое, длина имени должна превышать один символ.", MessageType.Info);
        }

        private void View_CreatingChat(object sender, EventArgs e)
        {
            model.CreateChat();
        }

        private void View_JoiningChat(object sender, IPAddressEventArgs e)
        {
            model.JoinChat(e.IPAddress);
        }

        private void View_NewMessage(object sender, NewMessageEventArgs e)
        {
            model.SendMessage(e.Message);
        }

        private void Model_ChatCreated(object sender, ChatCreatedEventArgs e)
        {
            SubscribeToServerEvents();
            SubscribeToClientEvents();
            view.ShowNewMessage(null, "Чат создан!", MessageType.Success);
            view.ShowNewMessage(null, string.Format("Ваш IP-адрес: {0}\nПорт: {1}\nОжидание собеседника...",
                e.IP, e.Port), MessageType.Info);
        }

        private void Model_ConnectionStarted(object sender, EventArgs e)
        {
            SubscribeToClientEvents();
        }

        private void Model_ErrorOccured(object sender, ErrorEventArgs e)
        {
            model.Reboot();
            view.ShowNewMessage(null, e.Exception.Message, MessageType.Error);
        }

        private void Client_SuccessJoining(object sender, SuccessJoiningEventArgs e)
        {
            view.ShowNewMessage(null, e.Message, MessageType.Info);
            view.HandleJoining();
        }

        private void IncomingMessagesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (model.Client.IncomingMessagesCollection == null || model.Client.IncomingMessagesCollection.Count < 1)
                return;
            var newMessage = model.Client.IncomingMessagesCollection[model.Client.IncomingMessagesCollection.Count - 1];
            view.ShowNewMessage(newMessage.UserName, newMessage.MessageContent, newMessage.Type);
        }
    }
}
