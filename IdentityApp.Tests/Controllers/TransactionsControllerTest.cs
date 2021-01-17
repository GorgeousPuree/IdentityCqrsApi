using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IdentityApp.Controllers;
using IdentityApp.CQRS.Commands;
using IdentityApp.CQRS.Commands.CommandResults;
using IdentityApp.CQRS.Queries;
using IdentityApp.CQRS.Queries.QueryResults;
using IdentityApp.Infrastructure.Helpers.Responses;
using IdentityApp.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

namespace IdentityApp.Tests.Controllers
{
    [TestFixture]
    public class TransactionsControllerTest
    {
        [Test]
        public async Task GetTransactionsPage_ServiceSucceeded_Returns200()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var transactionPageQueryResult = new OperationDataResult<TransactionPageQueryResult>(true, new TransactionPageQueryResult(1, new List<TransactionModel>()));

            mediator.Setup(m => m.Send(It.IsAny<TransactionPageQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(transactionPageQueryResult));

            var controller = new TransactionsController(mediator.Object);

            // Act
            var result = await controller.GetTransactionsPage(new TransactionPageQuery());
            var objectResult = result as ObjectResult;
            var operationDataResult = objectResult.Value as OperationDataResult<TransactionPageQueryResult>;

            // Assert
            Assert.AreEqual(200, objectResult.StatusCode);
            Assert.AreEqual(true, operationDataResult.Succeeded);
            Assert.IsNotNull(operationDataResult.Model.Transactions);
            Assert.AreEqual(transactionPageQueryResult.Model.TotalTransactionsCount, operationDataResult.Model.TotalTransactionsCount);
        }

        [Test]
        public async Task GetTransactionsPage_ServiceFailed_Returns500()
        {
            // Arrange
            var mediator = new Mock<IMediator>();
            var transactionPageQueryResult = new OperationDataResult<TransactionPageQueryResult>(false, new List<string> { "Error" });

            mediator.Setup(m => m.Send(It.IsAny<TransactionPageQuery>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(transactionPageQueryResult));

            var controller = new TransactionsController(mediator.Object);

            // Act
            var result = await controller.GetTransactionsPage(new TransactionPageQuery());
            var objectResult = result as ObjectResult;
            var operationDataResult = objectResult.Value as OperationDataResult<TransactionPageQueryResult>;

            // Assert
            Assert.AreEqual(500, objectResult.StatusCode);
            Assert.AreEqual(false, operationDataResult.Succeeded);
            Assert.AreNotEqual(0, operationDataResult.Messages);
        }

        //[Test]
        //public async Task GetExportedTransactions_ServiceSucceeded_Returns200()
        //{
        //    // Arrange
        //    var mediator = new Mock<IMediator>();
        //    var exportedTransactionsQueryResult = new OperationDataResult<ExportTransactionsQueryResult>(true, new ExportTransactionsQueryResult(new byte[] { 1 }));

        //    mediator.Setup(m => m.Send(It.IsAny<ExportTransactionsQuery>(),
        //        It.IsAny<CancellationToken>()))
        //        .Returns(Task.FromResult(exportedTransactionsQueryResult));

        //    var controller = new TransactionsController(mediator.Object);

        //    // Act
        //    var result = await controller.GetExportedTransactions(new ExportTransactionsQuery());
        //    var type = result.GetType();
        //    var objectResult = result as FileContentResult;
        //}
    }
}
