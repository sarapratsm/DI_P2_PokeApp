using DependencyPropertyGenerator;
using PokeData.Models;
using PokeUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Type = PokeData.Models.Type;

namespace PokeUI.Pagines
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged = "OnThePokemonChanged")]
    [DependencyProperty<bool>("hiHaModificacions", Category = "controlador", Description = "Mira si hi ha canvis al Pokemon")]

    public partial class PaginaEdicio : Page
    {
        AppDbContext context = new AppDbContext();

        private List<Type> _llistaTipus;
        private ObservableCollection<EvolutionEditableItem> _llistaEvos = new ObservableCollection<EvolutionEditableItem>();
        private ObservableCollection<Pokemon> _llistaTotsPokemons;

        private List<Ability> _todasLasHabilidades;
        private ObservableCollection<PokemonAbility> _habilidadesDelPokemon = new ObservableCollection<PokemonAbility>();

        public PaginaEdicio()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ThePokemon == null || context == null) return;

            // 1. Tipos
            _llistaTipus = context.Types.OrderBy(t => t.TypeName).ToList();
            _llistaTipus.Insert(0, new Type { TypeId = -1, TypeName = "---", Color = "#F0F0F0" });

            GenerarCombosDinamicament();

            // 2. Evoluciones
            CarregarEvosUI();

            // 3. Habilidades
            CargarHabilidadesUI();
        }

        public void OnThePokemonChanged()
        {
            if (ThePokemon == null || context == null) return;
            if (_llistaTipus != null)
            {
                GenerarCombosDinamicament();
                CarregarEvosUI();
                CargarHabilidadesUI();
            }
        }

        private void ValidarTexto_Changed(object sender, TextChangedEventArgs e)
        {
            validatePokemon();
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

                if (tipoActual == null && tipusDelPokemon.Count >= maxTipus) continue;
                if (i > 0 && tipusDelPokemon.Count == 0) continue;

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

            if (tipoSeleccionado != null)
            {
                cmb.SelectedItem = _llistaTipus.FirstOrDefault(t => t.TypeId == tipoSeleccionado.TypeId);
                cmb.Background = UIUtils.GetBrushFromHex(tipoSeleccionado.Color);
            }
            else
            {
                cmb.SelectedIndex = 0;
            }
            cmb.SelectionChanged += Cmb_SelectionChanged;
            return cmb;
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var nuevoTipo = cmb.SelectedItem as PokeData.Models.Type;

            if (nuevoTipo != null) cmb.Background = UIUtils.GetBrushFromHex(nuevoTipo.Color);
            else cmb.Background = Brushes.White;

            ThePokemon.PokemonTypes.Clear();
            foreach (var control in stpTipos.Children)
            {
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
            cmb.SelectionChanged -= Cmb_SelectionChanged;
            GenerarCombosDinamicament();

            if (ucDfModif != null)
            {
                ucDfModif.calcularResistencies();
            }

            validatePokemon();
        }
        #endregion

        #region Evoluciones (Logica)
        private void CarregarEvosUI()
        {
            if (this._llistaTotsPokemons == null)
            {
                _llistaTotsPokemons = new ObservableCollection<Pokemon>(context.Pokemons.OrderBy(p => p.PokName).ToList());
                cmbEvoSearch.ItemsSource = _llistaTotsPokemons;
            }

            lstEvoluciones.ItemsSource = this._llistaEvos;
            _llistaEvos.Clear();

            if (ThePokemon != null)
            {
                var etapas = EvolutionUtils.GetEvolucionesPorEtapas(context, ThePokemon.PokId);

                for (int i = 0; i < etapas.Count; i++)
                {
                    int numeroEtapa = i + 1;
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

        #region Habilidades (ABILITIES) - CORREGIDO

        private void CargarHabilidadesUI()
        {
            // 1. Cargar el diccionario completo para el buscador
            if (_todasLasHabilidades == null)
            {
                _todasLasHabilidades = context.Abilities.OrderBy(a => a.AbilName).ToList();
                cmbAbilitySearch.ItemsSource = _todasLasHabilidades;
            }

            // 2. Vincular la lista visual
            lstAbilities.ItemsSource = _habilidadesDelPokemon;
            _habilidadesDelPokemon.Clear();

            // 3. Rellenar asegurándonos de que tenemos el NOMBRE
            foreach (var hab in ThePokemon.PokemonAbilities)
            {
                // --- FIX: Si falta el objeto Ability (el nombre), lo cargamos a mano ---
                if (hab.Abil.AbilName == null)
                {
                    hab.Abil = context.Abilities.Find(hab.AbilId);
                }
                // ----------------------------------------------------------------------

                _habilidadesDelPokemon.Add(hab);
            }
        }

        private void btnAddAbility_Click(object sender, RoutedEventArgs e)
        {
            var habilidadSeleccionada = cmbAbilitySearch.SelectedItem as Ability;

            if (habilidadSeleccionada != null)
            {
                // CAMBIO: Usar AbilId en lugar de AbilityId (tanto en la relación como en la habilidad)
                if (!ThePokemon.PokemonAbilities.Any(pa => pa.AbilId == habilidadSeleccionada.AbilId))
                {
                    var nuevaRelacion = new PokemonAbility
                    {
                        PokId = ThePokemon.PokId,
                        AbilId = habilidadSeleccionada.AbilId, // CORREGIDO
                        Abil = habilidadSeleccionada,
                        IsHidden = false,
                        Slot = 1
                    };

                    ThePokemon.PokemonAbilities.Add(nuevaRelacion);
                    _habilidadesDelPokemon.Add(nuevaRelacion);

                    cmbAbilitySearch.SelectedItem = null;
                    cmbAbilitySearch.Text = "";
                }
            }
        }

        private void btnRemoveAbility_Click(object sender, RoutedEventArgs e)
        {
            var boton = sender as Button;
            var habilidadAQuitar = boton.DataContext as PokemonAbility;

            if (habilidadAQuitar != null)
            {
                ThePokemon.PokemonAbilities.Remove(habilidadAQuitar);
                _habilidadesDelPokemon.Remove(habilidadAQuitar);
            }
        }

        #endregion

        #region ClicksBotons (Evoluciones)
        private void btnAddEvo_Click(object sender, RoutedEventArgs e)
        {
            var seleccionado = cmbEvoSearch.SelectedItem as Pokemon;
            if (seleccionado != null)
            {
                if (!_llistaEvos.Any(item => item.Pokemon.PokId == seleccionado.PokId))
                {
                    int nuevaEtapa = 2;
                    if (_llistaEvos.Count > 0) nuevaEtapa = _llistaEvos.Max(x => x.Stage);
                    _llistaEvos.Add(new EvolutionEditableItem { Pokemon = seleccionado, Stage = nuevaEtapa });
                }
            }
        }

        private void btnDelEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null) _llistaEvos.Remove(item);
        }

        private void btnUpEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null && item.Stage > 1)
            {
                item.Stage--;
                OrdenarLista();
            }
        }

        private void btnDownEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null)
            {
                item.Stage++;
                OrdenarLista();
            }
        }

        private void OrdenarLista()
        {
            var listaOrdenada = _llistaEvos.OrderBy(x => x.Stage).ThenBy(x => x.Pokemon.PokId).ToList();
            _llistaEvos.Clear();
            foreach (var item in listaOrdenada) _llistaEvos.Add(item);
        }
        #endregion

        #region Validacion
        private void validatePokemon()
        {
            bool hayErrores = false;

            if (string.IsNullOrWhiteSpace(ThePokemon.PokName)) hayErrores = true;

            bool heightError = ThePokemon.PokHeight == null || ThePokemon.PokHeight <= 0;
            PintarError(ObtenerTextBox(0), heightError);
            if (heightError) hayErrores = true;

            bool weightError = ThePokemon.PokWeight == null || ThePokemon.PokWeight <= 0;
            PintarError(ObtenerTextBox(1), weightError);
            if (weightError) hayErrores = true;

            bool expError = ThePokemon.PokBaseExperience == null || ThePokemon.PokBaseExperience < 0;
            PintarError(ObtenerTextBox(2), expError);
            if (expError) hayErrores = true;

            bool typeError = ThePokemon.PokemonTypes.Count == 0;
            stpTipos.Background = typeError ? new SolidColorBrush(Color.FromArgb(50, 255, 0, 0)) : Brushes.Transparent;
            if (typeError) hayErrores = true;

            // btnSave.IsEnabled = !hayErrores;
        }

        private void PintarError(TextBox txt, bool error)
        {
            if (txt == null) return;
            if (error)
            {
                txt.Background = new SolidColorBrush(Color.FromRgb(255, 200, 200));
                txt.BorderBrush = Brushes.Red;
            }
            else
            {
                txt.Background = Brushes.White;
                txt.BorderBrush = Brushes.Gray;
            }
        }

        private TextBox ObtenerTextBox(int row)
        {
            foreach (var child in grdAtributs.Children)
            {
                if (child is TextBox txt && Grid.GetRow(txt) == row) return txt;
            }
            return null;
        }
        #endregion
    }

    public class EvolutionEditableItem : System.ComponentModel.INotifyPropertyChanged
    {
        private int _stage;
        public Pokemon Pokemon { get; set; }

        public int Stage
        {
            get { return _stage; }
            set { _stage = value; OnPropertyChanged("Stage"); }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));
    }
}