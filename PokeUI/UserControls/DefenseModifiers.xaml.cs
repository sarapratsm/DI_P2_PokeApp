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
using Microsoft.EntityFrameworkCore;
using PokeData.Models;
using DependencyPropertyGenerator;


namespace PokeUI.UserControls
{
    [DependencyProperty<Pokemon>("ThePokemon", Category = "Pokemon", Description = "El pokemon", OnChanged ="OnPokemonChanged")]

    /// <summary>
    /// Lógica de interacción para DefenseModifiers.xaml
    /// </summary>
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

            // --- CORRECCIÓN ---
            // ANTES: Leías de la BD (context.PokemonTypes...), que tiene datos viejos.
            // AHORA: Lees de la memoria (ThePokemon.PokemonTypes), que tiene lo que acabas de seleccionar en el combo.
            var misTiposIds = ThePokemon.PokemonTypes.Select(pt => pt.TypeId).ToList();

            if (misTiposIds.Count == 0) return;

            // Las reglas de efectividad SÍ las pedimos a la BD (porque son fijas)
            var relacions = context.TypeEfficacies
                .Include(te => te.DamageType)
                .Where(te => misTiposIds.Contains(te.TargetTypeId))
                .ToList();

            var listaCalculada = relacions
                .GroupBy(te => te.DamageType)
                .Select(grupo => new
                {
                    TipoAtacante = grupo.Key,
                    Factor = grupo.Aggregate(1.0, (acumulado, te) => acumulado * (te.DamageFactor / 100.0))
                })
                .OrderByDescending(x => x.Factor)
                .ToList();

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
