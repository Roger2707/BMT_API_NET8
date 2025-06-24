using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Store_API.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Created")]
        Created = 0,

        [Display(Name = "Prepared")]
        Prepared = 1,

        [Display(Name = "Shipping")]
        Shipping = 2,

        [Display(Name = "Shipped")]
        Shipped = 3,

        [Display(Name = "Completed")]
        Completed = 4,

        [Display(Name = "Cancelled")]
        Cancelled = 5,

        [Display(Name = "BackAndRefund")]
        BackAndRefund = 6,
    }

    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                .GetMember(enumValue.ToString())[0]
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? enumValue.ToString();
        }
    }
}
