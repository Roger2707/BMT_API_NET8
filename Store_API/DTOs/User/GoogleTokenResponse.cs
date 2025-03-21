namespace Store_API.DTOs.User
{
    public class GoogleTokenResponse
    {
        public string Access_token { get; set; }
        public string Id_token { get; set; }
        public string Eefresh_token { get; set; }
        public string Expires_in { get; set; }
        public string Token_type { get; set; }
    }

}
