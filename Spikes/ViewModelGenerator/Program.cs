namespace ViewModelGenerator
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public static class Program
    {
        public static void Main()
        {
            var factory = new ViewModelFactory();

            var displayModelType = factory.CreateDisplayModel(typeof(Product));
            var editModelType = factory.CreateEditModel(typeof(Product));

            Console.WriteLine("DisplayModel Type: {0} ", displayModelType.FullName);
            Console.WriteLine("EditModel Type: {0} ", editModelType.FullName);

            var displayModelAttribute = displayModelType.GetCustomAttributes(typeof(TypeConverterAttribute), false).OfType<TypeConverterAttribute>().SingleOrDefault();
            var editModelAttribute = editModelType.GetCustomAttributes(typeof(TypeConverterAttribute), false).OfType<TypeConverterAttribute>().SingleOrDefault();

            Console.WriteLine("DisplayModel Attribute: {0} ", ((displayModelAttribute != null) && displayModelAttribute.ConverterTypeName.Contains("EntityConverter")) ? "OK" : "ERROR");
            Console.WriteLine("EditModel Attribute: {0} ", ((editModelAttribute != null) && editModelAttribute.ConverterTypeName.Contains("EntityConverter")) ? "OK" : "ERROR");

            var displayModel = Activator.CreateInstance(displayModelType);
            var editModel = Activator.CreateInstance(editModelType);

            Console.WriteLine("DisplayModel Created: {0} ", (displayModel != null) ? "OK" : "ERROR");
            Console.WriteLine("EditModel Created: {0} ", (editModel != null) ? "OK" : "ERROR");

            displayModel.GetType().GetProperty("Id").SetValue(displayModel, 10, null);
            editModel.GetType().GetProperty("Id").SetValue(editModel, 10, null);

            Console.WriteLine("DisplayModel ToString: {0} ", (displayModel.ToString().Equals("10")) ? "OK" : "ERROR");
            Console.WriteLine("EditModel ToString: {0} ", (editModel.ToString().Equals("10")) ? "OK" : "ERROR");

            Console.Read();
        }
    }
}