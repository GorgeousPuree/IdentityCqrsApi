namespace IdentityApp.CQRS.Commands.CommandResults
{
    public class LoginUserCommandResult
    {
        public bool LoggedIn { get; set; }

        public LoginUserCommandResult(bool loggedIn)
        {
            LoggedIn = loggedIn;
        }
    }
}
