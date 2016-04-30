using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ChatCore
{
    public class Client
    {
        private SynchronizationContext context;
        private BinaryWriter writer;
        private BinaryReader reader;
        private Chat chat;

        public Client(Chat chat, SynchronizationContext context, string userName)
        {
            this.chat = chat;
            this.context = context;
            this.Name = userName;
        }

        public Client(Chat chat, SynchronizationContext context) : this(chat, context, null) { }

        internal string Name { get; set; }

        internal void SendMessage(string message)
        {
            try
            {
                if (message == string.Empty)
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
                    chat.InterlocutorName = reader.ReadString();
                    var clientData = client.Client.RemoteEndPoint.ToString().Split(':');
                    chat.InterlocutorIP = clientData[0];
                    chat.InterlocutorPort = clientData[1];
                    OnSuccessJoining();
                    writer = new BinaryWriter(client.GetStream());
                    writer.Write(Name);
                    ReadMessage(reader);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception("Ошибка соединения.", ex));
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
                    chat.InterlocutorName = reader.ReadString();
                    OnSuccessJoining();
                    ReadMessage(reader);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccured(new Exception("Ошибка соединения.", ex));
            }
        }

        private void ReadMessage(BinaryReader reader)
        {
            var message = string.Empty;
            while (true)
            {
                message = reader.ReadString();
                context.Post(o => chat.IncomingMessages.Add(message), null);
            }
        }

        private void OnSuccessJoining()
        {
            context.Post(o => chat.OnSuccessJoining(), null);
        }

        private bool VerifyProtocol(BinaryReader reader)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            return (!reader.ReadBoolean() || reader.ReadString() != assemblyName.ToString()) ? false : true;
        }

        private void OnErrorOccured(Exception exception)
        {
            context.Post(o => chat.OnErrorOccured(exception), null);
        }
    }
}
