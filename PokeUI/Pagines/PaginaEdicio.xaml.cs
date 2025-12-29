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
            
            //Utils.Carregas.carregarTipus(context, cmbType2);
            
            
        }

        private void cmbType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
