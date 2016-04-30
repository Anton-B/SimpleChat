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
            this.model.ErrorOccured += Model_ErrorOccured;
            this.model.SuccessJoining += Model_SuccessJoining;
            this.model.IncomingMessages.CollectionChanged += IncomingMessages_CollectionChanged;
            this.view.UserName = model.Name;
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
            view.ShowNewMessage(null, "Чат создан!", MessageType.Success);
            view.ShowNewMessage(null, string.Format("Ваш IP-адрес: {0}\nПорт: {1}\nОжидание собеседника...",
                e.IP, e.Port), MessageType.Info);
        }

        private void Model_ErrorOccured(object sender, ErrorEventArgs e)
        {
            model.Reboot();
            view.ShowNewMessage(null, e.Exception.Message, MessageType.Error);
        }

        private void Model_SuccessJoining(object sender, EventArgs e)
        {
            if (model.IsChatOwner)
                view.ShowNewMessage(null, string.Format("К вам подключился {0}.\n\tIP-адрес собеседника: {1}\n\tПорт: {2}\nНачните общение:",
                    model.InterlocutorName, model.InterlocutorIP, model.InterlocutorPort), MessageType.Info);
            else
                view.ShowNewMessage(null, string.Format("Вы успешно подключились к собеседнику по имени {0}.\nНачните общение: ",
                    model.InterlocutorName), MessageType.Info);
            view.HandleJoining();
        }

        private void IncomingMessages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (model.IncomingMessages == null || e.NewStartingIndex < 0)
                return;
            var newMessage = model.IncomingMessages[e.NewStartingIndex];
            view.ShowNewMessage(model.InterlocutorName, newMessage, MessageType.Input);
        }
    }
}
