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
        Pokemon selectedPokemon;


        public LlistaPrincipal()
        {
            InitializeComponent();

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Cargar la lista de Pokémon (Asegúrate de incluir las relaciones si vas a filtrar)
            // Usamos Include para traer los tipos junto con el Pokemon
            var pokemons = context.Pokemons
                                  .Include(p => p.PokemonTypes)
                                  .ThenInclude(pt => pt.Type)
                                  .ToList();

            lsvPokedex.ItemsSource = pokemons;
            var llistaTipus = context.Types.ToList();
            cmbTipus.DisplayMemberPath = "TypeName";
            cmbTipus.SelectedValuePath = "TypeId";
            cmbTipus.ItemsSource = llistaTipus;

            lsvPokedex.SelectedIndex = 0;
        }

        private void btnNetejaFiltres_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lsvPokedex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedPokemon = lsvPokedex.SelectedItem as Pokemon;
            MasterDetail.ThePokemon = selectedPokemon;
        }
    }
}
