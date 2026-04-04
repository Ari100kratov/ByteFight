namespace Application.Abstractions.Authorization;

public static class Permissions
{
    public static class Users
    {
        public const string Access = "users:access";
    }

    public static class Arenas
    {
        public const string Create = "arenas:create";
        public const string Edit = "arenas:edit";
        public const string Delete = "arenas:delete";
    }

    public static class Enemies
    {
        public const string Create = "enemies:create";
        public const string Edit = "enemies:edit";
        public const string Delete = "enemies:delete";
    }

    public static class CharacterClasses
    {
        public const string Create = "character-classes:create";
        public const string Edit = "character-classes:edit";
        public const string Delete = "character-classes:delete";
    }

    public static readonly string[] All =
    [
        Users.Access,

        Arenas.Create,
        Arenas.Edit,
        Arenas.Delete,

        Enemies.Create,
        Enemies.Edit,
        Enemies.Delete,

        CharacterClasses.Create,
        CharacterClasses.Edit,
        CharacterClasses.Delete
    ];
}
