using Application.Abstractions.Authentication;

using Application.Auth.Users.ChangePassword;
using SharedKernel;
using SharedKernel.Messaging;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Auth.Users;

internal sealed class ChangeCurrentUserPassword : IEndpoint
{
    public sealed record Request(string CurrentPassword, string NewPassword);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("users/me/password", async (
            Request request,
            IUserContext userContext,
            ICommandHandler<ChangeUserPasswordCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangeUserPasswordCommand(
                userContext.UserId,
                request.CurrentPassword,
                request.NewPassword);

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users);
    }
}
