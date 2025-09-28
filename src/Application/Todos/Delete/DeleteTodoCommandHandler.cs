using Application.Abstractions.Messaging;
using SharedKernel;

namespace Application.Todos.Delete;

internal sealed class DeleteTodoCommandHandler
    : ICommandHandler<DeleteTodoCommand>
{
    public Task<Result> Handle(DeleteTodoCommand command, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());

        //TodoItem? todoItem = await context.TodoItems
        //    .SingleOrDefaultAsync(t => t.Id == command.TodoItemId && t.UserId == userContext.UserId, cancellationToken);

        //if (todoItem is null)
        //{
        //    return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        //}

        //context.TodoItems.Remove(todoItem);

        //todoItem.Raise(new TodoItemDeletedDomainEvent(todoItem.Id));

        //await context.SaveChangesAsync(cancellationToken);

        //return Result.Success();
    }
}
