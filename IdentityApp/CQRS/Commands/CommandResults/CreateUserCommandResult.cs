namespace IdentityApp.CQRS.Commands.CommandResults
{
    public class CreateUserCommandResult
    {
        public bool IsCreated { get; set; }

        public CreateUserCommandResult(bool isCreated)
        {
            IsCreated = isCreated;
        }
    }
}
