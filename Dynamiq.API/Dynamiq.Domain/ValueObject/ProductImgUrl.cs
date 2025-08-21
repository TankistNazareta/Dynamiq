namespace Dynamiq.Domain.ValueObject
{
    public class ProductImgUrl
    {
        public string ImgUrl { get; private set; }

        public ProductImgUrl(string imgUrl)
        {
            ImgUrl = imgUrl;
        }
    }
}
