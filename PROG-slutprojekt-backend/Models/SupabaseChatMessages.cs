using Postgrest.Attributes;
using Postgrest.Models;

namespace PROG_slutprojekt_backend.Models
{

    [Table("prog_messages")]
    public class SupabaseChatMessages : BaseModel
    {
            [PrimaryKey("id")]
            public string? id { get; set; }

            [Column("message")]
            public string? message { get; set; }

            [Column("sent_at")]
            public DateTime sentAt { get; set; }

            [Column("chat_room_id")] 
            public string? chatRoomId { get; set; }

            [Column("user_id")]
            public string? userId { get; set; }

            [Reference(typeof(SupabaseChatRooms))]
            public SupabaseChatRooms? chatRoom { get; set; }

            [Reference(typeof(SupabaseUser))]   
            public SupabaseUser? user { get; set; }
    }
    
}
