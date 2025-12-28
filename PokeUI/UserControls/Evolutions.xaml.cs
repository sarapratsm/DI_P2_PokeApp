using PokeData.Models;
<<<<<<< HEAD
using System;
=======
>>>>>>> 8d86b033637667f410cdd0ce56677e06909b0124
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
<<<<<<< HEAD
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DependencyPropertyGenerator;
using Microsoft.EntityFrameworkCore;
=======
using System.Windows.Input; // Necesario para el click
using DependencyPropertyGenerator;
>>>>>>> 8d86b033637667f410cdd0ce56677e06909b0124
using System.Collections.ObjectModel;

namespace PokeUI.UserControls
{
<<<<<<< HEAD
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged ="OnThePokemonChanged")]
    [DependencyProperty<ObservableCollection<Pokemon>>("EvolutionChain", Category = "OC", Description = "Cadena Evolutiva")]

    /// <summary>
    /// Lógica de interacción para Evolutions.xaml
    /// </summary>
    public partial class Evolutions : UserControl
    {
        int numEvos = 0;
        AppDbContext context = new AppDbContext();
=======
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon actual")]
    [DependencyProperty<ObservableCollection<Pokemon>>("EvolutionChain", Category = "Data", Description = "Lista de evoluciones")]
    public partial class Evolutions : UserControl
    {
        private AppDbContext context = new AppDbContext();
>>>>>>> 8d86b033637667f410cdd0ce56677e06909b0124

        public Evolutions()
        {
            InitializeComponent();
        }

<<<<<<< HEAD
        private void Evos_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void OnThePokemonChanged()
        {
            if (ThePokemon == null) return;
            CarregarEvolucions();
        }


        /// <summary>
        /// Busca l'id del pokemon base de la linea evolutiva, afagant com a referencia ThePokemon.
        /// </summary>
        /// <returns>id del pokemon base de la linea evolutiva</returns>
        private int buscarBasePokemon()
        {
            int pokemonId = ThePokemon.PokId;

            while (true)
            {
                var poke_pem = context.PokemonEvolutionMatchups
                                           .FirstOrDefault(pem => pem.PokId == pokemonId);

                if (poke_pem == null || poke_pem.EvolvesFromSpeciesId == null)
                {
                    return pokemonId;
                }
                pokemonId = poke_pem.EvolvesFromSpeciesId.Value;
            }

            //int? preevo = null;
            //int aux_id = ThePokemon.PokId;
            //Pokemon aux_poke = context.Pokemons.Where(p => p.PokId == ThePokemon.PokId).Include(p=>p.PokemonEvolutionMatchup).FirstOrDefault();
            //if (aux_poke.PokemonEvolutionMatchup.EvolvesFromSpeciesId == null)
            //{
            //    return null;         
            //}
            //do
            //{
            //    //obtenir l'id de la preevo
            //    preevo = aux_poke.PokemonEvolutionMatchup.EvolvesFromSpeciesId;
            //    aux_poke = context.Pokemons.Where(p => p.PokId == preevo).Include(p => p.PokemonEvolutionMatchup).FirstOrDefault();
            //} while (aux_poke.PokemonEvolutionMatchup.EvolvesFromSpeciesId != null);
            //return preevo;
        }

        /// <summary>
        /// Mètode per crear la cadena evolutiva en forma de diccionari.
        /// <returns>Un diccionari de relacions entre evolucions, on la key és l'ID del pokemon al que s'ha 
        /// evolucionat i el value és l'ID del pokemon del que prové. </returns>
        /// </summary>
        private Dictionary<int, int> crearCadenaEvolutiva()
        {
            int pokemon_base = buscarBasePokemon();
            Dictionary<int, int> k_evoId_v_preEvo = new Dictionary<int, int>();
            OmplirArbreRecursiu(pokemon_base, k_evoId_v_preEvo);
            return k_evoId_v_preEvo;
        }

        /// <summary>
        /// Mètode recursiu: Busca els fills del pare actual, els afegeix i repeteix el procés.
        /// </summary>
        private void OmplirArbreRecursiu(int pareId, Dictionary<int, int> diccionari)
        {
            List<int> fills = buscarEvos(pareId);

            foreach (int fillId in fills)
            {
                // KEY (Fill) -> VALUE (Pare)
                if (!diccionari.ContainsKey(fillId))
                {
                    diccionari.Add(fillId, pareId);
                }
                OmplirArbreRecursiu(fillId, diccionari);
            }
        }

        private List<int> buscarEvos(int pokemon_id)
        {
            return context.PokemonEvolutionMatchups
                .Where(pem => pem.EvolvesFromSpeciesId == pokemon_id)
                .Select(pem => pem.PokId)
                .ToList();
        }

=======
        // 1. ESTO SUSTITUYE AL 'LOADED'. Se ejecuta cada vez que cambia el Pokemon.
        partial void OnThePokemonChanged(Pokemon oldValue, Pokemon newValue)
        {
            if (newValue != null)
            {
                CarregarEvolucions();
            }
        }

        // 2. Lógica principal de carga
>>>>>>> 8d86b033637667f410cdd0ce56677e06909b0124
        private void CarregarEvolucions()
        {
            if (ThePokemon == null) return;

<<<<<<< HEAD
            Dictionary<int, int> relaciones = crearCadenaEvolutiva();

            // 2. Extraemos TODOS los IDs únicos que participan en la familia
            // (Tanto los hijos (Keys) como los padres (Values))
            HashSet<int> IdsFamilia = new HashSet<int>();
            foreach (var kvp in relaciones)
            {
                IdsFamilia.Add(kvp.Key);   // El hijo
                IdsFamilia.Add(kvp.Value); // El padre
            }

            // Si la lista está vacía, es que es un pokemon sin familia (o es la base sola), 
            // añadimos al propio pokemon para que al menos salga él.
            if (IdsFamilia.Count == 0) IdsFamilia.Add(ThePokemon.PokId);

            List<Pokemon> familiaPokemon = context.Pokemons
                                        .Where(p => IdsFamilia.Contains(p.PokId))
                                        .OrderBy(p => p.PokId)
                                        .ToList();

            //familiaPokemon = familiaPokemon.OrderBy(p => p.PokId).ToList();

            // 5. Asignar a la vista
            EvolutionChain = new ObservableCollection<Pokemon>(familiaPokemon);
        }

=======
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
>>>>>>> 8d86b033637667f410cdd0ce56677e06909b0124
        private void Pokemon_Click(object sender, MouseButtonEventArgs e)
        {
            // Obtenemos el Pokemon del elemento clickeado
            if (sender is FrameworkElement element && element.DataContext is Pokemon clickedPokemon)
            {
                // Al cambiar esto, se dispara OnThePokemonChanged automáticamente y se recarga todo
                this.ThePokemon = clickedPokemon;
            }
        }
<<<<<<< HEAD

=======
>>>>>>> 8d86b033637667f410cdd0ce56677e06909b0124
    }
}

//public class EvolutionNode
//{
//    public Pokemon Pokemon { get; set; }
//    public List<EvolutionNode> Evolutions { get; set; } = new List<EvolutionNode>();
//    public bool HasEvolutions => Evolutions.Count > 0;
//}