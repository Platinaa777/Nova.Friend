using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Nova.Friend.Domain.Exceptions.FriendRequest;
using Nova.Friend.Domain.FriendRequestAggregate;

namespace Nova.Friend.Infrastructure.Snapshots;

public class FriendRequestSnapshot
{
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Id { get; set; } = string.Empty;

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Key { get; set; } = string.Empty;
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public static class FriendRequestSnapshotExtensions
{
    public static FriendRequestSnapshot Save(this FriendRequest fr) =>
        new()
        {
            Key = fr.Id.Value,
            SenderId = fr.SenderId.Value,
            ReceiverId = fr.ReceiverId.Value,
            Status = fr.Status.Name
        };
    
    public static FriendRequest Restore(this FriendRequestSnapshot snapshot)
    {
        var friendRequestResult = FriendRequest.Create(
            snapshot.Key, snapshot.SenderId, snapshot.ReceiverId, snapshot.Status);

        if (friendRequestResult.IsFailure)
            throw new FriendRequestRestoreException(JsonConvert.SerializeObject(friendRequestResult.Error));

        return friendRequestResult.Value;
    }
}