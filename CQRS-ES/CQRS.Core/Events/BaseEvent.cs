using CQRS.Core.Messages;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Core.Events
{
    [BsonIgnoreExtraElements]
    public class BaseEvent : Message
    {
        protected BaseEvent(string type)
        {
            Type = type;
        }

        public string Type { get; set; }
        public int Version { get; set; }
    }
}