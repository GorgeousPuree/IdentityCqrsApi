using System.Collections.Generic;

namespace IdentityApp.Infrastructure.Helpers.Responses
{
    public class OperationDataResult<TModel> : OperationResult
    {
        public TModel Model { get; set; }
        OperationDataResult(bool succeeded) : base(succeeded)
        {
        }
        public OperationDataResult(bool success, IEnumerable<string> messages) : base(success, messages)
        {
        }

        public OperationDataResult(bool success, TModel model) : base(success)
        {
            Model = model;
        }

        public OperationDataResult(bool success, IEnumerable<string> messages, TModel model) : base(success, messages)
        {
            Model = model;
        }
    }
}
