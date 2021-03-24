using System;
using System.ComponentModel.DataAnnotations;

namespace Project.App.Helpers
{
    public class AdvancedRequest
    {
        [TypeInt32MinValueValidation(1, ErrorMessage = "PageSizeGreaterThanOrEqual1")]
        public int PageSize { get; set; } = int.MaxValue / 2;
        [TypeInt32MinValueValidation(1, ErrorMessage = "PageNumberGreaterThanOrEqual1")]
        public int PageNumber { get; set; } = 1;
        public string OrderByQuery { get; set; } //Sample: "customerName desc, customerBirthday"
        public string SearchContent { get; set; }
    }

    public class TypeInt32MinValueValidation : ValidationAttribute
    {
        private readonly int MinValue;
        public TypeInt32MinValueValidation(int minValue)
        {
            MinValue = minValue;
        }
        public override bool IsValid(object value)
        {
            return Convert.ToInt32(value) >= MinValue;
        }
    }
}
