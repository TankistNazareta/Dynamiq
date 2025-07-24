namespace Dynamiq.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string Slug { get; private set; }

        public Guid? ParentCategoryId { get; private set; }

        public Category? ParentCategory { get; private set; }

        private readonly List<Category> _subCategories = new();
        public IReadOnlyCollection<Category> SubCategories => _subCategories.AsReadOnly();

        private readonly List<Product> _products = new();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        private Category() { }

        public Category(string name, Guid? parentCategoryId = null)
        {
            Id = Guid.NewGuid();
            Name = name;
            Slug = name.Trim().ToLower().Replace(" ", "-"); ;
            ParentCategoryId = parentCategoryId;
        }

        public void AddSubCategory(Category subCategory)
        {
            _subCategories.Add(subCategory);
        }

        public void AddProduct(Product product)
        {
            _products.Add(product);
        }
    }
}