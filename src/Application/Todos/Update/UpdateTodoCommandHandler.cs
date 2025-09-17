using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Todos;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Todos.Update;

internal sealed class UpdateTodoCommandHandler
    : ICommandHandler<UpdateTodoCommand>
{
    public Task<Result> Handle(UpdateTodoCommand command, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Success());

        //TodoItem? todoItem = await context.TodoItems
        //    .SingleOrDefaultAsync(t => t.Id == command.TodoItemId, cancellationToken);

        //if (todoItem is null)
        //{
        //    return Result.Failure(TodoItemErrors.NotFound(command.TodoItemId));
        //}

        //todoItem.Description = command.Description;

        //await context.SaveChangesAsync(cancellationToken);
        
        //return Result.Success();
    }
}
