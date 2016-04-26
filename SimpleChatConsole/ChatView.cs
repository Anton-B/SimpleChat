using System;
using ChatUtils;

namespace SimpleChatConsole
{
    class ChatView
    {
        private ChatState chatState = ChatState.None;
        public string UserName { get; set; }
        public event EventHandler<NewNameEventArgs> NewName;
        public event EventHandler CreatingChat;
        public event EventHandler<IPAddressEventArgs> JoiningChat;
        public event EventHandler<NewMessageEventArgs> NewMessage;

        public ChatView()
        {
            new Presenter(this);
            ShowMenu();
            ReadInput();
        }

        private void ShowMenu()
        {
            ShowNewMessage(null, "Что вы хотите сделать? Введите номер пункта:\n\t"
                                    + "1 - Сменить имя.\n\t"
                                    + "2 - Создать чат.\n\t"
                                    + "3 - Присоединиться к созданному чату.\n\t"
                                    + "4 - Выйти.\n", MessageType.Info);
            chatState = ChatState.Menu;
        }

        private void ReadInput()
        {
            while (true)
            {
                if (chatState == ChatState.None)
                    continue;
                var inputText = Console.ReadLine();
                if (inputText.Equals(string.Empty) || chatState == ChatState.None)
                    continue;
                ShowNewMessage(UserName, inputText, MessageType.Output);
                switch (chatState)
                {
                    case ChatState.SendMessage:
                        SendMessage(inputText);
                        break;
                    case ChatState.Menu:
                        ChooseMenuItem(inputText);
                        break;
                }
            }
        }

        private void ChooseMenuItem(string answer)
        {
            switch (answer)
            {
                case "1":
                    ShowNewMessage(null, "Введите своё новое имя:", MessageType.Info);
                    chatState = ChatState.ChangeName;
                    ChangeName(Console.ReadLine());
                    ShowMenu();
                    break;
                case "2":
                    CreateChat();
                    break;
                case "3":
                    ShowNewMessage(null, "Введите IP-адрес чата, к которому хотите присоединиться:\n", MessageType.Info);
                    chatState = ChatState.JoinChat;
                    JoinChat(Console.ReadLine());
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    ShowNewMessage(null, "Введен некорректный вариант.\n", MessageType.Error);
                    break;
            }
        }

        private void ChangeName(string name)
        {
            NewName?.Invoke(this, new NewNameEventArgs(name));
        }

        private void CreateChat()
        {
            CreatingChat?.Invoke(this, EventArgs.Empty);
        }

        private void JoinChat(string ipAddress)
        {
            JoiningChat?.Invoke(this, new IPAddressEventArgs(ipAddress));
        }

        private void SendMessage(string message)
        {
            NewMessage?.Invoke(this, new NewMessageEventArgs(UserName, message, MessageType.Output));
        }

        public void HandleJoining()
        {
            chatState = ChatState.SendMessage;
        }

        public void ShowNewMessage(string userName, string message, MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Info:
                    Console.WriteLine(message);
                    break;
                case MessageType.Success:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case MessageType.Error:
                    chatState = ChatState.None;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(message);
                    Console.ForegroundColor = ConsoleColor.Gray;
                    ShowMenu();
                    break;
                case MessageType.Output:
                    Console.CursorTop -= 1;
                    Console.CursorLeft = 0;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(string.Format("{0}: {1}", userName, message));
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.CursorLeft = 0;
                    Console.CursorTop += 1;
                    break;
                case MessageType.Input:
                    var currCursorLeft = Console.CursorLeft;
                    Console.MoveBufferArea(0, Console.CursorTop, Console.CursorLeft + 1, Console.CursorTop + 1, 0, Console.CursorTop + 1);
                    Console.CursorLeft = 0;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Format("\t\t{0}: {1}", userName, message));
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.CursorLeft = currCursorLeft;
                    break;
                default:
                    throw new Exception("Ошибка вывода сообщения.");
            }
        }

        static void Main(string[] args)
        {
            Console.SetWindowSize(80, 60);
            Console.Title = "Чат";
            var chatView = new ChatView();
        }
    }
}
