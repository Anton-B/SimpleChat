using System;
using System.Threading;
using ChatUtils;

namespace ChatCore
{
    public interface IView
    {
        string UserName { get; set; }
        SynchronizationContext Context { get; }

        event EventHandler<NewNameEventArgs> NewName;
        event EventHandler CreatingChat;
        event EventHandler<IPAddressEventArgs> JoiningChat;
        event EventHandler<NewMessageEventArgs> NewMessage;

        void HandleJoining();
        void ShowNewMessage(string userName, string message, MessageType messageType);
    }
}
