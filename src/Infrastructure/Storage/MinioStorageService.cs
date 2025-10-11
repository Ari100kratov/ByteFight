using Application.Abstractions.Storage;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Infrastructure.Storage;

public sealed class MinioStorageService(IMinioClient minioClient) : IStorageService
{
    public async Task<Stream> GetFileStreamAsync(string bucket, string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var memoryStream = new MemoryStream();

            await minioClient.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
                    .WithCallbackStream(async stream => await stream.CopyToAsync(memoryStream))
            , cancellationToken);

            memoryStream.Position = 0;

            return memoryStream;
        }
        catch (BucketNotFoundException)
        {
            throw new StorageBucketNotFoundException(bucket);
        }
        catch (ObjectNotFoundException)
        {
            throw new StorageObjectNotFoundException(bucket, key);
        }
        catch (InvalidBucketNameException)
        {
            throw new StorageInvalidNameException(bucket);
        }
        catch (InvalidObjectNameException)
        {
            throw new StorageInvalidNameException(key);
        }
        catch (AuthorizationException)
        {
            throw new StorageAuthorizationException();
        }
        catch (Exception ex)
        {
            throw new StorageReadException(ex.Message);
        }
    }

    public async Task<string> GetFileUrlAsync(string bucket, string key, TimeSpan? lifetime = null)
    {
        lifetime ??= TimeSpan.FromHours(1);

        return await minioClient.PresignedGetObjectAsync(
            new PresignedGetObjectArgs()
                .WithBucket(bucket)
                .WithObject(key)
                .WithExpiry((int)lifetime.Value.TotalSeconds)
        );
    }

    public async Task<bool> ExistsAsync(string bucket, string key, CancellationToken cancellationToken)
    {
        try
        {
            await minioClient.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket(bucket)
                    .WithObject(key)
            , cancellationToken);

            return true;
        }
        catch (MinioException)
        {
            return false;
        }
    }
}
