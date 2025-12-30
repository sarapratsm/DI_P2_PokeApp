using DependencyPropertyGenerator;
using Microsoft.EntityFrameworkCore;
using PokeData.Models;
using System.Linq;
using System.Windows.Controls;

namespace PokeUI.UserControls
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged = "OnPokemonChanged")]

    public partial class DefenseModifiers : UserControl
    {
        private AppDbContext context = new AppDbContext();

        public DefenseModifiers()
        {
            InitializeComponent();
        }

        public void OnPokemonChanged()
        {
            calcularResistencies();
        }

        public void calcularResistencies()
        {
            if (ThePokemon == null) return;

            wrpModifiers.Children.Clear();

            // 1. Leemos los tipos DESDE MEMORIA (lo que acabas de seleccionar)
            var misTiposIds = ThePokemon.PokemonTypes.Select(pt => pt.TypeId).ToList();

            if (misTiposIds.Count == 0) return;

            // 2. Buscamos las reglas de daño en la BD
            var relacions = context.TypeEfficacies
                .Include(te => te.DamageType)
                .Where(te => misTiposIds.Contains(te.TargetTypeId))
                .ToList();

            // 3. Calculamos
            var listaCalculada = relacions
                .GroupBy(te => te.DamageType)
                .Select(grupo => new
                {
                    TipoAtacante = grupo.Key,
                    // Multiplicamos factores (ej: x2 * x2 = x4)
                    Factor = grupo.Aggregate(1.0, (acumulado, te) => acumulado * (te.DamageFactor / 100.0))
                })
                .OrderBy(x => x.TipoAtacante.TypeId)
                .ToList();

            // 4. Pintamos
            foreach (var item in listaCalculada)
            {
                DefenseModifierAux aux = new DefenseModifierAux();

                aux.stringColor = item.TipoAtacante.Color;
                aux.stringType = item.TipoAtacante.TypeName;
                aux.Multiplier = item.Factor;

                wrpModifiers.Children.Add(aux);
            }
        }
    }
}