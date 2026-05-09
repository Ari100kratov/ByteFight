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

    public static class ArenaEnemies
    {
        public const string Add = "arena-enemies:add";
        public const string Remove = "arena-enemies:remove";
    }

    public static class Enemies
    {
        public const string Create = "enemies:create";
        public const string Edit = "enemies:edit";
        public const string EditName = "enemies:edit:name";
        public const string EditDescription = "enemies:edit:description";
        public const string EditStats = "enemies:edit:stats";
        public const string EditActionAssets = "enemies:edit:action-assets";
        public const string EditAbilities = "enemies:edit:abilities";
        public const string Delete = "enemies:delete";
    }

    public static class CharacterClasses
    {
        public const string Create = "character-classes:create";
        public const string Edit = "character-classes:edit";
        public const string Delete = "character-classes:delete";
    }

    public static class CharacterSpecs
    {
        public const string Create = "character-specs:create";
        public const string Edit = "character-specs:edit";
        public const string EditName = "character-specs:edit:name";
        public const string EditDescription = "character-specs:edit:description";
        public const string EditPortrait = "character-specs:edit:portrait";
        public const string EditStats = "character-specs:edit:stats";
        public const string EditActionAssets = "character-specs:edit:action-assets";
        public const string EditAbilities = "character-specs:edit:abilities";
        public const string Delete = "character-specs:delete";
    }

    public static readonly string[] All =
    [
        Users.Access,

        Arenas.Create,
        Arenas.Edit,
        Arenas.Delete,

        ArenaEnemies.Add,
        ArenaEnemies.Remove,

        Enemies.Create,
        Enemies.Edit,
        Enemies.EditName,
        Enemies.EditDescription,
        Enemies.EditStats,
        Enemies.EditActionAssets,
        Enemies.EditAbilities,
        Enemies.Delete,

        CharacterClasses.Create,
        CharacterClasses.Edit,
        CharacterClasses.Delete,

        CharacterSpecs.Create,
        CharacterSpecs.Edit,
        CharacterSpecs.EditName,
        CharacterSpecs.EditDescription,
        CharacterSpecs.EditPortrait,
        CharacterSpecs.EditStats,
        CharacterSpecs.EditActionAssets,
        CharacterSpecs.EditAbilities,
        CharacterSpecs.Delete
    ];
}
