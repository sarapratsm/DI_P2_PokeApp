using System.Windows.Media;

namespace PokeUI.Utils
{
    public static class UIUtils
    {
        // Convierte un string HEX (#FF0000) a Brush de WPF de forma segura
        public static Brush GetBrushFromHex(string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor)) return Brushes.White;

            try
            {
                var converter = new BrushConverter();
                return (Brush)converter.ConvertFromString(hexColor);
            }
            catch
            {
                return Brushes.White; // Color por defecto si falla
            }
        }
    }
}