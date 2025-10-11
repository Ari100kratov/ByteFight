using Application.Abstractions.Messaging;
using Application.Abstractions.Storage;
using Domain.Assets;
using SharedKernel;

namespace Application.Assets.GetAssetFile;

internal sealed class GetAssetUrlQueryHandler(IStorageService storage)
    : IQueryHandler<GetAssetFileQuery, StreamResult>
{
    public async Task<Result<StreamResult>> Handle(GetAssetFileQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Key))
        {
            return Result.Failure<StreamResult>(AssetErrors.KeyRequired);
        }

        string decodedKey = Uri.UnescapeDataString(query.Key);
        string[] parts = decodedKey.Split('/', 2);

        if (parts.Length != 2)
        {
            return Result.Failure<StreamResult>(AssetErrors.InvalidKeyFormat);
        }

        string bucket = parts[0];
        string objectName = parts[1];

        try
        {
            Stream stream = await storage.GetFileStreamAsync(bucket, objectName, cancellationToken);
            string contentType = GetContentType(objectName);
            return Result.Success(new StreamResult(stream, contentType, Path.GetFileName(objectName)));
        }
        catch (StorageBucketNotFoundException)
        {
            return Result.Failure<StreamResult>(AssetErrors.NotFound(bucket, objectName));
        }
        catch (StorageObjectNotFoundException)
        {
            return Result.Failure<StreamResult>(AssetErrors.NotFound(bucket, objectName));
        }
        catch (StorageAuthorizationException)
        {
            return Result.Failure<StreamResult>(AssetErrors.Unauthorized);
        }
        catch (StorageInvalidNameException)
        {
            return Result.Failure<StreamResult>(AssetErrors.InvalidKeyFormat);
        }
        catch (StorageReadException ex)
        {
            return Result.Failure<StreamResult>(AssetErrors.ReadFailed(ex.Message));
        }
    }

    private static string GetContentType(string filename)
    {
        string ext = Path.GetExtension(filename).ToUpperInvariant();
        return ext switch
        {
            ".PNG" => "image/png",
            ".JPG" or ".JPEG" => "image/jpeg",
            ".WEBP" => "image/webp",
            ".GIF" => "image/gif",
            _ => "application/octet-stream"
        };
    }
}
