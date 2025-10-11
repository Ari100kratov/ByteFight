using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Extensions;

public static class ResultExtensions
{
    public static TOut Match<TOut>(
        this Result result,
        Func<TOut> onSuccess,
        Func<Result, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess() : onFailure(result);
    }

    public static TOut Match<TIn, TOut>(
        this Result<TIn> result,
        Func<TIn, TOut> onSuccess,
        Func<Result<TIn>, TOut> onFailure)
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result);
    }

    /// <summary>
    /// Возвращает 201 Created с Location на созданный ресурс
    /// или вызывает CustomResults.Problem при ошибке
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора ресурса</typeparam>
    /// <param name="result">Результат команды</param>
    /// <param name="getResourceUrl">Функция для формирования URL ресурса по идентификатору</param>
    public static IResult ToCreated<TId>(
        this Result<TId> result,
        Func<TId, string> getResourceUrl)
    {
        return result.Match(
            onSuccess: id => Results.Created(getResourceUrl(id), new { Id = id }),
            onFailure: CustomResults.Problem
        );
    }
}
