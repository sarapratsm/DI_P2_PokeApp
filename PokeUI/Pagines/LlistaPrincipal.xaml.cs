using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PokeData.Models;
using PokeUI.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
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

namespace PokeUI.Pagines
{
    /// <summary>
    /// Lógica de interacción para LlistaPrincipal.xaml
    /// </summary>
    public partial class LlistaPrincipal : Page
    {
        private AppDbContext context = new AppDbContext();
        //Pokemon selectedPokemon;


        public LlistaPrincipal()
        {
            InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                carregarPokemons();
                carregarTipus();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en carregar els pokémons des de la base de dades: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        /// <summary>
        /// Mètode per carregar els tipus de pokémon dins del combobox i fer la configuració inicial.
        /// (Aquest mètode no és realment necessari però així quedava més entenedor i net.)
        /// </summary>
        private void carregarTipus()
        {
            var llistaTipus = context.Types.ToList();
            cmbTipus.DisplayMemberPath = "TypeName";
            cmbTipus.SelectedValuePath = "TypeId";
            cmbTipus.ItemsSource = llistaTipus;
        }

        /// <summary>
        /// Mètode per carregar els pokémon des de la base de dades i els entra a la llista lsvPokedex, .
        /// </summary>
        private void carregarPokemons()
        {
            var pokemons = context.Pokemons
                                              .Include(p => p.PokemonTypes)
                                              .ThenInclude(pt => pt.Type)
                                              .Include(p => p.BaseStat)
                                              .Include(p => p.PokemonAbilities).ThenInclude(pa => pa.Abil)
                                              .ToList();
            lsvPokedex.ItemsSource = pokemons;
            lsvPokedex.SelectedIndex = 0;


        }

        private void btnNetejaFiltres_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lsvPokedex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //selectedPokemon = lsvPokedex.SelectedItem as Pokemon;
            //MasterDetail.ThePokemon = selectedPokemon;
        }
    }
}
