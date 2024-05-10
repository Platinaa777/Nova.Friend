namespace Nova.Friend.HttpModels.Requests;

public class SendRequestToFriend
{
    public string SenderId { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
}