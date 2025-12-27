using PokeData.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Necesario para el click
using DependencyPropertyGenerator;
using System.Collections.ObjectModel;

namespace PokeUI.UserControls
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon actual")]
    [DependencyProperty<ObservableCollection<Pokemon>>("EvolutionChain", Category = "Data", Description = "Lista de evoluciones")]
    public partial class Evolutions : UserControl
    {
        private AppDbContext context = new AppDbContext();

        public Evolutions()
        {
            InitializeComponent();
        }

        // 1. ESTO SUSTITUYE AL 'LOADED'. Se ejecuta cada vez que cambia el Pokemon.
        partial void OnThePokemonChanged(Pokemon oldValue, Pokemon newValue)
        {
            if (newValue != null)
            {
                CarregarEvolucions();
            }
        }

        // 2. Lógica principal de carga
        private void CarregarEvolucions()
        {
            if (ThePokemon == null) return;

            // Paso A: Obtener el diccionario de relaciones (Hijo -> Padre)
            Dictionary<int, int> relaciones = crearCadenaEvolutiva();

            // Paso B: Sacar todos los IDs implicados
            HashSet<int> idsFamilia = new HashSet<int>();

            // Añadimos SIEMPRE la base actual para evitar listas vacías si no tiene familia
            idsFamilia.Add(buscarBasePokemon());

            foreach (var kvp in relaciones)
            {
                idsFamilia.Add(kvp.Key);   // Hijo
                idsFamilia.Add(kvp.Value); // Padre
            }

            // Paso C: Buscar los objetos completos en la BD
            List<Pokemon> familiaPokemon = context.Pokemons
                                        .Where(p => idsFamilia.Contains(p.PokId))
                                        .OrderBy(p => p.PokId) // Ordenar por ID suele ponerlos en orden evolutivo
                                        .ToList();

            // CORRECCIÓN CRÍTICA: ¡Asignar la lista a la propiedad visual!
            EvolutionChain = new ObservableCollection<Pokemon>(familiaPokemon);
        }

        // --- MÉTODOS RECURSIVOS (Lógica del Árbol) ---

        private Dictionary<int, int> crearCadenaEvolutiva()
        {
            int pokemon_base = buscarBasePokemon();
            Dictionary<int, int> arbol = new Dictionary<int, int>();

            // Llamada recursiva inicial
            OmplirArbreRecursiu(pokemon_base, arbol);

            return arbol;
        }

        private void OmplirArbreRecursiu(int pareId, Dictionary<int, int> diccionari)
        {
            // Busca hijos directos
            List<int> fills = buscarEvos(pareId);

            foreach (int fillId in fills)
            {
                if (!diccionari.ContainsKey(fillId))
                {
                    diccionari.Add(fillId, pareId);
                    // Recursividad: buscar hijos de este hijo
                    OmplirArbreRecursiu(fillId, diccionari);
                }
            }
        }

        private int buscarBasePokemon()
        {
            int currentId = ThePokemon.PokId;
            while (true)
            {
                var info = context.PokemonEvolutionMatchups.FirstOrDefault(pem => pem.PokId == currentId);
                if (info == null || info.EvolvesFromSpeciesId == null) return currentId;
                currentId = info.EvolvesFromSpeciesId.Value;
            }
        }

        private List<int> buscarEvos(int idPokemon)
        {
            return context.PokemonEvolutionMatchups
                  .Where(pem => pem.EvolvesFromSpeciesId == idPokemon)
                  .Select(pem => pem.PokId)
                  .ToList();
        }

        // --- EVENTO CLIC (Navegación) ---
        private void Pokemon_Click(object sender, MouseButtonEventArgs e)
        {
            // Obtenemos el Pokemon del elemento clickeado
            if (sender is FrameworkElement element && element.DataContext is Pokemon clickedPokemon)
            {
                // Al cambiar esto, se dispara OnThePokemonChanged automáticamente y se recarga todo
                this.ThePokemon = clickedPokemon;
            }
        }
    }
}

//public class EvolutionNode
//{
//    public Pokemon Pokemon { get; set; }
//    public List<EvolutionNode> Evolutions { get; set; } = new List<EvolutionNode>();
//    public bool HasEvolutions => Evolutions.Count > 0;
//}