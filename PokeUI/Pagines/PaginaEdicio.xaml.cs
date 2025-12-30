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
        private List<Type> llistaTipus;
        public PaginaEdicio()
        {
            InitializeComponent();
        }





        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (ThePokemon == null || context == null) return;

            llistaTipus = context.Types.OrderBy(t => t.TypeName).ToList();
            llistaTipus.Insert(0, new Type { TypeId = -1, TypeName = "---", Color = "#F0F0F0" });

            GenerarCombosDinamicament();

        }

        public void OnThePokemonChanged()
        {
            if (ThePokemon == null || context == null) return;
            if (llistaTipus != null)
            {
                GenerarCombosDinamicament();
            }
        }


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

            cmb.ItemsSource = llistaTipus;
            cmb.ItemTemplate = (DataTemplate)this.FindResource("PlantillaComboBox");

            // Seleccionar el ítem correcto
            if (tipoSeleccionado != null)
            {
                cmb.SelectedItem = llistaTipus.FirstOrDefault(t => t.TypeId == tipoSeleccionado.TypeId);
                pintarFondoCombo(cmb, tipoSeleccionado.Color);
            }
            else
            {
                cmb.SelectedIndex = 0; //Hem fet l'insert de la llista a l'index 0.
                pintarFondoCombo(cmb, "#F0F0F0");
            }

            // Suscribir al evento de cambio
            cmb.SelectionChanged += Cmb_SelectionChanged;

            return cmb;
        }

        private void validatePokemon()
        {
            /*Tots els camps són obligatoris, i han de tenir un caràcter diferent d’espai com a mínim.
                Com a mínim, ha de tenir un tipus.
                Els camps numèrics han de validar-se dins un rang raonable.
                Si hi ha qualsevol camp erroni, el botó desar es deshabilita i es mostra el fons del Textbox de color
                vermell suau.*/
        }

        private void Cmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cmb = sender as ComboBox;
            var nuevoTipo = cmb.SelectedItem as PokeData.Models.Type;

            // 1. Efecto visual inmediato
            if (nuevoTipo != null) pintarFondoCombo(cmb, nuevoTipo.Color);
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

        private void pintarFondoCombo(ComboBox cmb, string colorHex)
        {
            try
            {
                if (!string.IsNullOrEmpty(colorHex))
                    cmb.Background = (Brush)new BrushConverter().ConvertFromString(colorHex);
                else
                    cmb.Background = Brushes.White;
            }
            catch { cmb.Background = Brushes.White; }
        }
    }
}
