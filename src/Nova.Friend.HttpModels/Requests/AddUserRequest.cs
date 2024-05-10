namespace Nova.Friend.HttpModels.Requests;

public class AddUserRequest
{
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}