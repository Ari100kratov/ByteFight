using Application.Abstractions.Messaging;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Application.Abstractions.Behaviors;

internal static partial class LoggingDecorator
{
    internal sealed partial class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogProcessingCommand(commandName);

            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                LogCompletedCommand(commandName);
            }
            else
            {
                var data = new Dictionary<string, object>
                {
                    ["Error"] = result.Error
                };
                using (logger.BeginScope(data))
                {
                    logger.LogError("Completed command {Command} with error", commandName);
                }
            }

            return result;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Processing command {Command}")]
        private partial void LogProcessingCommand(string command);

        [LoggerMessage(Level = LogLevel.Information, Message = "Completed command {Command}")]
        private partial void LogCompletedCommand(string command);
    }

    internal sealed partial class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            string commandName = typeof(TCommand).Name;

            LogProcessingCommand(commandName);

            Result result = await innerHandler.Handle(command, cancellationToken);

            if (result.IsSuccess)
            {
                LogCompletedCommand(commandName);
            }
            else
            {
                var data = new Dictionary<string, object>
                {
                    ["Error"] = result.Error
                };
                using (logger.BeginScope(data))
                {
                    logger.LogError("Completed command {Command} with error", commandName);
                }
            }

            return result;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Processing command {Command}")]
        private partial void LogProcessingCommand(string command);

        [LoggerMessage(Level = LogLevel.Information, Message = "Completed command {Command}")]
        private partial void LogCompletedCommand(string command);
    }

    internal sealed partial class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            string queryName = typeof(TQuery).Name;

            LogProcessingQuery(queryName);

            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if (result.IsSuccess)
            {
                LogCompletedQuery(queryName);
            }
            else
            {
                var data = new Dictionary<string, object>
                {
                    ["Error"] = result.Error
                };
                using (logger.BeginScope(data))
                {
                    logger.LogError("Completed query {Query} with error", queryName);
                }
            }

            return result;
        }

        [LoggerMessage(Level = LogLevel.Information, Message = "Processing query {Query}")]
        private partial void LogProcessingQuery(string query);

        [LoggerMessage(Level = LogLevel.Information, Message = "Completed query {Query}")]
        private partial void LogCompletedQuery(string query);
    }
}
