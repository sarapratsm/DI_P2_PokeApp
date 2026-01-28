using Microsoft.EntityFrameworkCore;
using PokeData.Models;
using System.Collections.Generic;
using System.Linq;

namespace PokeUI.Utils
{
    /// <summary>
    /// Clase de utilidad estática para gestionar la lógica de las cadenas evolutivas.
    /// Permite reconstruir familias de Pokémon (Abuelo -> Padre -> Hijo) consultando la base de datos.
    /// </summary>
    public static class EvolutionUtils
    {
        /// <summary>
        /// Devuelve una lista plana con TODOS los miembros de una familia evolutiva,
        /// independientemente de a quién preguntes (ej: si preguntas por Charmeleon, devuelve Charmander, Charmeleon y Charizard).
        /// </summary>
        /// <param name="context">El contexto de base de datos para realizar las consultas.</param>
        /// <param name="pokemonId">El ID de cualquier Pokémon de la familia.</param>
        /// <returns>Lista de objetos Pokémon ordenada por ID.</returns>
        public static List<Pokemon> GetFamiliaCompleta(AppDbContext context, int pokemonId)
        {
            // 1. Encontrar la Raíz: Buscamos al ancestro original (el "Abuelo") de la familia.
            // Esto asegura que siempre empezamos desde el principio de la cadena.
            int rootId = BuscarBasePokemon(context, pokemonId);

            // 2. Recorrer el árbol: Desde el abuelo, bajamos recursivamente buscando hijos y nietos.
            List<Pokemon> familia = new List<Pokemon>();
            RecorrerArbolRecursivo(context, rootId, familia);

            // 3. Ordenar: Devolvemos la lista ordenada por ID para que salga limpia (1, 2, 3...)
            return familia.OrderBy(p => p.PokId).ToList();
        }

        /// <summary>
        /// Método auxiliar que rastrea la cadena evolutiva hacia atrás (hacia los padres)
        /// hasta encontrar al Pokémon original (Stage 1).
        /// </summary>
        private static int BuscarBasePokemon(AppDbContext context, int pokemonId)
        {
            int currentId = pokemonId;

            // Bucle infinito: seguiremos subiendo hasta que no encontremos padre
            while (true)
            {
                var match = context.PokemonEvolutionMatchups
                                   .AsNoTracking() // Optimización: No necesitamos guardar cambios, solo leer.
                                   .FirstOrDefault(pem => pem.PokId == currentId);

                // Si no hay registro o EvolvesFromSpeciesId es null, significa que nadie evoluciona a este Pokémon.
                // Por tanto, este es el Pokémon Base.
                if (match == null || match.EvolvesFromSpeciesId == null)
                {
                    return currentId;
                }

                // Si tiene padre, actualizamos el ID actual al del padre y repetimos el ciclo.
                currentId = match.EvolvesFromSpeciesId.Value;
            }
        }

        /// <summary>
        /// Método recursivo que busca hacia adelante (hacia los hijos).
        /// Añade el Pokémon actual a la lista y se llama a sí mismo para cada evolución encontrada.
        /// </summary>
        private static void RecorrerArbolRecursivo(AppDbContext context, int currentId, List<Pokemon> lista)
        {
            // Cargamos el Pokémon actual incluyendo sus datos básicos y sus evoluciones (hijos)
            var match = context.PokemonEvolutionMatchups
                               .Include(m => m.Pok)               // Include: Para tener acceso al Nombre, Imagen, etc.
                               .Include(m => m.PokemonEvolutions) // Include: Para saber si tiene hijos
                               .FirstOrDefault(m => m.PokId == currentId);

            if (match != null)
            {
                // Evitamos duplicados: solo añadimos si no está ya en la lista
                if (!lista.Any(p => p.PokId == match.Pok.PokId))
                {
                    lista.Add(match.Pok);
                }

                // RECURSIVIDAD: Si tiene hijos (PokemonEvolutions), llamamos a este mismo método
                // para cada uno de los hijos encontrados.
                if (match.PokemonEvolutions != null)
                {
                    foreach (var evo in match.PokemonEvolutions)
                    {
                        // "evo.EvolvedSpeciesId" es el ID del hijo.
                        RecorrerArbolRecursivo(context, evo.EvolvedSpeciesId, lista);
                    }
                }
            }
            else
            {
                // Caso de seguridad (Fallback): 
                // El Pokémon existe en la tabla 'Pokemon' pero no tiene entrada en 'PokemonEvolutionMatchups'.
                // Lo buscamos directamente para no devolver nada vacío.
                var simplePoke = context.Pokemons.Find(currentId);
                if (simplePoke != null && !lista.Any(p => p.PokId == simplePoke.PokId))
                {
                    lista.Add(simplePoke);
                }
            }
        }

        /// <summary>
        /// Organiza la familia evolutiva por etapas (Generaciones).
        /// Útil para la UI, donde queremos mostrar: Stage 1, Stage 2, Stage 3.
        /// </summary>
        /// <returns>Una lista de listas. Cada sub-lista es una etapa evolutiva.</returns>
        public static List<List<Pokemon>> GetEvolucionesPorEtapas(AppDbContext context, int pokemonId)
        {
            var resultado = new List<List<Pokemon>>();

            // Paso 1: Buscar al primer (p.e. charmander) (Stage 1)
            int rootId = BuscarBasePokemon(context, pokemonId);

            // Inicializamos la búsqueda con el ID del primer
            var idsNivelActual = new List<int> { rootId };

            // Bucle: Mientras tengamos Pokémon en el nivel actual para procesar...
            while (idsNivelActual.Count > 0)
            {
                // A. Obtenemos los OBJETOS completos de los Pokémon de este nivel
                var pokemonsNivel = context.Pokemons
                                           .Where(p => idsNivelActual.Contains(p.PokId))
                                           .Include(p => p.PokemonTypes) // Carga los tipos (Fuego, Agua...)
                                              .ThenInclude(pt => pt.Type)
                                           .OrderBy(p => p.PokId)
                                           .ToList();

                // B. Añadimos esta "generación" a la lista de resultados
                if (pokemonsNivel.Count > 0)
                {
                    resultado.Add(pokemonsNivel);
                }

                // C. Buscar la SIGUIENTE generación (Hijos de los actuales)
                // Buscamos en la tabla de Matchups quiénes evolucionan DESDE (EvolvesFromSpeciesId)
                // los IDs que tenemos ahora.
                var idsSiguienteNivel = context.PokemonEvolutionMatchups
                               .Where(m => m.EvolvesFromSpeciesId != null &&
                                           idsNivelActual.Contains(m.EvolvesFromSpeciesId.Value))
                               .Select(m => m.PokId) // Seleccionamos el ID del hijo
                               .ToList();

                // D. Preparamos la siguiente vuelta del bucle
                idsNivelActual = idsSiguienteNivel;
            }

            return resultado;
        }
    }
}