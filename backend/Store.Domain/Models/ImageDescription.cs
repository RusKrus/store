namespace Store.Domain.Models;

public class ImageDescription
{
    private ImageDescription() {}

    public ImageDescription(string imageKey, string description = "")
    {
        Guid = new Guid();
        ImageKey = imageKey;
        Description = description;
    }


    public int Id { get; init;  }
    public Guid Guid { get; private set; }
    public string? Description { get; private set; } = "";
    public string ImageKey { get; private set; }
}