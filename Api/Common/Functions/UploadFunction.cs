using AzureStaticWebApp.Api.Common.Data;
using AzureStaticWebApp.Common.Shared.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http.Headers;

namespace AzureStaticWebApp.Api.Common.Functions;

public class UploadFunction
{
    private readonly IAzureStorageService _azureStorageService;
    private readonly IGuid _guid;

    public UploadFunction(IAzureStorageService azureStorageService, IGuid guid)
    {
        _azureStorageService = azureStorageService;
        _guid = guid;
    }

    [FunctionName("UploadFunction")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "manage/upload")] HttpRequest req, CancellationToken cancellationToken)
    {
        try
        {
            var file = req.Form.Files[0];

            if (file.Length > 0)
            {
                var content = ContentDispositionHeaderValue.Parse(file.ContentDisposition);
                if (content?.FileName != null)
                {
                    var fileNameCleaned = content.FileName.Trim('"');
                    var fileExtension = fileNameCleaned.Split('.').Last();
                    var fileWithoutExtension = fileNameCleaned.Replace($".{fileExtension}", string.Empty);
                    var fileName = $"{fileWithoutExtension}-{_guid.NewGuid}.{fileExtension}";
                    var fileUrl = await _azureStorageService.UploadFileAsync(file.OpenReadStream(), fileName, file.ContentType, cancellationToken);
                    return new OkObjectResult(fileUrl);
                }
                else
                {
                    return new BadRequestResult();
                }
            }
            else
            {
                return new BadRequestResult();
            }
        }
        catch (Exception)
        {
            return new BadRequestResult();
        }
    }
}