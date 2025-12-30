using System;
using System.Globalization;
using System.Windows.Data;

namespace PokeUI.Utils // O el namespace que uses
{
    public class IntToDecimalConverter : IValueConverter
    {
        // DE LA BASE DE DATOS A LA PANTALLA (Dividir / 10)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return (intValue / 10.0).ToString("0.0", CultureInfo.InvariantCulture);
            }
            return "0";
        }

        // DE LA PANTALLA A LA BASE DE DATOS (Multiplicar * 10)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string strValue)
            {
                // Intentamos leer el número (aceptando punto o coma por si acaso)
                if (double.TryParse(strValue.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    return (int)(result * 10); // Ej: 6.9 -> 69
                }
            }
            return 0;
        }
    }
}