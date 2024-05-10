namespace Nova.Friend.HttpModels.Requests;

public class DeleteFriend
{
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
}