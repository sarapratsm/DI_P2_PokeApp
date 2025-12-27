using DependencyPropertyGenerator;
using Microsoft.EntityFrameworkCore;
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

namespace PokeUI.UserControls
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon")]


    /// <summary>
    /// Lógica de interacción para FitxaCompleta.xaml
    /// </summary>
    public partial class FitxaCompleta : UserControl
    {
        AppDbContext context = new AppDbContext();
        public FitxaCompleta()
        {
            InitializeComponent();


        }

        private void Fitxa_Loaded(object sender, RoutedEventArgs e)
        {
            omplirStkDamageModifiers();
        }


        partial void OnThePokemonChanged(Pokemon oldValue, Pokemon newValue)
        {
            // Solo si tenemos un pokemon nuevo válido, recargamos la info
            if (newValue != null)
            {
                omplirStkDamageModifiers();
            }
        }

        /// <summary>
        /// Populates the UI container with damage modifier controls based on the Pokémon's type effectiveness.
        /// </summary>
        /// <remarks>This method clears the existing damage modifier controls and adds new ones that
        /// represent how effective different attack types are against the current Pokémon, according to its types. Only
        /// modifiers that differ from the standard (x1) are displayed, grouped and labeled by their effectiveness
        /// multiplier (e.g., x2, x0.5).</remarks>
        private void omplirStkDamageModifiers()
        {
            if (ThePokemon == null) return;

            stkDamageModifiers.Children.Clear();

            //Calcular la llista de tipus amb cada modificador (x4, x2, x0.5 i x.0.25)
            //var tipusDelPokemon = ThePokemon.PokemonTypes.ToList();
            var tipusDelPokemon = context.PokemonTypes
                                  .Where(pt => pt.PokId == ThePokemon.PokId)
                                  .Select(pt => pt.TypeId)
                                  .ToList();

            if (tipusDelPokemon.Count == 0) return;

            //Buscar les relacions amb els altres tipus
            var relacions = context.TypeEfficacies
                .Include(te => te.DamageType)
                .Where(te => tipusDelPokemon.Contains(te.TargetTypeId))
                .ToList();

            var listaCalculada = relacions
                .GroupBy(te => te.DamageType) // Agrupamos por el tipo que nos ataca
                .Select(grupo => new
                {
                    TipoAtacante = grupo.Key, // El objeto Tipo (Roca)

                    // Calculamos el factor final multiplicando todos los daños recibidos
                    Factor = grupo.Aggregate(1.0, (acumulado, te) => acumulado * (te.DamageFactor / 100.0))
                })
                .Where(x => x.Factor != 1.0) // Quitamos los x1 (daño normal) porque aburren
                .OrderByDescending(x => x.Factor) // Ordenamos: x4, x2, x0.5, x0
                .ToList();

            //Crear dinamicament cada UC DamageModifiers i enxufar-li la llista de cada tipus
            var gruposParaPintar = listaCalculada.GroupBy(x => x.Factor);

            foreach (var grupo in gruposParaPintar)
            {
                var damageControl = new DamageModifiers();
                // Le ponemos el texto: x4, x2, x0.25...
                damageControl.MultiplierText = "x" + grupo.Key.ToString("0.##");
                // Le pasamos la lista de tipos de ese grupo (para que pinte las chapitas)
                damageControl.TypeList = grupo.Select(x => x.TipoAtacante).ToList();
                //Enxufar els DamageModifiers al stkDamageModifiers
                stkDamageModifiers.Children.Add(damageControl);
            }


        }


    }
}
