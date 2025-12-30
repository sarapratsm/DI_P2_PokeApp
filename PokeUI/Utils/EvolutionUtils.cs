using Microsoft.EntityFrameworkCore;
using PokeData.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokeUI.Utils
{
    public static class EvolutionUtils
    {
        /// <summary>
        /// Devuelve la lista completa de la familia (abuelos, padres, hijos, nietos...)
        /// ordenada por ID.
        /// </summary>
        public static List<Pokemon> GetFamiliaCompleta(AppDbContext context, int pokemonId)
        {
            // 1. Buscar al "Abuelo" (Raíz del árbol)
            int rootId = BuscarBasePokemon(context, pokemonId);

            // 2. Recorrer el árbol hacia abajo para encontrar a todos los descendientes
            List<Pokemon> familia = new List<Pokemon>();
            RecorrerArbolRecursivo(context, rootId, familia);

            // 3. Devolver lista ordenada y sin duplicados
            return familia.OrderBy(p => p.PokId).ToList();
        }

        // Busca hacia atrás (Padres) hasta que no haya más
        private static int BuscarBasePokemon(AppDbContext context, int pokemonId)
        {
            int currentId = pokemonId;
            while (true)
            {
                var match = context.PokemonEvolutionMatchups
                                   .AsNoTracking() // Más rápido para lecturas
                                   .FirstOrDefault(pem => pem.PokId == currentId);

                if (match == null || match.EvolvesFromSpeciesId == null)
                {
                    return currentId; // Hemos llegado al inicio
                }
                currentId = match.EvolvesFromSpeciesId.Value;
            }
        }

        // Busca hacia adelante (Hijos) recursivamente
        private static void RecorrerArbolRecursivo(AppDbContext context, int currentId, List<Pokemon> lista)
        {
            // Cargamos el Pokémon y sus hijos directos
            var match = context.PokemonEvolutionMatchups
                               .Include(m => m.Pok) // Para tener Nombre e Imagen
                               .Include(m => m.PokemonEvolutions) // Para saber los hijos
                               .FirstOrDefault(m => m.PokId == currentId);

            if (match != null)
            {
                // Añadimos el actual a la lista si no está ya
                if (!lista.Any(p => p.PokId == match.Pok.PokId))
                {
                    lista.Add(match.Pok);
                }

                // Llamada recursiva para cada hijo
                if (match.PokemonEvolutions != null)
                {
                    foreach (var evo in match.PokemonEvolutions)
                    {
                        RecorrerArbolRecursivo(context, evo.EvolvedSpeciesId, lista);
                    }
                }
            }
            else
            {
                // Caso raro: El Pokémon existe en tabla Pokemon pero no en Matchups
                var simplePoke = context.Pokemons.Find(currentId);
                if (simplePoke != null && !lista.Any(p => p.PokId == simplePoke.PokId))
                {
                    lista.Add(simplePoke);
                }
            }
        }

        // En EvolutionUtils.cs
        public static List<List<Pokemon>> GetEvolucionesPorEtapas(AppDbContext context, int pokemonId)
        {
            var resultado = new List<List<Pokemon>>();
            int rootId = BuscarBasePokemon(context, pokemonId);
            var idsNivelActual = new List<int> { rootId };

            while (idsNivelActual.Count > 0)
            {
                // CAMBIO AQUÍ: Añadimos los Includes para traer los TIPOS también
                var pokemonsNivel = context.Pokemons
                                           .Where(p => idsNivelActual.Contains(p.PokId))
                                           .Include(p => p.PokemonTypes)       // <--- NUEVO
                                              .ThenInclude(pt => pt.Type)      // <--- NUEVO
                                           .OrderBy(p => p.PokId)
                                           .ToList();

                if (pokemonsNivel.Count > 0)
                {
                    resultado.Add(pokemonsNivel);
                }

                // Buscar hijos (igual que antes)
                var idsSiguienteNivel = context.PokemonEvolutionMatchups
                               .Where(m => m.EvolvesFromSpeciesId != null &&
                                           idsNivelActual.Contains(m.EvolvesFromSpeciesId.Value))
                               .Select(m => m.PokId)
                               .ToList();

                idsNivelActual = idsSiguienteNivel;
            }

            return resultado;
        }
    }
}