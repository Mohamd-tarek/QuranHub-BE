namespace QuranHub.Core.Dtos.Request;

public class AddGroupRequestModel : Request
{
    public string name { get; set; }
    public List<int> versesId { get; set; }

}

