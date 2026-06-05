using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace BeautySalonWpf
{
    public class PriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 &&
                values[0] is int totalSum &&
                values[1] is int rate)
            {
                double rateInPercentage = (double)rate / 100;
                return $"{totalSum * rateInPercentage}  ₽";
            }
            if (values.Length == 1 && values[0] is int priceValue) 
            {
                return $"{priceValue} ₽";
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
