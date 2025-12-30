using PokeData.Models;
using PokeUI.Utils; // <--- Importante para usar tu nueva clase
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DependencyPropertyGenerator;

namespace PokeUI.UserControls
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged = "OnThePokemonChanged")]
    [DependencyProperty<ObservableCollection<Pokemon>>("EvolutionChain", Category = "OC", Description = "Cadena Evolutiva")]

    public partial class Evolutions : UserControl
    {
        AppDbContext context = new AppDbContext();

        public Evolutions()
        {
            InitializeComponent();
        }

        // Si cambia el Pokémon desde fuera, recargamos
        public void OnThePokemonChanged()
        {
            CargarEvolucions();
        }

        private void CargarEvolucions()
        {
            if (ThePokemon == null || context == null) return;

            // 1. LLAMADA MÁGICA: Usamos el Utils compartido
            // Esto busca abuelos, hijos, nietos y nos da la lista ordenada.
            var familia = EvolutionUtils.GetFamiliaCompleta(context, ThePokemon.PokId);

            // 2. Asignar a la vista (Convertimos List a ObservableCollection)
            EvolutionChain = new ObservableCollection<Pokemon>(familia);
        }
    }
}