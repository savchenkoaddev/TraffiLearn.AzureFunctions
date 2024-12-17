namespace TraffiLearn.AzureFunctions
{
    public sealed class MessageEnvelope<T>
    {
        public T Message { get; set; }

        public string[] MessageType { get; set; }
    }
}
