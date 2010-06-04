namespace ViewModelGenerator
{
    using System.ComponentModel;
    using System.Globalization;

    [TypeConverter(typeof(EntityConverter))]
    public abstract class EntityBase
    {
        public int Id { get; set; }

        public override string ToString()
        {
            return Id.ToString(CultureInfo.CurrentCulture);
        }
    }
}