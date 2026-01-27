using DependencyPropertyGenerator;
using Microsoft.EntityFrameworkCore;
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
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El Pokémon actual que s'està editant", OnChanged = "OnThePokemonChanged")]
    [DependencyProperty<bool>("hiHaModificacions", Category = "Controlador", Description = "Indica si hi ha canvis no guardats al Pokémon")]

    // Propietats per fer Binding als ComboBox de tipus
    [DependencyProperty<Type>("SelectedType1", OnChanged = "OnSelectedType1Changed")]
    [DependencyProperty<Type>("SelectedType2", OnChanged = "OnSelectedType2Changed")]

    /// <summary>
    /// Pàgina d'edició per modificar o crear nous Pokémon.
    /// Gestiona dades bàsiques, tipus, habilitats i evolucions.
    /// </summary>
    public partial class PaginaEdicio : Page
    {
        private AppDbContext context = new AppDbContext();

        // Variables de dades
        private List<Type> _llistaTipus;
        public List<Type> LlistaTipusPublica { get { return _llistaTipus; } }

        private ObservableCollection<EvolutionEditableItem> _llistaEvos = new ObservableCollection<EvolutionEditableItem>();
        private ObservableCollection<Pokemon> _llistaTotsPokemons;

        private List<Ability> _todasLasHabilidades;
        private ObservableCollection<PokemonAbility> _habilidadesDelPokemon = new ObservableCollection<PokemonAbility>();

        // Bandera per evitar bucles infinits quan actualitzem els combos des del codi
        private bool _isUpdatingTypes = false;

        public PaginaEdicio()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Mètode que s'executa quan es carrega la pàgina.
        /// Inicialitza les llistes mestres i carrega les dades inicials.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ThePokemon == null || context == null) return;

            // 1. Carregar llista de Tipus disponibles
            _llistaTipus = context.Types.OrderBy(t => t.TypeName).ToList();
            _llistaTipus.Insert(0, new Type { TypeId = -1, TypeName = "---", Color = "#F0F0F0" });

            // Assignació directa per evitar problemes de binding
            cmbType1.ItemsSource = _llistaTipus;
            cmbType2.ItemsSource = _llistaTipus;

            // 2. Carregar dades del Pokémon actual a la interfície
            OnThePokemonChanged();

            // 3. Carregar Evolucions
            CarregarEvosUI();

            // 4. Carregar Habilitats
            CargarHabilidadesUI();
        }

        /// <summary>
        /// Es dispara automàticament quan canvia l'objecte ThePokemon (o al carregar la pàgina).
        /// Sincronitza els controls visuals amb les dades del model.
        /// </summary>
        public void OnThePokemonChanged()
        {
            if (ThePokemon == null || context == null) return;

            if (ThePokemon.BaseStat == null)
            {
                // Si no existe, creamos uno vacío en memoria para que los sliders tengan donde escribir
                ThePokemon.BaseStat = new BaseStat();
            }

            if (_llistaTipus != null)
            {
                // Bloquegem esdeveniments per evitar que el canvi de combo dispari lògica de guardat
                _isUpdatingTypes = true;

                var tipos = ThePokemon.PokemonTypes.ToList();

                // Assignar Tipus 1
                if (tipos.Count > 0)
                    SelectedType1 = _llistaTipus.FirstOrDefault(t => t.TypeId == tipos[0].TypeId);
                else
                    SelectedType1 = _llistaTipus.FirstOrDefault(t => t.TypeId == -1); // "---"

                // Assignar Tipus 2
                if (tipos.Count > 1)
                    SelectedType2 = _llistaTipus.FirstOrDefault(t => t.TypeId == tipos[1].TypeId);
                else
                    SelectedType2 = _llistaTipus.FirstOrDefault(t => t.TypeId == -1); // "---"

                _isUpdatingTypes = false; // Desbloquegem esdeveniments

                // Recarregar altres llistes dependents
                CarregarEvosUI();
                CargarHabilidadesUI();
            }
        }

        /// <summary>
        /// Callback quan l'usuari canvia el primer tipus al ComboBox.
        /// </summary>
        public void OnSelectedType1Changed()
        {
            if (_isUpdatingTypes || ThePokemon == null) return;
            ActualizarListaTipos();
        }

        /// <summary>
        /// Callback quan l'usuari canvia el segon tipus al ComboBox.
        /// </summary>
        public void OnSelectedType2Changed()
        {
            if (_isUpdatingTypes || ThePokemon == null) return;
            ActualizarListaTipos();
        }

        /// <summary>
        /// Sincronitza els tipus seleccionats als ComboBox amb la llista real del Pokémon (ThePokemon.PokemonTypes).
        /// També recalcula les resistències i valida el formulari.
        /// </summary>
        private void ActualizarListaTipos()
        {
            ThePokemon.PokemonTypes.Clear();

            // Afegir Tipus 1
            if (SelectedType1 != null && SelectedType1.TypeId != -1)
            {
                ThePokemon.PokemonTypes.Add(new PokemonType
                {
                    PokId = ThePokemon.PokId,
                    TypeId = SelectedType1.TypeId,
                    Type = SelectedType1
                });
            }

            // Afegir Tipus 2 (Només si és vàlid i diferent del primer)
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

            // Recalcular taula de debilitats/resistències en temps real
            if (ucDfModif != null) ucDfModif.calcularResistencies();

            // Validar estat del formulari
            validatePokemon();
        }

        #region Validació

        /// <summary>
        /// Esdeveniment genèric per validar quan canvia text als TextBox.
        /// </summary>
        private void ValidarText_Changed(object sender, TextChangedEventArgs e)
        {
            validatePokemon();
        }

        /// <summary>
        /// Comprova les regles de negoci i pinta els camps erronis de vermell.
        /// Retorna true si el pokemon és vàlid i false si no ho és.
        /// </summary>
        private bool validatePokemon()
        {
            bool valid = true;

            // Validar Nom
            if (string.IsNullOrWhiteSpace(ThePokemon.PokName)) valid = false;

            // Validar Alçada
            bool heightError = ThePokemon.PokHeight == null || ThePokemon.PokHeight <= 0;
            PintarError(ObtenerTextBox(0), heightError);
            if (heightError) valid = false;

            // Validar Pes
            bool weightError = ThePokemon.PokWeight == null || ThePokemon.PokWeight <= 0;
            PintarError(ObtenerTextBox(1), weightError);
            if (weightError) valid = false;

            // Validar Experiència Base
            bool expError = ThePokemon.PokBaseExperience == null || ThePokemon.PokBaseExperience < 0;
            PintarError(ObtenerTextBox(2), expError);
            if (expError) valid = false;

            // Validar Tipus (Ha de tenir almenys un tipus vàlid)
            bool typeError = (SelectedType1 == null || SelectedType1.TypeId == -1) &&
                             (SelectedType2 == null || SelectedType2.TypeId == -1);

            if (typeError)
            {
                cmbType1.BorderBrush = Brushes.Red;
                cmbType2.BorderBrush = Brushes.Red;
                valid = false;
            }
            else
            {
                cmbType1.ClearValue(Border.BorderBrushProperty);
                cmbType2.ClearValue(Border.BorderBrushProperty);
            }

            return valid;
        }

        /// <summary>
        /// Helper per canviar el color de fons d'un TextBox segons si té error.
        /// </summary>
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

        /// <summary>
        /// Busca un TextBox específic dins del Grid d'atributs segons la fila.
        /// </summary>
        private TextBox ObtenerTextBox(int row)
        {
            foreach (var child in grdAtributs.Children)
            {
                if (child is TextBox txt && Grid.GetRow(txt) == row) return txt;
            }
            return null;
        }

        #endregion

        #region Evolucions

        /// <summary>
        /// Prepara i carrega la llista visual d'evolucions, calculant les etapes.
        /// </summary>
        private void CarregarEvosUI()
        {
            // Càrrega mandrosa (lazy) de tots els Pokémon pel cercador
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

        /// <summary>
        /// Afegeix una nova evolució a la llista visual.
        /// </summary>
        private void btnAddEvo_Click(object sender, RoutedEventArgs e)
        {
            var seleccionado = cmbEvoSearch.SelectedItem as Pokemon;
            if (seleccionado != null)
            {
                // Evitar duplicats
                if (!_llistaEvos.Any(item => item.Pokemon.PokId == seleccionado.PokId))
                {
                    try
                    {
                    //int nuevaEtapa = 2;
                    //if (_llistaEvos.Count > 0) nuevaEtapa = _llistaEvos.Max(x => x.Stage);
                    int nuevaEtapa = _llistaEvos.Max(x => x.Stage);
                    _llistaEvos.Add(new EvolutionEditableItem { Pokemon = seleccionado, Stage = nuevaEtapa });

                    }
                    catch (Exception ex) {
                        //System.InvalidOperationException: 'Sequence contains no elements'???
                        //
                    }
                }
            }
        }

        /// <summary>
        /// Elimina l'evolució seleccionada de la llista visual.
        /// </summary>
        private void btnDelEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null) _llistaEvos.Remove(item);
        }

        /// <summary>
        /// Mou l'evolució a una etapa anterior (Stage - 1).
        /// </summary>
        private void btnUpEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null && item.Stage > 1)
            {
                item.Stage--;
                OrdenarLista();
            }
        }

        /// <summary>
        /// Mou l'evolució a una etapa posterior (Stage + 1).
        /// </summary>
        private void btnDownEvo_Click(object sender, RoutedEventArgs e)
        {
            var item = lstEvoluciones.SelectedItem as EvolutionEditableItem;
            if (item != null)
            {
                item.Stage++;
                OrdenarLista();
            }
        }

        /// <summary>
        /// Reordena la llista d'evolucions visualment per etapa i després per ID.
        /// </summary>
        private void OrdenarLista()
        {
            var listaOrdenada = _llistaEvos.OrderBy(x => x.Stage).ThenBy(x => x.Pokemon.PokId).ToList();
            _llistaEvos.Clear();
            foreach (var item in listaOrdenada) _llistaEvos.Add(item);
        }

        /// <summary>
        /// Gestiona l'activació dels botons segons si hi ha una evolució seleccionada.
        /// </summary>
        private void lstEvoluciones_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var itemSeleccionat = lstEvoluciones.SelectedItem as EvolutionEditableItem;

            if (itemSeleccionat != null)
            {
                btnDeleteEvo.IsEnabled = true;

                btnDownEvo.IsEnabled = true;

                if (itemSeleccionat.Stage > 1)
                {
                    btnUpEvo.IsEnabled = true;
                }
                else
                {
                    btnUpEvo.IsEnabled = false;
                }
            }
            else
            {
                // Si no hi ha res seleccionat, tot desactivat
                btnDeleteEvo.IsEnabled = false;
                btnUpEvo.IsEnabled = false;
                btnDownEvo.IsEnabled = false;
            }
        }

        /// <summary>
        /// Mètode segur per guardar evolucions linealment.
        /// Connecta Stage 1 -> Stage 2 -> Stage 3.
        /// Si la llista està buida, no fa res (evita l'error).
        /// </summary>
        private void guardarEvolucions()
        {
            // 1. PROTECCIÓ CRÍTICA: Si no hi ha llista o està buida, marxem.
            // Això arregla l'error de "dades que ja estaven bé".
            if (_llistaEvos == null || _llistaEvos.Count == 0) return;

            // 2. Preparació i Neteja
            Dictionary<EvolutionEditableItem, Pokemon> lineaEvo = new Dictionary<EvolutionEditableItem, Pokemon>();

            foreach (var evo in _llistaEvos)
            {
                // Recuperem l'entitat real de la BD
                var evoBd = context.Pokemons
                    .Include(p => p.PokemonEvolutionMatchup)
                    .FirstOrDefault(p => p.PokId == evo.Pokemon.PokId);

                if (evoBd != null)
                {
                    // Si no té fitxa (Pokémon nou), la creem
                    if (evoBd.PokemonEvolutionMatchup == null)
                    {
                        evoBd.PokemonEvolutionMatchup = new PokemonEvolutionMatchup
                        {
                            PokId = evoBd.PokId,
                            GenderRate = -1,
                            CaptureRate = 45,
                            BaseHappiness = 70
                        };
                        context.PokemonEvolutionMatchups.Add(evoBd.PokemonEvolutionMatchup);
                    }

                    // RESET: Trenquem la relació pare-fill momentàniament
                    evoBd.PokemonEvolutionMatchup.EvolvesFromSpeciesId = null;

                    lineaEvo.Add(evo, evoBd);
                }
            }

            // Guardem el reset (opcional però recomanable per netejar claus foranes)
            // context.SaveChanges(); 

            // 3. Reconstrucció de la Línia
            // Com que ja hem comprovat que Count > 0 a l'inici, això ja no pot fallar.
            int minStage = lineaEvo.Keys.Min(x => x.Stage);
            int maxStage = lineaEvo.Keys.Max(x => x.Stage);

            // Recorrem des de la segona etapa existent fins al final
            for (int i = minStage + 1; i <= maxStage; i++)
            {
                // Busquem Pares (Stage i - 1) i Fills (Stage i)
                var possiblesParesUI = lineaEvo.Keys.Where(x => x.Stage == i - 1).ToList();
                var fillsEtapaUI = lineaEvo.Keys.Where(x => x.Stage == i).ToList();

                // Si falta l'etlabó perdut (no hi ha pare a l'etapa anterior), saltem
                if (possiblesParesUI.Count == 0) continue;

                // ESTRATÈGIA SIMPLE: El primer de l'etapa anterior és el pare de tots els d'ara.
                var itemPareUI = possiblesParesUI.First();
                var entitatPare = lineaEvo[itemPareUI];

                foreach (var itemFillUI in fillsEtapaUI)
                {
                    var entitatFill = lineaEvo[itemFillUI];
                    // "Jo sóc el teu pare"
                    entitatFill.PokemonEvolutionMatchup.EvolvesFromSpeciesId = entitatPare.PokId;
                }
            }
        }
        #endregion

        #region Habilitats

        /// <summary>
        /// Carrega les habilitats del Pokémon a la llista visual.
        /// </summary>
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
                // Assegurar que l'objecte Abil està carregat
                if (hab.Abil == null)
                {
                    hab.Abil = context.Abilities.Find(hab.AbilId);
                }
                _habilidadesDelPokemon.Add(hab);
            }
        }

        /// <summary>
        /// Afegeix una habilitat seleccionada al Pokémon.
        /// </summary>
        private void btnAddAbility_Click(object sender, RoutedEventArgs e)
        {
            var habilidadSeleccionada = cmbAbilitySearch.SelectedItem as Ability;

            if (habilidadSeleccionada != null)
            {
                // 1. COMPROBACIÓN: ¿Ya tiene esta habilidad?
                // Buscamos si existe alguna habilidad en la lista con el mismo ID
                bool yaExiste = ThePokemon.PokemonAbilities.Any(pa => pa.AbilId == habilidadSeleccionada.AbilId);

                if (yaExiste)
                {
                    MessageBox.Show($"El Pokémon ja té l'habilitat '{habilidadSeleccionada.AbilName}'.",
                                    "Habilitat duplicada", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // Limpiamos el buscador y salimos sin hacer nada
                    cmbAbilitySearch.SelectedItem = null;
                    cmbAbilitySearch.Text = "";
                    return;
                }

                // 2. Si no existe, procedemos a añadirla (Tu código original)
                var nuevaRelacion = new PokemonAbility
                {
                    PokId = ThePokemon.PokId,
                    AbilId = habilidadSeleccionada.AbilId,
                    Abil = habilidadSeleccionada,
                    IsHidden = false,
                    Slot = 1 // Podrías calcular el slot libre si quisieras (ej: Count + 1)
                };

                ThePokemon.PokemonAbilities.Add(nuevaRelacion);
                _habilidadesDelPokemon.Add(nuevaRelacion);

                // Limpiar selección
                cmbAbilitySearch.SelectedItem = null;
                cmbAbilitySearch.Text = "";
            }
        }

        /// <summary>
        /// Elimina una habilitat del Pokémon.
        /// </summary>
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

        #region Navegació i Guardat

        /// <summary>
        /// Torna a la pàgina anterior sense guardar.
        /// </summary>
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        /// <summary>
        /// Gestiona l'esdeveniment de clic del botó de Guardar.
        /// Verifica si el Pokémon existeix a la base de dades per actualitzar-lo o crear-ne un de nou.
        /// Gestiona la persistència de propietats bàsiques, així com les relacions de tipus, habilitats i evolucions.
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ---------------------------------------------------------
                // 1. VALIDACIÓ DE DADES (Nom, stats, etc.)
                // ---------------------------------------------------------
                if (ThePokemon == null) return;

                // Cridem al mètode que pinta de vermell si falta alguna cosa
                if(!validatePokemon())
                {
                    throw new Exception("S'han de completar tots els camps!");
                }



                // ---------------------------------------------------------
                // 3. RECUPERAR O CREAR EL POKÉMON A LA BASE DE DADES
                // ---------------------------------------------------------

                var pokemonDb = context.Pokemons
                                       .Include(p => p.PokemonTypes)
                                       .Include(p => p.PokemonAbilities)
                                       .Include(p => p.BaseStat) 
                                       .FirstOrDefault(p => p.PokId == ThePokemon.PokId);

                // Si no existeix, és un INSERT (Nou Pokémon)
                if (pokemonDb == null)
                {
                    pokemonDb = new Pokemon();
                    context.Pokemons.Add(pokemonDb);
                }

                // ---------------------------------------------------------
                // 4. ACTUALITZAR DADES BÀSIQUES
                // ---------------------------------------------------------
                pokemonDb.PokName = ThePokemon.PokName;
                pokemonDb.PokHeight = ThePokemon.PokHeight;
                pokemonDb.PokWeight = ThePokemon.PokWeight;
                pokemonDb.PokBaseExperience = ThePokemon.PokBaseExperience;
                pokemonDb.PokImage = ThePokemon.PokImage; // Si has implementat la imatge

                // ---------------------------------------------------------
                // 5. ACTUALITZAR TIPUS (Esborrar els vells i posar els nous)
                // ---------------------------------------------------------
                pokemonDb.PokemonTypes.Clear();

                // Afegim Tipus 1
                if (SelectedType1 != null && SelectedType1.TypeId != -1)
                {
                    pokemonDb.PokemonTypes.Add(new PokemonType { TypeId = SelectedType1.TypeId, Slot = 1 });
                }

                // Afegim Tipus 2 (si n'hi ha)
                if (SelectedType2 != null && SelectedType2.TypeId != -1)
                {
                    pokemonDb.PokemonTypes.Add(new PokemonType { TypeId = SelectedType2.TypeId, Slot = 2 });
                }

                // ---------------------------------------------------------
                // 6. ACTUALITZAR HABILITATS (Opcional, segons la teva lògica)
                // ---------------------------------------------------------
                // Nota: Aquí mantinc la lògica que tenies, assumint que 'ThePokemon.PokemonAbilities' ve ple.
                // Si les gestiones diferent a la UI, potser caldria canviar això.
                pokemonDb.PokemonAbilities.Clear();
                int slotAbil = 1;
                foreach (var a in ThePokemon.PokemonAbilities)
                {
                    pokemonDb.PokemonAbilities.Add(new PokemonAbility
                    {
                        AbilId = a.AbilId,
                        IsHidden = a.IsHidden,
                        Slot = a.Slot > 0 ? a.Slot : slotAbil // Assegurar slot vàlid
                    });
                    slotAbil++;
                }

                // ---------------------------------------------------------
                // 7. ACTUALITZAR EVOLUCIONS (Estratègia: Delete & Re-Insert)
                // ---------------------------------------------------------
                guardarEvolucions();


                // ---------------------------------------------------------
                // 8. GUARDAR ESTADISTIQUES BASE
                // ---------------------------------------------------------
                if (ThePokemon.BaseStat != null)
                {
                    // Si la fitxa d'estadístiques no existeix a la BD, la creem
                    if (pokemonDb.BaseStat == null)
                    {
                        // Assegura't que la teva classe es diu 'PokemonStat' o 'BaseStats' segons el teu model
                        pokemonDb.BaseStat = new BaseStat { PokId = pokemonDb.PokId };
                    }

                    // Copiem els valors directament (molt més net!)
                    pokemonDb.BaseStat.BHp = ThePokemon.BaseStat.BHp;
                    pokemonDb.BaseStat.BAtk = ThePokemon.BaseStat.BAtk;
                    pokemonDb.BaseStat.BDef = ThePokemon.BaseStat.BDef;
                    pokemonDb.BaseStat.BSpAtk = ThePokemon.BaseStat.BSpAtk;
                    pokemonDb.BaseStat.BSpDef = ThePokemon.BaseStat.BSpDef;
                    pokemonDb.BaseStat.BSpeed = ThePokemon.BaseStat.BSpeed;
                }

                // ---------------------------------------------------------
                // 9. GUARDAR CANVIS I SORTIR
                // ---------------------------------------------------------
                context.SaveChanges();

                MessageBox.Show("Guardat correctament", "Èxit", MessageBoxButton.OK, MessageBoxImage.Information);

                if (NavigationService.CanGoBack) NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar: {ex.Message} \n\n {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }




        #endregion


        
    }

    /// <summary>
    /// Classe auxiliar per gestionar els ítems de la llista d'evolucions (Pokémon + Etapa).
    /// </summary>
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