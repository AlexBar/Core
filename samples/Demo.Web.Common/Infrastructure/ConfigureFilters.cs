namespace Demo.Web
{
    using MvcExtensions;

    public class ConfigureFilters : ConfigureFiltersBase
    {
        public ConfigureFilters(IFilterRegistry registry) : base(registry)
        {
        }

        protected override void Configure()
        {
            Registry.Register<ProductController, PopulateCategoriesAttribute, PopulateSuppliersAttribute>(c => c.Create())
                    .Register<ProductController, PopulateCategoriesAttribute, PopulateSuppliersAttribute>(c => c.Create(null))
                    .Register<ProductController, PopulateCategoriesAttribute, PopulateSuppliersAttribute>(c => c.Edit(0))
                    .Register<ProductController, PopulateCategoriesAttribute, PopulateSuppliersAttribute>(c => c.Edit(null));
        }
    }
}