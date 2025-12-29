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
using DependencyPropertyGenerator;
using PokeUI.Utils;
using Type = PokeData.Models.Type;

namespace PokeUI.Pagines
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged = "OnThePokemonChanged")]

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
                Utils.Carregas.carregarTipus(context, cmbTipus);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en carregar els pokémons des de la base de dades: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }



        }

        ///// <summary>
        ///// Mètode per carregar els tipus de pokémon dins del combobox i fer la configuració inicial.
        ///// (Aquest mètode no és realment necessari però així quedava més entenedor i net.)
        ///// </summary>
        //public void carregarTipus(ComboBox cb)
        //{
        //    var llistaTipus = context.Types.ToList();
        //    cb.DisplayMemberPath = "TypeName";
        //    cb.SelectedValuePath = "TypeId";
        //    cb.ItemsSource = llistaTipus;
        //}

        /// <summary>
        /// Mètode per carregar els pokémon des de la base de dades i els entra a la llista lsvPokedex, .
        /// </summary>
        private void carregarPokemons()
        {
            string filtreNom = null;
            string filtreTipus = null;

            if (txbFiltreNom.Text.Trim() != string.Empty)
            {
                filtreNom = txbFiltreNom.Text.Trim();
            }
            if (cmbTipus.SelectedIndex != -1)
            {
                Type tipusFiltre=cmbTipus.SelectedItem as Type;
                filtreTipus=tipusFiltre.TypeName;
            }
            
            // 2. Empezamos la consulta como "Queryable" (aún no se ejecuta en BD)
            var query = context.Pokemons.AsQueryable();

            // 3. Añadimos los Includes necesarios (igual que antes)
            query = query.Include(p => p.PokemonTypes).ThenInclude(pt => pt.Type)
                         .Include(p => p.BaseStat)
                         .Include(p => p.PokemonAbilities).ThenInclude(pa => pa.Abil);

            // 4. APLICAR FILTRO DE NOMBRE (si nos han pasado uno)
            if (!string.IsNullOrEmpty(filtreNom))
            {
                // Usamos Contains para que busque "Pika" y encuentre "Pikachu"
                query = query.Where(p => p.PokName.Contains(filtreNom));
            }

            // 5. APLICAR FILTRO DE TIPO (si nos han pasado uno)
            if (!string.IsNullOrEmpty(filtreTipus))
            {
                // Buscamos Pokémon que tengan AL MENOS UN tipo que coincida con el nombre
                query = query.Where(p => p.PokemonTypes.Any(pt => pt.Type.TypeName == filtreTipus));
            }

            // 6. Al final hacemos el ToList(), que es cuando realmente se pide la info a la BD
            lsvPokedex.ItemsSource = query.ToList();

            // Validamos que haya resultados antes de seleccionar el primero
            if (lsvPokedex.Items.Count > 0)
            {
                lsvPokedex.SelectedIndex = 0;
            }

            //var pokemons = context.Pokemons
            //                                  .Include(p => p.PokemonTypes)
            //                                  .ThenInclude(pt => pt.Type)
            //                                  .Include(p => p.BaseStat)
            //                                  .Include(p => p.PokemonAbilities).ThenInclude(pa => pa.Abil)
            //                                  .ToList();
            //lsvPokedex.ItemsSource = pokemons;
            //lsvPokedex.SelectedIndex = 0;


        }

        private void btnNetejaFiltres_Click(object sender, RoutedEventArgs e)
        {
            cmbTipus.SelectedIndex = -1;
            carregarPokemons();
        }

        private void lsvPokedex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThePokemon = lsvPokedex.SelectedItem as Pokemon;
        }

        public void OnThePokemonChanged()
        {

        }

        private void cmbTipus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipus.SelectedItem == null) return;
            Type selected = cmbTipus.SelectedItem as Type;

            if (selected != null && !string.IsNullOrEmpty(selected.Color))
            {
                try
                {
                    var converter = new BrushConverter();
                    var typeColor = (Brush)converter.ConvertFromString(selected.Color);

                    cmbTipus.Background = typeColor;
                }
                catch (Exception ex)
                {
                    {
                        cmbTipus.Background = Brushes.White;
                    }
                }
            }
            filtrarlsv();
        }

        private void filtrarlsv()
        {
            carregarPokemons();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            carregarPokemons();
        }
    }
}
