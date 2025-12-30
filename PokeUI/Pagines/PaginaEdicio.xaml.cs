using DependencyPropertyGenerator;
using PokeData.Models;
using PokeUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private List<Type> _llistaTipus;
        private ObservableCollection<Pokemon> _llistaEvos = new ObservableCollection<Pokemon>();
        private ObservableCollection<Pokemon> _llistaTotsPokemons;
        public PaginaEdicio()
        {
            InitializeComponent();
        }





        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ThePokemon == null || context == null) return;

            _llistaTipus = context.Types.OrderBy(t => t.TypeName).ToList();
            _llistaTipus.Insert(0, new Type { TypeId = -1, TypeName = "---", Color = "#F0F0F0" });

            GenerarCombosDinamicament();
            CarregarEvosUI();

        }

        public void OnThePokemonChanged()
        {
            if (ThePokemon == null || context == null) return;
            if (_llistaTipus != null)
            {
                GenerarCombosDinamicament();
                CarregarEvosUI();
            }
        }

        #region Comboboxes
        private void GenerarCombosDinamicament()
        {
            stpTipos.Children.Clear();

            var tipusDelPokemon = ThePokemon.PokemonTypes.Select(pt => pt.Type).ToList();
            int maxTipus = 2;

            for (int i = 0; i < maxTipus; i++)
            {
                var tipoActual = (i < tipusDelPokemon.Count) ? tipusDelPokemon[i] : null;

                // Lógica para pintar huecos:
                // - Si el tipo es null y ya llevamos maxTipos, paramos.
                // - Si estamos en el 2º hueco y el 1º estaba vacío, paramos (no dejar huecos intercalados).
                if (tipoActual == null && tipusDelPokemon.Count >= maxTipus) continue;
                if (i > 0 && tipusDelPokemon.Count == 0) continue;

                // Creamos y añadimos el combo
                ComboBox cmb = CrearComboTipo(tipoActual);
                stpTipos.Children.Add(cmb);
            }
        }

        private ComboBox CrearComboTipo(PokeData.Models.Type tipoSeleccionado)
        {
            ComboBox cmb = new ComboBox();
            cmb.Width = 100;
            cmb.Margin = new Thickness(0, 0, 5, 0);

            cmb.ItemsSource = _llistaTipus;
            cmb.ItemTemplate = (DataTemplate)this.FindResource("PlantillaComboBox");

            // Seleccionar el ítem correcto
            if (tipoSeleccionado != null)
            {
                cmb.SelectedItem = _llistaTipus.FirstOrDefault(t => t.TypeId == tipoSeleccionado.TypeId);
                cmb.Background = UIUtils.GetBrushFromHex(tipoSeleccionado.Color);
              
            }
            else
            {
                cmb.SelectedIndex = 0; //Hem fet l'insert de la llista a l'index 0.
                //pintarFondoCombo(cmb, "#F0F0F0");
            }

            // Suscribir al evento de cambio
            cmb.SelectionChanged += Cmb_SelectionChanged;

            return cmb;
        }





        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var nuevoTipo = cmb.SelectedItem as PokeData.Models.Type;

            // 1. Efecto visual inmediato
            if (nuevoTipo != null) cmb.Background = UIUtils.GetBrushFromHex(nuevoTipo.Color);
            else cmb.Background = Brushes.White;

            // 2. Actualizar la lista real del Pokémon
            ThePokemon.PokemonTypes.Clear();

            foreach (var control in stpTipos.Children)
            {
                // Añadimos una condición extra: t.TypeId != -1
                if (control is ComboBox c && c.SelectedItem is PokeData.Models.Type t && t.TypeId != -1)
                {
                    if (!ThePokemon.PokemonTypes.Any(pt => pt.TypeId == t.TypeId))
                    {
                        ThePokemon.PokemonTypes.Add(new PokemonType
                        {
                            PokId = ThePokemon.PokId,
                            TypeId = t.TypeId,
                            Type = t
                        });
                    }
                }
            }

            // 3. Regenerar combos para mostrar/ocultar el hueco vacío siguiente
            cmb.SelectionChanged -= Cmb_SelectionChanged; // Desconectar para evitar bucles
            GenerarCombosDinamicament();

        }
        #endregion



        #region evolucions
        private void CarregarEvosUI()
        {
            // 1. Buscador (Carga Lazy)
            if (this._llistaTotsPokemons == null)
            {
                _llistaTotsPokemons = new ObservableCollection<Pokemon>(context.Pokemons.OrderBy(p => p.PokName).ToList());
                cmbEvoSearch.ItemsSource = _llistaTotsPokemons;
            }

            // 2. Vincular lista visual
            lstEvoluciones.ItemsSource = this._llistaEvos;
            _llistaEvos.Clear();

            // 3. ¡AQUÍ ESTÁ LA MAGIA DRY!
            if (ThePokemon != null)
            {
                // Llamamos a nuestra clase Utils en lugar de escribir 50 líneas de código aquí
                var familia = EvolutionUtils.GetFamiliaCompleta(context, ThePokemon.PokId);

                // Rellenamos la ObservableCollection con los resultados
                foreach (var p in familia)
                {
                    _llistaEvos.Add(p);
                }
            }
        }
        #endregion

        #region ClicksBotons

        // BOTÓN AÑADIR (+)
        private void btnAddEvo_Click(object sender, RoutedEventArgs e)
        {
            var seleccionado = cmbEvoSearch.SelectedItem as Pokemon;
            if (seleccionado != null)
            {
                // Evitar duplicados
                if (!_llistaEvos.Any(p => p.PokId == seleccionado.PokId))
                {
                    _llistaEvos.Add(seleccionado);
                }
            }
        }

        // BOTÓN ELIMINAR (X)
        private void btnDelEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as Pokemon;
            if (item != null)
            {
                _llistaEvos.Remove(item);
            }
        }

        // BOTÓN SUBIR (↑)
        private void btnUpEvo_Click(object sender, RoutedEventArgs e)
        {
            int index = lstEvoluciones.SelectedIndex;
            // Si hay algo seleccionado y no es el primero
            if (index > 0)
            {
                _llistaEvos.Move(index, index - 1);
            }
        }

        // BOTÓN BAJAR (↓)
        private void btnDownEvo_Click(object sender, RoutedEventArgs e)
        {
            int index = lstEvoluciones.SelectedIndex;
            // Si hay algo seleccionado y no es el último
            if (index >= 0 && index < _llistaEvos.Count - 1)
            {
                _llistaEvos.Move(index, index + 1);
            }
        }
        #endregion


        private void validatePokemon()
        {
            /*Tots els camps són obligatoris, i han de tenir un caràcter diferent d’espai com a mínim.
                Com a mínim, ha de tenir un tipus.
                Els camps numèrics han de validar-se dins un rang raonable.
                Si hi ha qualsevol camp erroni, el botó desar es deshabilita i es mostra el fons del Textbox de color
                vermell suau.*/
        }
    }
}
