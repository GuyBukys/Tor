using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;
using Tor.Application.Abstractions;
using Tor.Contracts.Images;
using Tor.TestsInfra;
using Tor.TestsInfra.CustomFakers;

namespace Tor.IntegrationTests.Images;
public class ImageControllerTests : BaseIntegrationTest
{
    private readonly Mock<IStorageManager> _storageManagerMock;

    public ImageControllerTests(IntegrationTestWebApplicationFactory factory)
        : base(factory)
    {
        _storageManagerMock = factory.StorageManagerMock;

        _storageManagerMock.Setup(x => x.IsFileExists(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [Fact]
    public async Task UploadImage_WhenValidRequest_ShouldUploadSuccessfully()
    {
        Uri fileUri = new("https://google.com/");
        Uri signedUri = new("https://signeduri.com");
        var request = Fakers.Images.UploadImageCommandFaker.Generate();
        _storageManagerMock.Setup(x => x.UploadFile(It.IsAny<string>(), It.IsAny<string>(), request.ContentAsBase64, It.IsAny<CancellationToken>()))
            .ReturnsAsync(fileUri);
        _storageManagerMock.Setup(x => x.SignUrl(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(signedUri);

        var result = await Client.PostAsJsonAsync(ImageControllerConstants.UploadImageUri, request);

        result.StatusCode.Should().Be(HttpStatusCode.OK);
        UploadImageResponse? response = await result.Content.ReadFromJsonAsync<UploadImageResponse>();
        response.Should().NotBeNull();
        response!.NewFileName.Should().Contain($"{request.EntityType}/{request.ImageType}");
        response!.OriginalUrl.Should().Be(fileUri);
        response!.SignedUrl.Should().Be(signedUri);
    }
}
