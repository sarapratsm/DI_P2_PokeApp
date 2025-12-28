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
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace PokeUI.UserControls
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon")]
    [DependencyProperty<ObservableCollection<Pokemon>>("EvolutionChain", Category = "OC", Description = "Cadena Evolutiva")]

    /// <summary>
    /// Lógica de interacción para Evolutions.xaml
    /// </summary>
    public partial class Evolutions : UserControl
    {
        int numEvos = 0;
        AppDbContext context = new AppDbContext();

        public Evolutions()
        {
            InitializeComponent();
        }

        private void Evos_Loaded(object sender, RoutedEventArgs e)
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

        private void CarregarEvolucions()
        {
            if (ThePokemon == null) return;

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

    }
}
