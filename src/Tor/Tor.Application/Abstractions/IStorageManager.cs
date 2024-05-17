using Tor.Domain.CategoryAggregate.Enums;
using Tor.Domain.Common.Enums;
using Tor.Domain.Common.ValueObjects;

namespace Tor.Application.Abstractions;

public interface IStorageManager
{
    Task<Uri> UploadFile(string fileName, string contentType, string fileContent, CancellationToken cancellationToken);

    Task<Uri> SignUrl(string fileName, CancellationToken cancellationToken);

    Task<bool> IsFileExists(string fileName, CancellationToken cancellationToken);

    Image GetDefaultImage(ImageType imageType, EntityType entityType, CategoryType? categoryType = null);
}
