using PokeData.Models;
using PokeUI.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows; // IMPORTANTE: Para que funcione 'Visibility'
using System.Windows.Controls;
using DependencyPropertyGenerator;

namespace PokeUI.UserControls
{
    // Propiedad que recibe el Pokémon desde la página principal
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged = "OnThePokemonChanged")]

    // Propiedad que usa el XAML para pintar la lista. 
    // Fíjate que usa nuestra clase auxiliar 'EvolutionStage' (definida abajo del todo).
    [DependencyProperty<ObservableCollection<EvolutionStage>>("EvolutionStages", Category = "OC", Description = "Etapas Evolutivas")]

    public partial class Evolutions : UserControl
    {
        AppDbContext context = new AppDbContext();

        public Evolutions()
        {
            InitializeComponent();
        }

        // Se ejecuta automáticamente cuando cambia el Pokémon
        public void OnThePokemonChanged()
        {
            if (ThePokemon == null || context == null) return;

            // 1. Pedimos a la base de datos la familia completa organizada por niveles
            // (Esta parte asume que tu EvolutionUtils.cs ya tiene la corrección de búsqueda inversa que hablamos)
            var etapasRaw = EvolutionUtils.GetEvolucionesPorEtapas(context, ThePokemon.PokId);

            // 2. Preparamos la lista visual
            var etapasVisuales = new ObservableCollection<EvolutionStage>();

            for (int i = 0; i < etapasRaw.Count; i++)
            {
                // Creamos la "caja" para esta etapa
                var etapa = new EvolutionStage();

                // Guardamos la lista de Pokémons (ej: Vaporeon, Jolteon...)
                etapa.Pokemons = etapasRaw[i];

                // LÓGICA DE LA FLECHA:
                // Si i > 0 (es la 2ª, 3ª columna...), mostramos la flecha.
                // Si i == 0 (es el primer Pokémon), la ocultamos (Collapsed).
                if (i > 0)
                    etapa.ArrowVisibility = Visibility.Visible;
                else
                    etapa.ArrowVisibility = Visibility.Collapsed;

                etapasVisuales.Add(etapa);
            }

            // 3. Asignamos la lista preparada a la propiedad que lee el XAML
            EvolutionStages = etapasVisuales;
        }
    }

    // --- CLASE AUXILIAR (MODELO VISUAL) ---
    // La definimos aquí para no tener que crear otro archivo.
    // El XAML buscará las propiedades "Pokemons" y "ArrowVisibility" dentro de esta clase.
    public class EvolutionStage
    {
        public List<Pokemon> Pokemons { get; set; }
        public Visibility ArrowVisibility { get; set; }
    }
}