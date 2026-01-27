using DependencyPropertyGenerator;
using Microsoft.EntityFrameworkCore;
using PokeData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Type = PokeData.Models.Type;

namespace PokeUI.Pagines
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El Pokémon seleccionat actualment", OnChanged = "OnThePokemonChanged")]

    /// <summary>
    /// Pàgina principal que mostra la llista de Pokémon (Pokedex).
    /// Gestiona la visualització, filtratge, navegació a detalls i eliminació segura.
    /// </summary>
    public partial class LlistaPrincipal : Page
    {
        // Context de la base de dades
        private AppDbContext context = new AppDbContext();

        public LlistaPrincipal()
        {
            InitializeComponent();
        }

        #region Inicialització i Càrrega

        /// <summary>
        /// S'executa quan la pàgina es carrega completament.
        /// Inicialitza la càrrega de dades i els desplegables.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                carregarPokemons();

                // Càrrega del ComboBox de tipus utilitzant la classe d'utilitats
                Utils.Carregas.carregarTipus(context, cmbTipus);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en carregar els pokémons des de la base de dades. Comprova que la base de dades és accessible. Es tancarà l'aplicació.",
                                "Error Crític", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Mètode principal per recuperar els Pokémon de la base de dades.
        /// Aplica els filtres de nom/codi i tipus seleccionats a la interfície.
        /// </summary>
        private void carregarPokemons()
        {
            // 1. Recollida de valors dels filtres
            string filtreNomCodi = null;
            string filtreTipus = null;

            if (txbFiltreNom.Text.Trim() != string.Empty)
            {
                filtreNomCodi = txbFiltreNom.Text.Trim();
            }
            if (cmbTipus.SelectedIndex != -1)
            {
                Type tipusFiltre = cmbTipus.SelectedItem as Type;
                if (tipusFiltre != null)
                {
                    filtreTipus = tipusFiltre.TypeName;
                }
            }

            // 2. Iniciem la consulta com a "Queryable" (encara no s'executa a la BD, permetent optimització)
            var query = context.Pokemons.AsQueryable();

            // 3. Afegim els Includes necessaris per portar dades relacionades (Eager Loading)
            query = query.Include(p => p.PokemonTypes).ThenInclude(pt => pt.Type)
                         .Include(p => p.BaseStat)
                         .Include(p => p.PokemonAbilities).ThenInclude(pa => pa.Abil);

            // 4. APLICAR FILTRE DE NOM O CODI
            if (!string.IsNullOrEmpty(filtreNomCodi))
            {
                int codi = 0;
                bool esCodi = Int32.TryParse(filtreNomCodi, out codi);

                if (esCodi)
                {
                    // Si és un número, busquem per ID
                    query = query.Where(p => p.PokId.ToString().StartsWith(filtreNomCodi));
                }
                else
                {
                    // Si és text, busquem pel nom del Pokémon
                    query = query.Where(p => p.PokName.Contains(filtreNomCodi));
                }
            }

            // 5. APLICAR FILTRE DE TIPUS
            if (!string.IsNullOrEmpty(filtreTipus))
            {
                // Busquem Pokémon que tinguin ALMENYS UN tipus que coincideixi amb el filtre
                query = query.Where(p => p.PokemonTypes.Any(pt => pt.Type.TypeName == filtreTipus));
            }

            // 6. Execució final: ToList() envia la consulta SQL a la base de dades
            lsvPokedex.ItemsSource = query.ToList();

            // Seleccionem el primer element per defecte si n'hi ha
            if (lsvPokedex.Items.Count > 0)
            {
                lsvPokedex.SelectedIndex = 0;
            }
        }

        #endregion

        #region Gestió de Filtres i UI

        /// <summary>
        /// Neteja els filtres de cerca i recarrega la llista completa.
        /// </summary>
        private void btnNetejaFiltres_Click(object sender, RoutedEventArgs e)
        {
            cmbTipus.SelectedIndex = -1;
            txbFiltreNom.Text = "";
            carregarPokemons();
        }

        /// <summary>
        /// Gestiona el canvi de selecció a la llista visual.
        /// Actualitza la propietat 'ThePokemon' per mostrar els detalls al panell lateral.
        /// </summary>
        private void lsvPokedex_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThePokemon = lsvPokedex.SelectedItem as Pokemon;
        }

        /// <summary>
        /// Callback automàtic quan canvia la propietat ThePokemon.
        /// </summary>
        public void OnThePokemonChanged()
        {
            // Mètode necessari per la DependencyProperty, encara que estigui buit de moment.
        }

        /// <summary>
        /// Gestiona el canvi de tipus al desplegable.
        /// Canvia el color de fons del combo segons el tipus i aplica el filtre.
        /// </summary>
        private void cmbTipus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbTipus.SelectedItem == null) return;
            Type selected = cmbTipus.SelectedItem as Type;

            // Canvi estètic del color de fons
            if (selected != null && !string.IsNullOrEmpty(selected.Color))
            {
                try
                {
                    var converter = new BrushConverter();
                    var typeColor = (Brush)converter.ConvertFromString(selected.Color);
                    cmbTipus.Background = typeColor;
                }
                catch
                {
                    cmbTipus.Background = Brushes.White;
                }
            }

            // Apliquem el filtre
            filtrarlsv();
        }

        /// <summary>
        /// Mètode auxiliar per refrescar la llista (wrapper de carregarPokemons).
        /// </summary>
        private void filtrarlsv()
        {
            carregarPokemons();
        }

        /// <summary>
        /// Es dispara quan l'usuari escriu al quadre de cerca. Refresca la llista en temps real.
        /// </summary>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            carregarPokemons();
        }

        /// <summary>
        /// Gestió del doble clic a la llista (Pendent d'implementar navegació directa).
        /// </summary>
        private void lsvPokedex_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // TODO: Hauríem de cridar la MainWindow per poder navegar o obrir l'edició directament.
            // De moment es pot fer servir el botó d'editar.
            btnEdit_Click(sender, e);
        }

        #endregion

        #region Operacions CRUD (Eliminar, Editar, Afegir)

        /// <summary>
        /// Elimina el Pokémon seleccionat i totes les seves dades relacionades manualment.
        /// Realitza una neteja profunda per evitar errors de restricció de claus foranes (Foreign Keys).
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Pokemon pokemonAEliminar;

            // Verifiquem que hi ha selecció
            if (lsvPokedex.SelectedItem != null)
            {
                pokemonAEliminar = (Pokemon)lsvPokedex.SelectedItem;

                // Demanem confirmació a l'usuari
                var resultat = MessageBox.Show(
                    $"Estàs segur que vols eliminar a {pokemonAEliminar.PokName}?\n\nAquesta acció no es pot desfer.",
                    "Confirmar eliminació",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (resultat == MessageBoxResult.Yes)
                {
                    try
                    {
                        int id = pokemonAEliminar.PokId;

                        // -----------------------------------------------------------------
                        // FASE 1: NETEJA DE DEPENDÈNCIES (Esborrat Manual)
                        // -----------------------------------------------------------------

                        // A. Esborrar Tipus associats
                        var tipusAsociats = context.PokemonTypes.Where(t => t.PokId == id).ToList();
                        context.PokemonTypes.RemoveRange(tipusAsociats);

                        // B. Esborrar Habilitats associades
                        var habsAsociades = context.PokemonAbilities.Where(a => a.PokId == id).ToList();
                        context.PokemonAbilities.RemoveRange(habsAsociades);

                        // C. Esborrar Estadístiques Base
                        // Nota: Si la BD fa servir una taula 'pokemon_stats' separada, cal netejar-la explícitament.
                        var statsAsociades = context.BaseStats.Where(s => s.PokId == id).ToList();
                        context.BaseStats.RemoveRange(statsAsociades);

                        // D. Gestió d'Evolucions (Crític per evitar cicles o errors)

                        // D1. Si jo sóc FILL d'algú -> Esborro la meva fitxa de fill
                        var fitxaEvo = context.PokemonEvolutionMatchups.FirstOrDefault(m => m.PokId == id);
                        if (fitxaEvo != null) context.PokemonEvolutionMatchups.Remove(fitxaEvo);

                        // D2. Si jo sóc PARE d'altres -> Els deixem orfes (posar a null) en lloc d'esborrar-los
                        var fills = context.PokemonEvolutionMatchups.Where(m => m.EvolvesFromSpeciesId == id).ToList();
                        foreach (var fill in fills)
                        {
                            fill.EvolvesFromSpeciesId = null; // Trenquem l'enllaç pare-fill
                        }

                        // Guardem aquests canvis preliminars (dependències) per evitar conflictes
                        context.SaveChanges();

                        // -----------------------------------------------------------------
                        // FASE 2: ELIMINACIÓ DEL POKÉMON
                        // -----------------------------------------------------------------

                        // Ara que està "net" de relacions, ja el podem esborrar sense errors SQL
                        context.Pokemons.Remove(pokemonAEliminar);
                        context.SaveChanges();

                        // -----------------------------------------------------------------
                        // FASE 3: REFRESCCAR IU
                        // -----------------------------------------------------------------

                        // Recarreguem la llista visual
                        carregarPokemons();

                        MessageBox.Show("Pokémon eliminat correctament.", "Esborrat", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error al intentar eliminar: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Si us plau, selecciona un Pokémon de la llista per eliminar-lo.", "Cap selecció", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Navega a la pàgina d'edició carregant les dades del Pokémon seleccionat.
        /// </summary>
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (lsvPokedex.SelectedItem is PokeData.Models.Pokemon seleccionado)
            {
                var paginaEdicion = new PaginaEdicio();
                paginaEdicion.ThePokemon = seleccionado; // Passem l'objecte existent per modificar (UPDATE)
                NavigationService.Navigate(paginaEdicion);
            }
            else
            {
                MessageBox.Show("Selecciona un Pokémon per editar.", "Avís", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// Navega a la pàgina d'edició amb un objecte Pokémon buit per crear-ne un de nou.
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var paginaEdicion = new PaginaEdicio();
            paginaEdicion.ThePokemon = new PokeData.Models.Pokemon(); // Nou Pokémon buit (INSERT)
            NavigationService.Navigate(paginaEdicion);
        }

        #endregion
    }
}