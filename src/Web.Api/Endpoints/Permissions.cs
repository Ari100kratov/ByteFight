namespace Web.Api.Endpoints;

internal static class Permissions
{
    internal static class Users
    {
        public const string Access = "users:access";
    }

    internal static class Arenas
    {
        public const string Create = "arenas:create";
        public const string Edit = "arenas:edit";
        public const string Delete = "arenas:delete";
    }

    internal static class Enemies
    {
        public const string Create = "enemies:create";
        public const string Edit = "enemies:edit";
        public const string Delete = "enemies:delete";
    }
}
