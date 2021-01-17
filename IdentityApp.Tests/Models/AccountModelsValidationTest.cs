using IdentityApp.CQRS.Commands;
using IdentityApp.CQRS.Queries;
using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityApp.Tests.Models
{
    [TestFixture]
    public class AccountModelsValidationTest
    {
        [Test]
        public void Register_UsernameAndPasswordAreTooShort_ReturnsTwoErrors()
        {
            var createUserCommand = new CreateUserCommand()
            {
                Password = "1",
                Username = "J",
            };

            var errorCount = ValidateModel(createUserCommand).Count;
            Assert.AreEqual(2, errorCount);
        }

        [Test]
        public void Register_UsernameIsRequired_ReturnsOneError()
        {
            var createUserCommand = new CreateUserCommand()
            {
                Password = "123456",
            };

            var errorCount = ValidateModel(createUserCommand).Count;
            Assert.AreEqual(1, errorCount);
        }

        [Test]
        public void Register_PasswordIsRequired_ReturnsOneError()
        {
            var createUserCommand = new CreateUserCommand()
            {
                Username = "qwerty",
            };

            var errorCount = ValidateModel(createUserCommand).Count;
            Assert.AreEqual(1, errorCount);
        }

        [Test]
        public void Register_UsernameAndPasswordAreTooLong_ReturnsTwoErrors()
        {
            var createUserCommand = new CreateUserCommand()
            {
                Username =
                    "qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty",
                Password =
                    "123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456",
            };

            var errorCount = ValidateModel(createUserCommand).Count;
            Assert.AreEqual(2, errorCount);
        }

        [Test]
        public void Register_ValidBodyPassed_ReturnsZeroErrors()
        {
            var createUserCommand = new CreateUserCommand()
            {
                Username = "qwerty",
                Password = "123456"
            };

            var errorCount = ValidateModel(createUserCommand).Count;
            Assert.AreEqual(0, errorCount);
        }

        [Test]
        public void Login_UsernameAndPasswordAreTooShort_ReturnsTwoErrors()
        {
            var loginUserCommand = new LoginUserCommand()
            {
                Password = "1",
                Username = "J",
            };

            var errorCount = ValidateModel(loginUserCommand).Count;
            Assert.AreEqual(2, errorCount);
        }

        [Test]
        public void Login_UsernameIsRequired_ReturnsOneError()
        {
            var loginUserCommand = new LoginUserCommand()
            {
                Password = "123456",
            };

            var errorCount = ValidateModel(loginUserCommand).Count;
            Assert.AreEqual(1, errorCount);
        }

        [Test]
        public void Login_PasswordIsRequired_ReturnsOneError()
        {
            var loginUserCommand = new LoginUserCommand()
            {
                Username = "qwerty",
            };

            var errorCount = ValidateModel(loginUserCommand).Count;
            Assert.AreEqual(1, errorCount);
        }

        [Test]
        public void Login_UsernameAndPasswordAreTooLong_ReturnsTwoErrors()
        {
            var loginUserCommand = new LoginUserCommand()
            {
                Username =
                    "qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty",
                Password =
                    "123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456_123456",
            };

            var errorCount = ValidateModel(loginUserCommand).Count;
            Assert.AreEqual(2, errorCount);
        }

        [Test]
        public void Login_ValidBodyPassed_ReturnsZeroErrors()
        {
            var loginUserCommand = new LoginUserCommand()
            {
                Username = "qwerty",
                Password = "123456"
            };

            var errorCount = ValidateModel(loginUserCommand).Count;
            Assert.AreEqual(0, errorCount);
        }

        [Test]
        public void IsUsernameTaken_UsernameIsTooShort_ReturnsTwoErrors()
        {
            var isUsernameTakenQuery = new IsUsernameTakenQuery()
            {
                Username = "J",
            };

            var errorCount = ValidateModel(isUsernameTakenQuery).Count;
            Assert.AreEqual(1, errorCount);
        }

        [Test]
        public void IsUsernameTaken_UsernameIsRequired_ReturnsOneError()
        {
            var isUsernameTakenQuery = new IsUsernameTakenQuery();

            var errorCount = ValidateModel(isUsernameTakenQuery).Count;
            Assert.AreEqual(1, errorCount);
        }

        [Test]
        public void IsUsernameTaken_UsernameAndPasswordAreTooLong_ReturnsTwoErrors()
        {
            var isUsernameTakenQuery = new IsUsernameTakenQuery()
            {
                Username =
                    "qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty_qwerty",
            };

            var errorCount = ValidateModel(isUsernameTakenQuery).Count;
            Assert.AreEqual(1, errorCount);
        }

        [Test]
        public void IsUsernameTaken_ValidBodyPassed_ReturnsZeroErrors()
        {
            var isUsernameTakenQuery = new IsUsernameTakenQuery()
            {
                Username = "qwerty"
            };

            var errorCount = ValidateModel(isUsernameTakenQuery).Count;
            Assert.AreEqual(0, errorCount);
        }


        private IList<ValidationResult> ValidateModel(object model)
        {
            var result = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            Validator.TryValidateObject(model, validationContext, result, true);
            if (model is IValidatableObject) (model as IValidatableObject).Validate(validationContext);

            return result;
        }
    }
}
