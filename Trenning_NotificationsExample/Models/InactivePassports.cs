using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Trenning_NotificationsExample.Models
{
    public class InactivePassports
    {        
        [BsonId]
        [BsonRepresentation(BsonType.String)] 
        public string Id { get; set; }
               
    }
}
