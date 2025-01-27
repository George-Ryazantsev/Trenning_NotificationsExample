using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Trenning_NotificationsExample.Models
{
    public class PassportChanges
    {
     
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; }

        [BsonIgnore] 
        public string Series => Id.Split('_')[0]; 

        [BsonIgnore]
        public string Number => Id.Split('_')[1]; 

        [BsonElement("ChangeType")]
        public string ChangeType { get; set; }

        [BsonElement("ChangeDate")]
        public DateTime ChangeDate { get; set; }
    }
}
