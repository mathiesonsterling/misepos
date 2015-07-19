using System;

using Xamarin.Forms;

namespace Mise.POSTerminal.Converters
{
	public class NotConverter:IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value.GetType() == typeof(bool)) {
				return !((bool)value);
			}
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}