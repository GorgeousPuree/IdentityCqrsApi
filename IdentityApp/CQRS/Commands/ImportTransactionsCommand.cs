using CsvHelper;
using CsvHelper.TypeConversion;
using IdentityApp.CQRS.Commands.CommandResults;
using IdentityApp.Infrastructure.Helpers.Responses;
using IdentityApp.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityApp.CQRS.Commands
{
    public class ImportTransactionsCommand : IRequest<OperationDataResult<ImportTransactionsCommandResult>>
    {
        public IFormFile File { get; set; }

        public ImportTransactionsCommand(IFormFile file)
        {
            File = file;
        }
    }

    public class ImportTransactionsCommandHandler : IRequestHandler<ImportTransactionsCommand, OperationDataResult<ImportTransactionsCommandResult>>
    {
        public async Task<OperationDataResult<ImportTransactionsCommandResult>> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                using (var reader = new StreamReader(request.File.OpenReadStream()))
                {
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
                        csv.Configuration.Delimiter = ",";
                        csv.Configuration.RegisterClassMap<TransactionCsvMap>();

                        var records = await csv.GetRecordsAsync<TransactionModel>().ToListAsync(cancellationToken);
                        // TODO: add records to database
                    }
                }
            }
            catch (TypeConverterException e)
            {
                var errorReason = e.MemberMapData.Member.Name == "Status" ? "status" : "transaction";
                return new OperationDataResult<ImportTransactionsCommandResult>(
                    true,
                    new List<string>
                        {$"Failed at {e.ReadingContext.RawRow} line. Unknown transaction {errorReason}: {e.Text}"},
                    new ImportTransactionsCommandResult(false));
            }
            catch (HeaderValidationException e)
            {
                var invalidHeaders = string.Join(", ", e.InvalidHeaders.Select(header => header.Names[0]));
                return new OperationDataResult<ImportTransactionsCommandResult>(
                    true,
                    new List<string>
                        {$"Failed at reading csv headers. Couldn't read {invalidHeaders} headers."});
            }
            catch (Exception)
            {
                return new OperationDataResult<ImportTransactionsCommandResult>(
                    true,
                    new List<string> { "Unknown error occurred during reading CSV." });
            }

            return new OperationDataResult<ImportTransactionsCommandResult>(true, new ImportTransactionsCommandResult(false));
        }
    }
}
