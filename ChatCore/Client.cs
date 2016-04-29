using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Collections.ObjectModel;
using ChatUtils;

namespace ChatCore
{
    public class Client
    {
        private BinaryWriter writer;
        private BinaryReader reader;
        private SynchronizationContext context;

        public Client(SynchronizationContext context)
        {
            this.context = context;
        }

        public Client(SynchronizationContext context, string userName) : this(context)
        {
            this.Name = userName;
        }

        public ObservableCollection<Message> IncomingMessagesCollection { get; } = new ObservableCollection<Message>();
        internal string Name { get; set; }

        public event EventHandler<ChatUtils.ErrorEventArgs> ErrorOccured;
        public event EventHandler<SuccessJoiningEventArgs> SuccessJoining;

        internal void SendMessage(string message)
        {
            try
            {
                if (message.Equals(string.Empty))
                    return;
                writer.Write(message);
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception("Ошибка отправки сообщения. Возможно, ваш собеседник покинул чат.", ex));
                return;
            }
        }

        internal void StartConnection(TcpClient client)
        {
            try
            {
                using (client)
                {
                    reader = new BinaryReader(client.GetStream());
                    if (!VerifyProtocol(reader))
                        return;
                    var interlocutorName = reader.ReadString();
                    var clientData = client.Client.RemoteEndPoint.ToString().Split(':');
                    OnSuccessJoining(string.Format("К вам подключился {0}.\n\tIP-адрес собеседника: {1}\n\tПорт: {2}\nНачните общение:",
                        interlocutorName, clientData[0], clientData[1]));
                    writer = new BinaryWriter(client.GetStream());
                    writer.Write(Name);
                    ReadMessage(interlocutorName, reader);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception("Ошибка соединения.", ex));
                return;
            }
        }

        internal async void ConnectAsync(string ipAddress)
        {
            await Task.Run(() => Connect(ipAddress));
        }

        private void Connect(string ipAddress)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    var ip = IPAddress.Parse(ipAddress);
                    var port = 81;
                    client.Connect(ip, port);
                    writer = new BinaryWriter(client.GetStream());
                    var assemblyName = Assembly.GetEntryAssembly().GetName();
                    writer.Write(true);
                    writer.Write(assemblyName.ToString());
                    writer.Write(Name);
                    reader = new BinaryReader(client.GetStream());
                    var interlocutorName = reader.ReadString();
                    OnSuccessJoining(string.Format("Вы успешно подключились к собеседнику по имени {0}.\nНачните общение: ", interlocutorName));
                    ReadMessage(interlocutorName, reader);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception("Ошибка соединения.", ex));
                return;
            }
        }

        private void ReadMessage(string interlocutorName, BinaryReader reader)
        {
            string message = string.Empty;
            while (true)
            {
                if ((message = GetInterlocutorMessage(reader)).Equals(string.Empty))
                    return;
                else
                    context.Post(o => IncomingMessagesCollection.Add(new Message(interlocutorName, message, MessageType.Input)), null);
            }
        }

        private string GetInterlocutorMessage(BinaryReader reader)
        {
            try
            {
                return reader.ReadString();
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception("Ошибка соединения. Возможно, ваш собеседник покинул чат.", ex));
                return string.Empty;
            }
        }

        private void OnSuccessJoining(string message)
        {
            context.Post(o => SuccessJoining?.Invoke(this, new SuccessJoiningEventArgs(message)), null);
        }

        private void OnErrorOccured(Exception exception)
        {
            context.Post(o => ErrorOccured?.Invoke(this, new ChatUtils.ErrorEventArgs(exception)), null);
        }

        private bool VerifyProtocol(BinaryReader reader)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            return (!reader.ReadBoolean() || !reader.ReadString().Equals(assemblyName.ToString())) ? false : true;
        }
    }
}
