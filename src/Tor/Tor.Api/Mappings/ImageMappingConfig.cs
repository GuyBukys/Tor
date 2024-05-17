using Mapster;
using Tor.Application.Images.Commands.Upload;
using Tor.Contracts.Images;

namespace Tor.Api.Mappings;

public class ImageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<UploadImageResult, UploadImageResponse>()
            .Map(dest => dest.NewFileName, src => src.FileName);
    }
}
