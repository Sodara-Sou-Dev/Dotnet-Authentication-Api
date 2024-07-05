namespace cube_gaming_store_back.DTOs
{
    public class GeneralResponse
    {
        public record class SignInResponse(string Token, string Msg);
        public record class SignUpResponse(bool Flag, string Msg);
        public record UserSession(string? Id, string? FirstName, string? LastName, string? Email, string? Role);
    }
}