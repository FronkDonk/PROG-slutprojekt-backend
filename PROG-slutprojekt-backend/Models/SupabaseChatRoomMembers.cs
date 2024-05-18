using Postgrest.Attributes;
using Postgrest.Models;

namespace PROG_slutprojekt_backend.Models
{
    [Table("prog_chat_room_members")]

    public class SupabaseChatRoomMembers : BaseModel
    {
        [PrimaryKey("id")]
        public int? id { get; set; }

        [Column("chat_room_id")]
        public string? chatRoomId { get; set; }

        [Column("user_id")]
        public string? userId { get; set; }

        [Column("joined_at")]
        public DateTime joinedAt { get; set; }

        [Reference(typeof(SupabaseChatRooms))]
        public SupabaseChatRooms? chatRoom { get; set; }

        [Reference(typeof(SupabaseUser))]
        public SupabaseUser? user { get; set; }

    }
}
