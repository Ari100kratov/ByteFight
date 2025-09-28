using Application.Abstractions.Messaging;
using Domain.Todos;
using SharedKernel;

namespace Application.Todos.GetById;

internal sealed class GetTodoByIdQueryHandler
    : IQueryHandler<GetTodoByIdQuery, TodoResponse>
{
    public Task<Result<TodoResponse>> Handle(GetTodoByIdQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(Result.Failure<TodoResponse>(TodoItemErrors.NotFound(query.TodoItemId)));

        //TodoResponse? todo = await context.TodoItems
        //    .Where(todoItem => todoItem.Id == query.TodoItemId && todoItem.UserId == userContext.UserId)
        //    .Select(todoItem => new TodoResponse
        //    {
        //        Id = todoItem.Id,
        //        UserId = todoItem.UserId,
        //        Description = todoItem.Description,
        //        DueDate = todoItem.DueDate,
        //        Labels = todoItem.Labels,
        //        IsCompleted = todoItem.IsCompleted,
        //        CreatedAt = todoItem.CreatedAt,
        //        CompletedAt = todoItem.CompletedAt
        //    })
        //    .SingleOrDefaultAsync(cancellationToken);

        //if (todo is null)
        //{
        //    return Result.Failure<TodoResponse>(TodoItemErrors.NotFound(query.TodoItemId));
        //}

        //return todo;
    }
}
