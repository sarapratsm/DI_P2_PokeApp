using DependencyPropertyGenerator;
using PokeData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PokeUI.UserControls
{
    [DependencyProperty<string>("stringColor", Category = "string", Description = "string del color del tipus", OnChanged = "OnStringColorChanged")]
    [DependencyProperty<string>("stringType", Category = "string", Description = "string del tipus atacant", OnChanged = "OnStringTypeChanged")]
    [DependencyProperty<string>("stringMultiplier", Category = "string", Description = "string del multiplicador", IsReadOnly =true)]
    [DependencyProperty<double>("Multiplier", DefaultValue =1.0, Category = "number", Description = "valor numeric del multiplicador", OnChanged = "OnMultiplierChanged")]

    /// <summary>
    /// Lógica de interacción para DefenseModifierAux.xaml
    /// </summary>
    public partial class DefenseModifierAux : UserControl
    {
        public DefenseModifierAux()
        {
            InitializeComponent();
        }

        public void OnStringColorChanged()
        {
            try
            {
                if (!string.IsNullOrEmpty(stringColor))
                {
                    var converter = new BrushConverter();
                    bdrType.Background = (Brush)converter.ConvertFromString(stringColor);
                }
                else
                {
                    bdrType.Background = Brushes.Gray; // Fallback si es nulo
                }
            }
            catch
            {
                bdrType.Background = Brushes.Gray; // Fallback si el formato del color es inválido
            }
        }

        public void OnStringTypeChanged()
        {
            if (!string.IsNullOrEmpty(stringType) && stringType.Length >= 3)
            {
                txbType.Text = stringType.Substring(0, 3).ToUpper();
            }
            else if (!string.IsNullOrEmpty(stringType))
            {
                txbType.Text = stringType.ToUpper();
            }
        }


        public void OnMultiplierChanged()
        {
            // Caso Inmunidad (x0) - Importante no olvidarlo
            if (Multiplier == 0)
            {
                stringMultiplier = "0";
                bdrMulti.Background = Brushes.DarkGray;
                txbMultiplier.Foreground = Brushes.White;
            }
            else if (Multiplier == 0.25)
            {
                stringMultiplier = "¼";
                bdrMulti.Background = Brushes.DarkRed;
                txbMultiplier.Foreground = Brushes.White;
            }
            else if (Multiplier == 0.5)
            {
                stringMultiplier = "½";
                // Usamos Brushes.Red o Brushes.DarkRed
                bdrMulti.Background = Brushes.DarkRed;
                txbMultiplier.Foreground = Brushes.White;
            }
            else if (Multiplier == 1)
            {
                stringMultiplier = "";
                bdrMulti.Background = Brushes.Transparent;
            }
            else // x2 i x4
            {
                stringMultiplier = Multiplier.ToString("0.##");
                bdrMulti.Background = Brushes.Green;
                txbMultiplier.Foreground = Brushes.White;
            }
        }





    }
}
