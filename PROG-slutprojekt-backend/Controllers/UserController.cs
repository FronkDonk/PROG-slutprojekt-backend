using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG_slutprojekt_backend.Models;
using Supabase.Gotrue;

namespace PROG_slutprojekt_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register(SupabaseUser user)
        {

            var url = "https://wzqbaxbadiqwdodpcglt.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Ind6cWJheGJhZGlxd2RvZHBjZ2x0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTEyODI2NDAsImV4cCI6MjAyNjg1ODY0MH0.edflXOAsbKYV7nuIQaGteGsAbdFaRjB64PyP0uRKnxw";
            try
            {
                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = true
                };

                var supabase = new Supabase.Client(url, key, options);
                await supabase.InitializeAsync();

                var existingUser = supabase.From<SupabaseUser>().Where(x => x.email == user.email).Get();

                

                if(existingUser.Result.Model != null)
                {
                    return new ConflictObjectResult(new { message = "user already has an account" });
                }

                var hashedPassword = Argon2.Hash(user.password);

                var model = new SupabaseUser
                {
                    username = user.username,
                    email = user.email,
                    password = hashedPassword,
                    createdAt = user.createdAt
                };

                await supabase.From<SupabaseUser>().Insert(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
          

            return new JsonResult(new { message = "User registered", success = true });
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SingIn(SignInUser signInUser)
        {

            var url = "https://wzqbaxbadiqwdodpcglt.supabase.co";
            var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6Ind6cWJheGJhZGlxd2RvZHBjZ2x0Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTEyODI2NDAsImV4cCI6MjAyNjg1ODY0MH0.edflXOAsbKYV7nuIQaGteGsAbdFaRjB64PyP0uRKnxw";
            try
            {
                var options = new Supabase.SupabaseOptions
                {
                    AutoConnectRealtime = true
                };

                var supabase = new Supabase.Client(url, key, options);
                await supabase.InitializeAsync();
                var existingUser = supabase.From<SupabaseUser>().Where(x => x.email == signInUser.email).Get();

                var user = existingUser.Result.Model;

                if (user == null)
                {
                    return new NotFoundObjectResult(new { message = "User not found" });
                }
                var passwordIsValid = Argon2.Verify(user.password ,signInUser.password);

                if (!passwordIsValid)
                {
                    return new UnauthorizedObjectResult(new { message = "Invalid credentials" });
                }

             
                return new JsonResult(new {
                    user.username,
                    user.email,
                    user.createdAt,
                    user.id,
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }


        }
    }
}
