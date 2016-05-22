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

        public MainWindow()
        {
            InitializeComponent();
            viewModel = new ViewModel(SynchronizationContext.Current);
            viewModel.Messages.CollectionChanged += Messages_CollectionChanged;
            userNameTextBlock.Text = viewModel.UserName;
            inputTextBox.Focus();
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
            if (e.Key != Key.Enter || inputTextBox.Text == string.Empty || !viewModel.IsConnected)
                return;
            var inputText = inputTextBox.Text;
            inputTextBox.Clear();
            viewModel.Messages.Add(new Message(viewModel.UserName, inputText, MessageType.Output));
            viewModel.SendMessage(inputText);
        }

        private void changeNameButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.UserName = changeNameTextBox.Text;
            userNameTextBlock.Text = viewModel.UserName;
        }

        private void createChatButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonsAccessibility(false);
            viewModel.Messages.Add(new Message(null, "Создание чата...", MessageType.Info));
            viewModel.CreateChat();
        }

        private void joinChatButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeButtonsAccessibility(false);
            viewModel.Messages.Add(new Message(null, "Выполняется подключение...", MessageType.Info));
            viewModel.JoinChat(joinChatTextBox.Text);
        }

        private void exitButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ChangeButtonsAccessibility(bool enable)
        {
            joinChatTextBox.IsReadOnly = enable ? false : true;
            changeNameTextBox.IsReadOnly = enable ? false : true;
            joinChatButton.IsEnabled = enable ? true : false;
            changeNameButton.IsEnabled = enable ? true : false;
            createChatButton.IsEnabled = enable ? true : false;
        }

        private void ShowNewMessage(string userName, string message, MessageType messageType)
        {
            var msgBorder = new Border();
            msgBorder.HorizontalAlignment = HorizontalAlignment.Left;
            var msgStackPanel = new StackPanel();
            msgStackPanel.Orientation = Orientation.Vertical;
            var userNameTextBlock = new TextBlock();
            userNameTextBlock.Visibility = Visibility.Collapsed;
            userNameTextBlock.Margin = new Thickness(4, 4, 4, 0);
            userNameTextBlock.Foreground = (Brush)new BrushConverter().ConvertFromString("#FF50687A");
            userNameTextBlock.FontWeight = FontWeights.Bold;
            userNameTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
            userNameTextBlock.Text = userName;
            if (userName != null && !userName.Equals(string.Empty))
                userNameTextBlock.Visibility = Visibility.Visible;
            msgStackPanel.Children.Add(userNameTextBlock);
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
                    ChangeButtonsAccessibility(true);
                    break;
                case MessageType.Output:
                    msgBorder.BorderBrush = (Brush)new BrushConverter().ConvertFromString("#FF528EB8");
                    msgBorder.HorizontalAlignment = HorizontalAlignment.Right;
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
