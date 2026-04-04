using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace fixflow.web.Data
{
    public class FfAccountNote
    {
        [BsonId]
        public ObjectId Id { get; set; }
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid TicketId { get; set; }
        public string NoteText { get; set; } = string.Empty;
        public string EnteredByUserId { get; set; } = string.Empty;
        public bool InternalOnly { get; set; } = true;
        public DateTime TimeStamp { get; set; }
    }
}

