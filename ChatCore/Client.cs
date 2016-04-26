using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using ChatUtils;

namespace ChatCore
{
    class Client
    {
        public string Name { get; set; }
        public event EventHandler<NewMessageEventArgs> NewMessage;
        public event EventHandler SuccessJoining;
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

        private void ShowNewMessage(string userName, string message, MessageType messageType)
        {
            context.Post(o => NewMessage?.Invoke(this, new NewMessageEventArgs(userName, message, messageType)), null);
        }

        private void SendJoiningNotification()
        {
            context.Post(o => SuccessJoining?.Invoke(this, EventArgs.Empty), null);
        }

        public async void StartConnection(TcpClient client)
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
                    ShowNewMessage(null, string.Format("К вам подключился {0}.\n\tIP-адрес собеседника: {1}\n\tПорт: {2}\nНачните общение:",
                        interlocutorName, clientData[0], clientData[1]), MessageType.Info);
                    SendJoiningNotification();
                    writer = new BinaryWriter(client.GetStream());
                    writer.Write(Name);
                    await ReadMessage(interlocutorName, reader);
                }
            }
            catch
            {
                ShowNewMessage(null, "Ошибка соединения.", MessageType.Error);
                return;
            }
        }

        private bool VerifyProtocol(BinaryReader reader)
        {
            var assemblyName = Assembly.GetEntryAssembly().GetName();
            return (!reader.ReadBoolean() || !reader.ReadString().Equals(assemblyName.ToString())) ? false : true;
        }

        public async void Connect(string ipAddress)
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
                    ShowNewMessage(null, string.Format("Вы успешно подключились к собеседнику по имени {0}.", interlocutorName), MessageType.Success);
                    SendJoiningNotification();
                    ShowNewMessage(null, "Начните общение:", MessageType.Info);
                    await ReadMessage(interlocutorName, reader);
                }
            }
            catch
            {
                ShowNewMessage(null, "Ошибка соединения.", MessageType.Error);
                return;
            }
        }

        private async Task ReadMessage(string interlocutorName, BinaryReader reader)
        {
            string message = string.Empty;
            while (true)
            {
                if ((message = await Task.Run(() => GetInterlocutorMessageAsync(reader))).Equals(string.Empty))
                    return;
                else
                    ShowNewMessage(interlocutorName, message, MessageType.Input);
            }
        }

        private string GetInterlocutorMessageAsync(BinaryReader reader)
        {
            try
            {
                return reader.ReadString();
            }
            catch
            {
                ShowNewMessage(null, "Ошибка соединения. Возможно, ваш собеседник покинул чат.", MessageType.Error);
                return string.Empty;
            }
        }

        public void SendMessage(string message)
        {
            try
            {
                if (message.Equals(string.Empty))
                    return;
                writer.Write(message);
            }
            catch
            {
                ShowNewMessage(null, "Ошибка отправки сообщения. Возможно, ваш собеседник покинул чат.", MessageType.Error);
                return;
            }
        }
    }
}
