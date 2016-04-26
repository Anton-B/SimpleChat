using System;
using System.Threading;
using ChatUtils;
using ChatCore;

namespace SimpleChatConsole
{
    class Presenter
    {
        private ChatView view;
        private Chat model;

        public Presenter(ChatView chatView)
        {
            this.view = chatView;
            this.view.NewName += View_NewName;
            this.view.CreatingChat += View_CreatingChat;
            this.view.JoiningChat += View_JoiningChat;
            this.view.NewMessage += View_NewMessage;
            this.model = new Chat(new SynchronizationContext());
            this.model.NewMessage += Model_NewMessage;
            this.model.SuccessJoinig += Model_SuccessJoinig;
            view.UserName = model.Name;
        }

        private void Model_NewMessage(object sender, NewMessageEventArgs e)
        {
            view.ShowNewMessage(e.UserName, e.Message, e.Type);
            if (e.Type == MessageType.Error)
                model.Reboot();
        }

        private void Model_SuccessJoinig(object sender, EventArgs e)
        {
            view.HandleJoining();
        }

        private void View_NewName(object sender, NewNameEventArgs e)
        {
            model.ChangeName(e.NewName);
            view.UserName = model.Name;
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
    }
}
