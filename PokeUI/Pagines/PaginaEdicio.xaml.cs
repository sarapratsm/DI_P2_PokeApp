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

    // PROPIEDADES NUEVAS PARA LOS COMBOS
    [DependencyProperty<Type>("SelectedType1", OnChanged = "OnSelectedType1Changed")]
    [DependencyProperty<Type>("SelectedType2", OnChanged = "OnSelectedType2Changed")]

    public partial class PaginaEdicio : Page
    {
        AppDbContext context = new AppDbContext();

        // Variables de datos
        private List<Type> _llistaTipus;
        public List<Type> LlistaTipusPublica { get { return _llistaTipus; } } // Propiedad pública para el XAML

        private ObservableCollection<EvolutionEditableItem> _llistaEvos = new ObservableCollection<EvolutionEditableItem>();
        private ObservableCollection<Pokemon> _llistaTotsPokemons;
        private List<Ability> _todasLasHabilidades;
        private ObservableCollection<PokemonAbility> _habilidadesDelPokemon = new ObservableCollection<PokemonAbility>();

        // Flag para evitar bucles infinitos al cargar
        private bool _isUpdatingTypes = false;

        public PaginaEdicio()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ThePokemon == null || context == null) return;

            // 1. Cargar Tipos
            _llistaTipus = context.Types.OrderBy(t => t.TypeName).ToList();
            _llistaTipus.Insert(0, new Type { TypeId = -1, TypeName = "---", Color = "#F0F0F0" });

            cmbType1.ItemsSource = _llistaTipus;
            cmbType2.ItemsSource = _llistaTipus;

            // 2. Cargar datos del Pokémon actual
            OnThePokemonChanged();

            // 3. Cargar Evoluciones
            CarregarEvosUI();

            // 4. Cargar Habilidades
            CargarHabilidadesUI();
        }

        public void OnThePokemonChanged()
        {
            if (ThePokemon == null || context == null) return;

            if (_llistaTipus != null)
            {
                // Sincronizar Combos con la lista real del Pokémon
                _isUpdatingTypes = true; // Bloqueamos eventos

                var tipos = ThePokemon.PokemonTypes.ToList();

                // Tipo 1
                if (tipos.Count > 0)
                    SelectedType1 = _llistaTipus.FirstOrDefault(t => t.TypeId == tipos[0].TypeId);
                else
                    SelectedType1 = _llistaTipus.FirstOrDefault(t => t.TypeId == -1); // "---"

                // Tipo 2
                if (tipos.Count > 1)
                    SelectedType2 = _llistaTipus.FirstOrDefault(t => t.TypeId == tipos[1].TypeId);
                else
                    SelectedType2 = _llistaTipus.FirstOrDefault(t => t.TypeId == -1); // "---"

                _isUpdatingTypes = false; // Desbloqueamos

                // Otras recargas
                CarregarEvosUI();
                CargarHabilidadesUI();
            }
        }

        // --- CALLBACKS DE CAMBIO DE TIPO (Disparados por Binding) ---
        public void OnSelectedType1Changed()
        {
            if (_isUpdatingTypes || ThePokemon == null) return;
            ActualizarListaTipos();
        }

        public void OnSelectedType2Changed()
        {
            if (_isUpdatingTypes || ThePokemon == null) return;
            ActualizarListaTipos();
        }

        // --- MÉTODO CENTRAL PARA ACTUALIZAR TIPOS ---
        private void ActualizarListaTipos()
        {
            ThePokemon.PokemonTypes.Clear();

            // Añadir Tipo 1
            if (SelectedType1 != null && SelectedType1.TypeId != -1)
            {
                ThePokemon.PokemonTypes.Add(new PokemonType
                {
                    PokId = ThePokemon.PokId,
                    TypeId = SelectedType1.TypeId,
                    Type = SelectedType1
                });
            }

            // Añadir Tipo 2 (Si es válido y diferente al 1)
            if (SelectedType2 != null && SelectedType2.TypeId != -1)
            {
                if (SelectedType1 == null || SelectedType2.TypeId != SelectedType1.TypeId)
                {
                    ThePokemon.PokemonTypes.Add(new PokemonType
                    {
                        PokId = ThePokemon.PokId,
                        TypeId = SelectedType2.TypeId,
                        Type = SelectedType2
                    });
                }
            }

            // Recalcular defensas
            if (ucDfModif != null) ucDfModif.calcularResistencies();

            // Validar formulario
            validatePokemon();
        }

        // --- VALIDACIÓN ---
        private void ValidarTexto_Changed(object sender, TextChangedEventArgs e)
        {
            validatePokemon();
        }

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

            // Validar Tipos (usando las propiedades seleccionadas)
            bool typeError = (SelectedType1 == null || SelectedType1.TypeId == -1) && (SelectedType2 == null || SelectedType2.TypeId == -1);

            // Pintar los combos de rojo si no hay tipos
            if (typeError)
            {
                cmbType1.BorderBrush = Brushes.Red;
                cmbType2.BorderBrush = Brushes.Red;
                hayErrores = true;
            }
            else
            {
                cmbType1.ClearValue(Border.BorderBrushProperty);
                cmbType2.ClearValue(Border.BorderBrushProperty);
            }

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


        #region Evoluciones
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

        #region Habilidades
        private void CargarHabilidadesUI()
        {
            if (_todasLasHabilidades == null)
            {
                _todasLasHabilidades = context.Abilities.OrderBy(a => a.AbilName).ToList();
                cmbAbilitySearch.ItemsSource = _todasLasHabilidades;
            }

            lstAbilities.ItemsSource = _habilidadesDelPokemon;
            _habilidadesDelPokemon.Clear();

            foreach (var hab in ThePokemon.PokemonAbilities)
            {
                if (hab.Abil == null)
                {
                    hab.Abil = context.Abilities.Find(hab.AbilId);
                }
                _habilidadesDelPokemon.Add(hab);
            }
        }

        private void btnAddAbility_Click(object sender, RoutedEventArgs e)
        {
            var habilidadSeleccionada = cmbAbilitySearch.SelectedItem as Ability;

            if (habilidadSeleccionada != null)
            {
                if (!ThePokemon.PokemonAbilities.Any(pa => pa.AbilId == habilidadSeleccionada.AbilId))
                {
                    var nuevaRelacion = new PokemonAbility
                    {
                        PokId = ThePokemon.PokId,
                        AbilId = habilidadSeleccionada.AbilId,
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