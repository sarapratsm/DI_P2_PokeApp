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

            var tipusDelPokemon = context.PokemonTypes
                                  .Where(pt => pt.PokId == ThePokemon.PokId)
                                  .Select(pt => pt.TypeId)
                                  .ToList();

            if (tipusDelPokemon.Count == 0) return;

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
                .OrderByDescending(x => x.Factor) // Ordenamos: x4, x2, x0.5, x0
                .ToList();
            //.Where(x => x.Factor != 1.0) // Quitamos los x1 (daño normal) porque aburren

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
