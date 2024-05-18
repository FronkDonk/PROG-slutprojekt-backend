using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PROG_slutprojekt_backend.Models;
using Supabase.Gotrue;

namespace PROG_slutprojekt_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        [HttpGet("chatrooms/{userId}")]
        public async Task<IActionResult> GetChatRoomsByUserId(string userId)
        {

            //hej
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
                //get 
                var res = supabase.From<SupabaseChatRoomMembers>().Where(x => x.userId == userId).Get();

                var chatRooms = res.Result.Models;

                if (chatRooms == null || chatRooms.Count == 0)
                {
                    Console.WriteLine($"No chat rooms found for user {userId}");
                    return new JsonResult(new { error = $"No chat rooms found for user {userId}" });
                }

                var chatRoomsDetails = new List<object>();

                foreach (var chatRoom in chatRooms)
                {
                    var chatRoomId = chatRoom.chatRoomId;
                    var membersRes = await supabase.From<SupabaseChatRoomMembers>().Where(x => x.chatRoomId == chatRoomId).Get();
                    var members = membersRes.Models;
                    var otherParticipants = members.Where(m => m.userId != userId).Select(m => new
                    {
                        m?.user?.id,
                        m?.user?.username,
                        m?.user?.email,
                    });

                    chatRoomsDetails.Add(new
                    {
                        chatRoomId,
                        otherParticipants   
                    });    
                }

                return new JsonResult(new { chatRoomsDetails });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }


        }

        [HttpPost("chatrooms/sendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SentMessage request)
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

                var chatMessage = new SupabaseChatMessages
                {
                    chatRoomId = request.roomId,
                    message = request.message,
                    userId = request.userId,
                };

                await supabase.From<SupabaseChatMessages>().Insert(chatMessage);

                return StatusCode(200, new { success = true});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }
        [HttpGet("chatrooms/messages/{chatRoomId}")]
        public async Task<IActionResult> GetChatMessagesById(string chatRoomId)
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

                var res = supabase.From<SupabaseChatMessages>().Where(x => x.chatRoomId == chatRoomId).Get();

                var chatMessages = res.Result.Models;
                
                
                if (chatMessages == null || chatMessages.Count == 0)
                {
                    Console.WriteLine($"No chat messages found for chat room {chatRoomId}");
                    return new JsonResult(new { error = $"No chat messages found for chat room {chatRoomId}" });
                }

                //make new json object with only the necessary fields
                var messages = chatMessages.Select(m => new
                {
                    m.message,
                    m.sentAt,
                    m.chatRoomId,
                    m.userId,
                    m.user?.username
                }).ToList();

                return new JsonResult(new { messages });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }



        }
    }
}
