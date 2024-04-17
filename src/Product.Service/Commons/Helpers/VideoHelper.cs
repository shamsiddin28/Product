namespace Product.Service.Commons.Helpers;

public class VideoHelper
{
    public static string UniqueName(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        string imageName = "VIDEO_" + Guid.NewGuid().ToString();
        return imageName + extension;
    }
}
