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
using DependencyPropertyGenerator;
using Type = PokeData.Models.Type;

namespace PokeUI.Pagines
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged = "OnThePokemonChanged")]

    /// <summary>
    /// Lógica de interacción para PaginaEdicio.xaml
    /// </summary>
    public partial class PaginaEdicio : Page
    {
        AppDbContext context = new AppDbContext();
        public PaginaEdicio()
        {
            InitializeComponent();
        }

        public void OnThePokemonChanged()
        {
            if (ThePokemon == null || context == null) return;

            
        }
        

        private void validatePokemon()
        {
            /*Tots els camps són obligatoris, i han de tenir un caràcter diferent d’espai com a mínim.
                Com a mínim, ha de tenir un tipus.
                Els camps numèrics han de validar-se dins un rang raonable.
                Si hi ha qualsevol camp erroni, el botó desar es deshabilita i es mostra el fons del Textbox de color
                vermell suau.*/
        }

        

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Utils.Carregas.carregarTipus(context, cmbType1);

            if (ThePokemon == null || context == null) return;
            
            var pt = ThePokemon.PokemonTypes.FirstOrDefault();

            if (pt != null && pt.Type != null)
            {
                foreach (var item in cmbType1.Items)
                {
                    var type1 = item as PokeData.Models.Type;

                    if (type1 != null && type1.TypeId == pt.Type.TypeId)
                    {
                        cmbType1.SelectedItem = item;
                        break;
                    }
                }
            }


        }

        private void cmbType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = cmbType1.SelectedItem as PokeData.Models.Type;

            if (selectedType != null && !string.IsNullOrEmpty(selectedType.Color))
            {
                try
                {
                    var converter = new BrushConverter();
                    var brush = (Brush)converter.ConvertFromString(selectedType.Color);

                    cmbType1.Background = brush;
                }
                catch(Exception ex)
                {
                    cmbType1.Background = Brushes.White;
                }
            }
        }
    }
}
