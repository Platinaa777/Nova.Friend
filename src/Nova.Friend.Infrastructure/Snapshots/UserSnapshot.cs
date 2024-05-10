using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Nova.Friend.Domain.Exceptions.User;
using Nova.Friend.Domain.UserAggregate;
using Nova.Friend.Domain.UserAggregate.ValueObjects;

namespace Nova.Friend.Infrastructure.Snapshots;

public class UserSnapshot
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Key { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<string> FriendIds { get; set; } = null!;
}

public static class UserSnapshotSnapshotExtensions
{
    public static UserSnapshot Save(this User u) =>
        new()
        {
            Key = u.Id.Value,
            FirstName = u.FirstName.Value,
            LastName = u.LastName.Value,
            FriendIds = u.Friends.Select(x => x.Value).ToList()
        };
    
    public static User Restore(this UserSnapshot snapshot)
    {
        List<UserId> friends = new();
        foreach (var friendId in snapshot.FriendIds)
        {
            friends.Add(UserId.Create(friendId).Value);
        }
        
        var userSnapshotResult = User.Create(
            snapshot.Key, snapshot.FirstName, snapshot.LastName, friends);

        if (userSnapshotResult.IsFailure)
            throw new UserRestoreException(JsonConvert.SerializeObject(userSnapshotResult.Error));

        return userSnapshotResult.Value;
    }
}