using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PokeUI.Utils
{
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string colorHex = value as string;

            // Si es nulo o vacío, devolvemos gris
            if (string.IsNullOrEmpty(colorHex)) return Brushes.Gray;

            try
            {
                // Si le falta la almohadilla, se la ponemos
                if (!colorHex.StartsWith("#"))
                {
                    colorHex = "#" + colorHex;
                }

                // Convertimos a Color real
                return (SolidColorBrush)(new BrushConverter().ConvertFrom(colorHex));
            }
            catch
            {
                return Brushes.Gray; // Si falla, gris por seguridad
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}