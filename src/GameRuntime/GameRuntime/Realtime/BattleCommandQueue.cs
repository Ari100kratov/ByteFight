using System.Threading.Channels;
using GameRuntime.Common.World;

namespace GameRuntime.Realtime;

/// <summary>
/// Очередь команд игрока для одной игровой сессии.
/// Боевой цикл читает команды, хаб пишет их.
/// </summary>
public sealed class BattleCommandQueue
{
    private readonly Channel<PlayerBattleCommand> channel =
        Channel.CreateUnbounded<PlayerBattleCommand>();

    /// <summary>
    /// Добавляет команду в очередь. Возвращает false, если канал закрыт.
    /// </summary>
    public bool TryPost(PlayerBattleCommand command) => channel.Writer.TryWrite(command);

    /// <summary>
    /// Ожидает следующую команду. Возвращает null, если канал закрыт
    /// или ожидание отменено.
    /// </summary>
    public async Task<PlayerBattleCommand?> ReadAsync(CancellationToken ct)
    {
        try
        {
            return await channel.Reader.ReadAsync(ct);
        }
        catch (ChannelClosedException)
        {
            return null;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
    }

    /// <summary>
    /// Закрывает канал: ожидающие читатели получат null.
    /// </summary>
    public void Complete() => channel.Writer.TryComplete();
}
