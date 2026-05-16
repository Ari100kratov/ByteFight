using Application.Abstractions.Authentication;

using Application.Auth.Users.UpdateProfile;
using SharedKernel;
using SharedKernel.Messaging;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth.Users;

internal sealed class UpdateCurrentUserProfile : IEndpoint
{
    public sealed record Request(string Email, string FirstName, string LastName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/me/profile", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<UpdateUserProfileCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateUserProfileCommand(
                userContext.UserId,
                request.Email,
                request.FirstName,
                request.LastName);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
