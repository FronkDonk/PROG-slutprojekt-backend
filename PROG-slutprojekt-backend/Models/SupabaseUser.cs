using Postgrest.Attributes;
using Postgrest.Models;

namespace PROG_slutprojekt_backend.Models
{
    [Table("prog_users")]
    public class SupabaseUser : BaseModel
    {
        [PrimaryKey("id")]
        public string? id { get; set; }

        [Column("username")]
        public string? username { get; set; }

        [Column("email")]
        public string? email { get; set; }

        [Column("password")]
        public string? password { get; set; }

        [Column("created_at")]
        public DateTime createdAt { get; set; }

        [Column("avatar_color_1")]
        public string? avatarColor1 { get; set; }

        [Column("avatar_color_2")]
        public string? avatarColor2 { get; set; }


    }

    
}
