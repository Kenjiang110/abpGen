using Bob.Abp.AppGen.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bob.Abp.AppGen.Dialogs
{
    internal class EntityKindsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var kind = (EntityKinds)value;
            var param = (EntityKinds) parameter;
            return kind.HasFlag(param);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
