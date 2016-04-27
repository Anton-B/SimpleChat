namespace ChatUtils
{
    public class Message
    {
        public string UserName { get; }
        public string MessageContent { get; }
        public MessageType Type { get; }

        public Message(string userName, string messageContent, MessageType type)
        {
            this.UserName = userName;
            this.MessageContent = messageContent;
            this.Type = type;
        }
    }
}
