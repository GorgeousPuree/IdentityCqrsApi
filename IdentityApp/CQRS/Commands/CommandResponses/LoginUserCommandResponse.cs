namespace IdentityApp.CQRS.Commands.CommandResponses
{
    public class LoginUserCommandResponse
    {
        public bool AccountExists { get; set; }
        public string Jwt { get; set; }

        public LoginUserCommandResponse(bool accountExists)
        {
            AccountExists = accountExists;
        }

        public LoginUserCommandResponse(string jwt, bool accountExists = true)
        {
            AccountExists = accountExists;
            Jwt = jwt;
        }
    }
}
