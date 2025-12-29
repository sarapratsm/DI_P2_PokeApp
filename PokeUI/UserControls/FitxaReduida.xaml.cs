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
using DependencyPropertyGenerator;
using System.Windows.Controls;
using PokeData.Models;

#nullable enable




namespace PokeUI.UserControls
{

    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged = "OnPokemonChanged")]

    /// <summary>
    /// Lógica de interacción para FitxaReduida.xaml
    /// </summary>
    public partial class FitxaReduida : UserControl
    {
        public FitxaReduida()
        {
            InitializeComponent();

        }

        private void Fitxa_Loaded(object sender, RoutedEventArgs e)
        {
            generarEtiquetesTipus();
        }

        private void generarEtiquetesTipus()
        {
            stkTypes.Children.Clear();

            if (ThePokemon.PokemonTypes == null) return;

            foreach (var pt in ThePokemon.PokemonTypes)
            {
                if (pt.Type == null) continue;

                Border borde = new Border();
                borde.CornerRadius = new CornerRadius(3);
                borde.Margin = new Thickness(0, 0, 5, 0);
                borde.Padding = new Thickness(8, 2, 8, 2);

                try
                {
                    if (!string.IsNullOrEmpty(pt.Type.Color))
                    {
                        var converter = new BrushConverter();
                        borde.Background = (Brush)converter.ConvertFromString(pt.Type.Color);
                    }
                    else
                    {
                        borde.Background = Brushes.Gray; // Fallback si es nulo
                    }
                }
                catch
                {
                    borde.Background = Brushes.Gray; // Fallback si el formato del color es inválido
                }

                TextBlock texto = new TextBlock();
                texto.Text = pt.Type.TypeName; // El nombre del tipo
                texto.Foreground = Brushes.White;
                texto.FontSize = 11;
                texto.FontWeight = FontWeights.SemiBold;

                borde.Child = texto;          // Meter texto en borde
                stkTypes.Children.Add(borde); // Meter borde en el panel XAML
            }
        }

        public void OnPokemonChanged()
        {
            generarEtiquetesTipus();
        }
    }
}
