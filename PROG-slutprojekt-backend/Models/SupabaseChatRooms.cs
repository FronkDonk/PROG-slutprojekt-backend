using Postgrest.Models;
using Postgrest.Attributes;

namespace PROG_slutprojekt_backend.Models
{
    [Table("prog_chat_rooms")]
    public class SupabaseChatRooms : BaseModel
    {
        [PrimaryKey("id")]
        public string? id { get; set; }

        [Column("created_at")]
        public DateTime createdAt { get; set; }
    }
}
