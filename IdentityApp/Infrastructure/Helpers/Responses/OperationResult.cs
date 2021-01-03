using System.Collections.Generic;
using System.Linq;

namespace IdentityApp.Infrastructure.Helpers.Responses
{
    public class OperationResult
    {
        public bool Succeeded { get; set; }
        public List<string> Messages { get; set; }

        public OperationResult(bool succeeded)
        {
            Succeeded = succeeded;
            Messages = new List<string> { succeeded ? "Succeeded" : "Request error" };
        }

        public OperationResult(bool succeeded, IEnumerable<string> messages)
        {
            Succeeded = succeeded;
            Messages = messages.ToList();
        }
    }
}
