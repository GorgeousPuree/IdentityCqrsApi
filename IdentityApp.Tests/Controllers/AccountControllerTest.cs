using System.Collections.Generic;
using IdentityApp.Controllers;
using IdentityApp.CQRS.Commands;
using IdentityApp.CQRS.Commands.CommandResults;
using IdentityApp.Infrastructure.Helpers.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.Tests.Controllers
{
    [TestFixture]
    public class AccountControllerTest
    {
        [Test]
        public async Task Login_ExistingAccountPassed_ReturnsLoggedInTrue()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var (foundAccountResult, jwt) = (new OperationDataResult<LoginUserCommandResult>(true, new LoginUserCommandResult(true)), "jwt");

            mediator.Setup(m => m.Send(It.IsAny<LoginUserCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((foundAccountResult, jwt)));

            var httpContext = new DefaultHttpContext();
            var controller = new AccountController(mediator.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var result = await controller.Login(new LoginUserCommand());
            var objectResult = result as ObjectResult;
            var operationDataResult = objectResult.Value as OperationDataResult<LoginUserCommandResult>;

            // Assert
            Assert.IsNotEmpty(jwt);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual(true, operationDataResult.Succeeded);
            Assert.AreEqual(true, operationDataResult.Model.LoggedIn);
        }

        [Test]
        public async Task Login_NonExistentAccountPassed_ReturnsLoggedInFalse()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var (foundAccountResult, jwt) = (new OperationDataResult<LoginUserCommandResult>(true, new LoginUserCommandResult(false)), string.Empty);

            mediator.Setup(m => m.Send(It.IsAny<LoginUserCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((foundAccountResult, jwt)));

            var httpContext = new DefaultHttpContext();
            var controller = new AccountController(mediator.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var result = await controller.Login(new LoginUserCommand());
            var objectResult = result as ObjectResult;
            var operationDataResult = objectResult.Value as OperationDataResult<LoginUserCommandResult>;

            // Assert
            Assert.IsEmpty(jwt);
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual(true, operationDataResult.Succeeded);
            Assert.AreEqual(false, operationDataResult.Model.LoggedIn);
        }

        [Test]
        public async Task Login_ServiceFailed_ReturnsSucceededFalse()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var (foundAccountResult, jwt) = (new OperationDataResult<LoginUserCommandResult>(false, new List<string> {"Error"}), string.Empty);

            mediator.Setup(m => m.Send(It.IsAny<LoginUserCommand>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult((foundAccountResult, jwt)));

            var httpContext = new DefaultHttpContext();
            var controller = new AccountController(mediator.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var result = await controller.Login(new LoginUserCommand() { Password = "1" });
            var objectResult = result as ObjectResult;
            var operationDataResult = objectResult.Value as OperationDataResult<LoginUserCommandResult>;

            // Assert
            Assert.IsEmpty(jwt);
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual(false, operationDataResult.Succeeded);
            Assert.AreNotEqual(0, operationDataResult.Messages);
        }

    }
}
