using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG_slutprojekt_backend.Models;
using Supabase.Gotrue;
using System.Diagnostics;

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

                // Check if a user with the same email already exists
                var existingUser = supabase.From<SupabaseUser>().Where(x => x.email == user.email).Get();


                // If the user already exists, return a conflict result
                if (existingUser.Result.Model != null)
                {
                    return new ConflictObjectResult(new { message = "user already has an account" });
                }

                // Hash the user's password using Argon2
                var hashedPassword = Argon2.Hash(user.password);

                // Create a new user model with the hashed password
                var model = new SupabaseUser
                {
                    username = user.username,
                    email = user.email,
                    password = hashedPassword,
                    createdAt = user.createdAt,
                    avatarColor1 = user.avatarColor1,
                    avatarColor2 = user.avatarColor2,
                };

                // Insert the new user into the database
                var storedUser = await supabase.From<SupabaseUser>().Insert(model);

                // Get the stored user model
                var jsonUser = storedUser.Model;

                // Return the stored user model as a JSON result
                return new JsonResult(new
                {
                    jsonUser.id,
                    jsonUser.username,
                    jsonUser.email,
                    jsonUser.createdAt,
                    jsonUser.avatarColor1,
                    jsonUser.avatarColor2,
                });

                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
          

        }
        [HttpGet("usersNotInChatRoom/{userId}")]
        public async Task<IActionResult> GetUsersNotInChatRoom(string userId)
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

                // Get all chat room members
                var allChatRoomMembers = supabase.From<SupabaseChatRoomMembers>().Get().Result.Models;

                // Get the chat rooms where the user is a member
                var userChatRooms = allChatRoomMembers.Where(x => x.userId == userId).Select(x => x.chatRoomId).ToList();

                // Get the user IDs of the members in the user's chat rooms
                var userIds = allChatRoomMembers.Where(x => userChatRooms.Contains(x.chatRoomId)).Select(x => x.userId).Distinct().ToList();

                // Get all users
                var allUsers = supabase.From<SupabaseUser>().Get().Result.Models;

                // Filter out the users who are in the same chat room with the specified user
                var usersNotInSameChatRoom = allUsers.Where(user => !userIds.Contains(user.id) && user.id != userId).Select(x => new
                {
                    x.id,
                    x.username,
                    x.email,       
                    x.avatarColor1,
                    x.avatarColor2,
                }).ToList();

                return new JsonResult(usersNotInSameChatRoom);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn(SignInUser signInUser)
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
                // Get the user with the provided email
                var existingUser = supabase.From<SupabaseUser>().Where(x => x.email == signInUser.email).Get();

                var user = existingUser.Result.Model;
                // If the user does not exist, return a not found result
                if (user == null)
                {
                    return new NotFoundObjectResult(new { message = "User not found" });
                }
                // Verify the user's password
                var passwordIsValid = Argon2.Verify(user.password ,signInUser.password);
                // If the password is not valid, return an unauthorized result
                if (!passwordIsValid)
                {
                    return new UnauthorizedObjectResult(new { message = "Invalid credentials" });
                }

                // Return the user model as a JSON result
                return new JsonResult(new {
                    user.username,
                    user.email,
                    user.createdAt,
                    user.id,
                    user.avatarColor1,
                    user.avatarColor2,
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
