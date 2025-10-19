using SharedKernel;

namespace Domain.Game.Enemies;

public static class EnemyErrors
{
    public static Error NotFound(Guid enemyId) => Error.NotFound(
        "Enemies.NotFound",
        $"¬раг с Id = '{enemyId}' не найден");
}
