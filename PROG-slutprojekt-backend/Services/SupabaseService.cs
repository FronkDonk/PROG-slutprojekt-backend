namespace PROG_slutprojekt_backend.Services
{
    public class SupabaseService
    {
        private readonly Supabase.Client supabase;

        public SupabaseService(Supabase.Client client)
        {
           supabase = client;
        }

        public static async Task<SupabaseService> CreateAsync(string url, string key)
        {
            var options = new Supabase.SupabaseOptions
            {
                AutoConnectRealtime = true,
            };

            var client = new Supabase.Client(url, key, options);
            await client.InitializeAsync();

            return new SupabaseService(client);
        }

        public Supabase.Client Client => supabase;
    }
}
