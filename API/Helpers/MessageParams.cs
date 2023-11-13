namespace API.Helpers;

public class MessageParams
{
    public string UserName { get; set; }
    public string Container { get; set; } = "Unread";
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
