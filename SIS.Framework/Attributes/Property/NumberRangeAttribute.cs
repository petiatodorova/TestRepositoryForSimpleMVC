using System.ComponentModel.DataAnnotations;

namespace SIS.Framework.Attributes.Property
{
    class NumberRangeAttribute : ValidationAttribute
    {
        private readonly double minValue;

        private readonly double maxValue;

        public NumberRangeAttribute(
                double minValue = double.MinValue,
                double maxValue = double.MaxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            return this.minValue <= (double)value 
                    && (double)value <= this.maxValue;
        }
    }
}
