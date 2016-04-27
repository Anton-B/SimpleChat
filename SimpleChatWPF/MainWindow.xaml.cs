using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ChatUtils;

namespace SimpleChatWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatState chatState = ChatState.None;
        public string UserName { get; set; }
        public event EventHandler<NewNameEventArgs> NewName;
        public event EventHandler CreatingChat;
        public event EventHandler<IPAddressEventArgs> JoiningChat;
        public event EventHandler<NewMessageEventArgs> NewMessage;

        public MainWindow()
        {
            InitializeComponent();
            new Presenter(this);
            inputTextBox.Focus();
        }

        private void ChangeName(string name)
        {
            NewName?.Invoke(this, new NewNameEventArgs(name));
            chatState = ChatState.None;
            buttonsGrid.IsEnabled = true;
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
            NewMessage?.Invoke(this, new NewMessageEventArgs(new Message(UserName, message, MessageType.Output)));
        }

        public void HandleJoining()
        {
            chatState = ChatState.SendMessage;
        }

        public void ShowNewMessage(string userName, string message, MessageType messageType)
        {
            var msgBorder = new Border();
            msgBorder.HorizontalAlignment = HorizontalAlignment.Left;
            var msgStackPanel = new StackPanel();
            msgStackPanel.Orientation = Orientation.Vertical;
            TextBlock userNameTextBlock = null;
            if (userName != null && !userName.Equals(string.Empty))
            {
                userNameTextBlock = new TextBlock();
                userNameTextBlock.Margin = new Thickness(4, 4, 4, 0);
                userNameTextBlock.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF50687A");
                userNameTextBlock.FontWeight = FontWeights.Bold;
                userNameTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                userNameTextBlock.Text = userName;
                msgStackPanel.Children.Add(userNameTextBlock);
            }
            switch (messageType)
            {
                case MessageType.Info:
                    msgBorder.BorderBrush = (Brush)new BrushConverter().ConvertFromString("Gray");
                    break;
                case MessageType.Success:
                    msgBorder.BorderBrush = (Brush)new BrushConverter().ConvertFromString("DarkGreen");
                    break;
                case MessageType.Error:
                    msgBorder.BorderBrush = (Brush)new BrushConverter().ConvertFromString("DarkRed");
                    buttonsGrid.IsEnabled = true;
                    chatState = ChatState.None;
                    break;
                case MessageType.Output:
                    msgBorder.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF528EB8");
                    msgBorder.HorizontalAlignment = HorizontalAlignment.Right;
                    if (userNameTextBlock != null)
                        userNameTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    break;
                case MessageType.Input:
                    msgBorder.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF4D7FA4");
                    break;
                default:
                    throw new Exception("Ошибка вывода сообщения.");
            }
            msgBorder.BorderThickness = new Thickness(2);
            msgBorder.Margin = new Thickness(10, 3, 10, 3);
            msgBorder.MinHeight = 30;
            msgBorder.CornerRadius = new CornerRadius(2);
            var msgTextBlock = new TextBlock();
            msgTextBlock.Background = null;
            msgTextBlock.VerticalAlignment = VerticalAlignment.Center;
            msgTextBlock.Margin = new Thickness(4);
            msgTextBlock.MaxWidth = 400;
            msgTextBlock.TextWrapping = TextWrapping.Wrap;
            msgTextBlock.Text = message;
            msgStackPanel.Children.Add(msgTextBlock);
            msgBorder.Child = msgStackPanel;
            chatStackPanel.Children.Add(msgBorder);
            chatScrollViewer.ScrollToBottom();
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var inputText = inputTextBox.Text;
            if (inputText.Equals(string.Empty) || chatState == ChatState.None)
                return;
            inputTextBox.Clear();
            ShowNewMessage(UserName, inputText, MessageType.Output);
            switch (chatState)
            {
                case ChatState.ChangeName:
                    ChangeName(inputText);
                    break;
                case ChatState.JoinChat:
                    JoinChat(inputText);
                    break;
                case ChatState.SendMessage:
                    SendMessage(inputText);
                    break;
            }
        }

        private void changeNameButton_Click(object sender, RoutedEventArgs e)
        {
            buttonsGrid.IsEnabled = false;
            ShowNewMessage(null, "Введите своё новое имя:", MessageType.Info);
            chatState = ChatState.ChangeName;
        }

        private void createChatButton_Click(object sender, RoutedEventArgs e)
        {
            buttonsGrid.IsEnabled = false;
            ShowNewMessage(null, "Создание чата...", MessageType.Info);
            CreateChat();
        }

        private void joinChatButton_Click(object sender, RoutedEventArgs e)
        {
            buttonsGrid.IsEnabled = false;
            ShowNewMessage(null, "Введите IP-адрес чата, к которому хотите присоединиться:", MessageType.Info);
            chatState = ChatState.JoinChat;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
