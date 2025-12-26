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
        private AppDbContext _context = new AppDbContext();
        public DefenseModifiers()
        {
            InitializeComponent();
        }

        public void OnPokemonChanged()
        {
            calcularDebilitats();
        }

        private void calcularDebilitats()
        {
            throw new NotImplementedException();
        }
    }
}
