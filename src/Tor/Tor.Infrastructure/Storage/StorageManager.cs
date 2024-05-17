using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tor.Application.Abstractions;
using Tor.Domain.CategoryAggregate.Enums;
using Tor.Domain.Common.Enums;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Infrastructure.Storage;

internal sealed class StorageManager : IStorageManager
{
    private readonly GoogleStorageSettings _settings;
    private readonly GoogleCredential _credential;
    private readonly ILogger<StorageManager> _logger;

    private const string _defaultFolderName = "Default";
    private const string _googleStorageUrl = "https://storage.cloud.google.com";

    public StorageManager(IOptions<GoogleStorageSettings> settings, ILogger<StorageManager> logger, GoogleCredential credential)
    {
        _settings = settings.Value;
        _logger = logger;
        _credential = credential;
    }

    public async Task<Uri> UploadFile(
        string fileName,
        string contentType,
        string fileContent,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("starting to upload file {fileName}", fileName);

        using MemoryStream stream = new(Convert.FromBase64String(fileContent));
        using var client = await StorageClient.CreateAsync(_credential);

        var uploadedFile = await client.UploadObjectAsync(
            _settings.BucketName,
            fileName,
            contentType,
            stream,
            cancellationToken: cancellationToken);

        _logger.LogInformation("finished to upload file {fileName}. link: {MediaLink}", fileName, uploadedFile.MediaLink);

        return new Uri(uploadedFile.MediaLink);
    }

    public async Task<Uri> SignUrl(string fileName, CancellationToken cancellationToken)
    {
        UrlSigner urlSigner = UrlSigner.FromCredential(_credential);

        string signedUrl = await urlSigner.SignAsync(
            _settings.BucketName,
            fileName,
            TimeSpan.FromHours(10),
            HttpMethod.Get,
            cancellationToken: cancellationToken);

        return new Uri(signedUrl);
    }

    public async Task<bool> IsFileExists(string fileName, CancellationToken cancellationToken)
    {
        using var client = await StorageClient.CreateAsync(_credential);

        try
        {
            var result = await client.GetObjectAsync(
                _settings.BucketName,
                fileName,
                cancellationToken: cancellationToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public Image GetDefaultImage(ImageType imageType, EntityType entityType, CategoryType? categoryType = null)
    {
        string name = $"{_defaultFolderName}/{entityType}/{imageType}";

        if (categoryType is not null)
        {
            name += $"/{categoryType}";
        }

        Uri url = new($"{_googleStorageUrl}/{_settings.BucketName}/{name}");

        return new Image(name, url);
    }
}
