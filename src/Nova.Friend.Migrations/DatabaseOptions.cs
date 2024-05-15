namespace Nova.Friend.Migrations;

public static class DatabaseOptions
{
    public const string DatabaseName = "friend-database";
    public const string UserCollection = "UserSnapshot";
    public const string RequestCollection = "FriendRequestSnapshot";
    public const string OutboxMessage = "OutboxMessage";
    public const string RelationGraph = "Relations";
    public const string FriendEdge = "Friends";
}