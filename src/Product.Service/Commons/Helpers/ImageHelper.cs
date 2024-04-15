namespace Product.Service.Commons.Helpers
{
    public class ImageHelper
    {
        public static string UniqueName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string imageName = "IMG_" + Guid.NewGuid().ToString();
            return imageName + extension;
        }
    }
}
