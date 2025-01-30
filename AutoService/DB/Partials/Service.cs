using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoService.DB
{
    public partial class Service
    {
        public string DiscountValue
        {
            get
            {
                if (Discount.HasValue)
                {
                    if (Discount >= 0 && Discount < 5)
                        return $"{Discount.Value}";
                    else if (Discount >= 5 && Discount < 15)
                        return $"{Discount.Value}";
                    else if (Discount >= 15 && Discount < 30)
                        return $"{Discount.Value}";
                    else if (Discount >= 30 && Discount < 70)
                        return $"{Discount.Value}";
                    else if (Discount >= 70 && Discount < 100)
                        return $"{Discount.Value}";
                }
                return string.Empty;
            }
        }

        public double NewCost
        {
            get
            {
                return Math.Round((double)(Cost - Cost * Convert.ToInt64(Discount.Value) / 100), 2);
            }
        }

        public string DurationInMinutes
        {
            get
            {
                string result = "";
                return result = $"за {Convert.ToString(DurationInMin)} минут";
            }
        }

        public double CostFormat
        {
            get
            {
                return Math.Round((double)(Cost), 2);
            }
        }
    }
}