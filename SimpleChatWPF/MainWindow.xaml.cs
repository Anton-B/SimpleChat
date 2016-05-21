using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using ChatUtils;
using ChatViewModel;

namespace SimpleChatWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel viewModel;
        private ChatState chatState = ChatState.None;
        private const string isConnectedPropertyName = "IsConnected";

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModel(SynchronizationContext.Current);
            viewModel.Messages.CollectionChanged += Messages_CollectionChanged;
            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            inputTextBox.Focus();
        }

        private void ChangeName(string name)
        {
            viewModel.ChangeName(name);
            chatState = ChatState.None;
            buttonsGrid.IsEnabled = true;
        }

        private void CreateChat()
        {
            viewModel.CreateChat();
        }

        private void JoinChat(string ipAddress)
        {
            viewModel.Messages.Add(new Message(null, "Выполняется подключение...", MessageType.Info));
            viewModel.JoinChat(ipAddress);
        }

        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (viewModel.Messages == null || e.NewStartingIndex < 0)
                return;
            var newMessage = viewModel.Messages[e.NewStartingIndex];
            ShowNewMessage(newMessage.OwnerName, newMessage.Content, newMessage.MessageType);
        }

        private void inputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            var inputText = inputTextBox.Text;
            if (inputText.Equals(string.Empty) || chatState == ChatState.None)
                return;
            inputTextBox.Clear();
            viewModel.Messages.Add(new Message(viewModel.UserName, inputText, MessageType.Output));
            switch (chatState)
            {
                case ChatState.ChangeName:
                    ChangeName(inputText);
                    break;
                case ChatState.JoinChat:
                    JoinChat(inputText);
                    break;
                case ChatState.SendMessage:
                    viewModel.SendMessage(inputText);
                    break;
            }
        }

        private void changeNameButton_Click(object sender, RoutedEventArgs e)
        {
            buttonsGrid.IsEnabled = false;
            viewModel.Messages.Add(new Message(null, "Введите своё новое имя:", MessageType.Info));
            chatState = ChatState.ChangeName;
        }

        private void createChatButton_Click(object sender, RoutedEventArgs e)
        {
            buttonsGrid.IsEnabled = false;
            viewModel.Messages.Add(new Message(null, "Создание чата...", MessageType.Info));
            CreateChat();
        }

        private void joinChatButton_Click(object sender, RoutedEventArgs e)
        {
            buttonsGrid.IsEnabled = false;
            viewModel.Messages.Add(new Message(null, "Введите IP-адрес чата, к которому хотите присоединиться:", MessageType.Info));
            chatState = ChatState.JoinChat;
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void HandleJoining()
        {
            chatState = ChatState.SendMessage;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == isConnectedPropertyName)
                HandleJoining();
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
    }
}
