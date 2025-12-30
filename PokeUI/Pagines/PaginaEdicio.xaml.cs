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
    [DependencyProperty<bool>("hiHaModificacions", Category = "controlador", Description = "Mira si hi ha canvis al Pokemon")]

    /// <summary>
    /// Lógica de interacción para PaginaEdicio.xaml
    /// </summary>
    public partial class PaginaEdicio : Page
    {
        AppDbContext context = new AppDbContext();

        private List<Type> _llistaTipus;
        private ObservableCollection<EvolutionEditableItem> _llistaEvos = new ObservableCollection<EvolutionEditableItem>(); 
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
            // ... (Tu parte del buscador se queda igual) ...
            if (this._llistaTotsPokemons == null)
            {
                _llistaTotsPokemons = new ObservableCollection<Pokemon>(context.Pokemons.OrderBy(p => p.PokName).ToList());
                cmbEvoSearch.ItemsSource = _llistaTotsPokemons;
            }

            lstEvoluciones.ItemsSource = this._llistaEvos;
            _llistaEvos.Clear();

            if (ThePokemon != null)
            {
                // Usamos el método de ETAPAS que ya programamos para el UserControl
                // Esto nos devuelve: List[0] = {Bulbasaur}, List[1] = {Ivysaur}, List[2] = {Venusaur}
                var etapas = EvolutionUtils.GetEvolucionesPorEtapas(context, ThePokemon.PokId);

                for (int i = 0; i < etapas.Count; i++)
                {
                    int numeroEtapa = i + 1; // Etapa 1, 2, 3...
                    foreach (var poke in etapas[i])
                    {
                        _llistaEvos.Add(new EvolutionEditableItem
                        {
                            Pokemon = poke,
                            Stage = numeroEtapa
                        });
                    }
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
                if (!_llistaEvos.Any(item => item.Pokemon.PokId == seleccionado.PokId))
                {
                    // Por defecto lo añadimos como Etapa 2 (o la última + 1)
                    int nuevaEtapa = 2;
                    if (_llistaEvos.Count > 0) nuevaEtapa = _llistaEvos.Max(x => x.Stage);

                    _llistaEvos.Add(new EvolutionEditableItem { Pokemon = seleccionado, Stage = nuevaEtapa });
                }
            }
        }

        private void btnDelEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null)
            {
                _llistaEvos.Remove(item);
            }
        }

        // BOTÓN SUBIR NIVEL (Ahora resta el número de etapa)
        private void btnUpEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null && item.Stage > 1)
            {
                item.Stage--; // Pasa de Etapa 2 a Etapa 1

                // Opcional: Reordenar la lista visualmente
                OrdenarLista();
            }
        }

        // BOTÓN BAJAR NIVEL (Ahora suma el número de etapa)
        private void btnDownEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null)
            {
                item.Stage++; // Pasa de Etapa 1 a Etapa 2
                OrdenarLista();
            }
        }

        // Pequeño helper para mantener la lista ordenada por etapas (1, 1, 2, 2, 2...)
        private void OrdenarLista()
        {
            var listaOrdenada = _llistaEvos.OrderBy(x => x.Stage).ThenBy(x => x.Pokemon.PokId).ToList();
            _llistaEvos.Clear();
            foreach (var item in listaOrdenada) _llistaEvos.Add(item);
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


    public class EvolutionEditableItem : System.ComponentModel.INotifyPropertyChanged
    {
        private int _stage;
        public Pokemon Pokemon { get; set; }

        public int Stage
        {
            get { return _stage; }
            set
            {
                _stage = value;
                OnPropertyChanged("Stage");
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}
