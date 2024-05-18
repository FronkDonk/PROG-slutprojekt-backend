using Postgrest.Attributes;

namespace PROG_slutprojekt_backend.Models
{
    public class SignInUser
    {
        public string? email { get; set; }
        public string? password { get; set; }
    }
}
