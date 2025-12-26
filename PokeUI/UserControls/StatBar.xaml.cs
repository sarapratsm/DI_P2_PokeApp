using DependencyPropertyGenerator;
using PokeData.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
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

#nullable enable

namespace PokeUI.UserControls
{

    [DependencyProperty<int>("Valor", DefaultValue = 0, Description = "El valor")]
    [DependencyProperty<string>("StatText", DefaultValue = null, Description = "La estadistica associada")]

    /// <summary>
    /// Lógica de interacción para StatBar.xaml
    /// </summary>
    public partial class StatBar : UserControl
    {
        //private const int MIN = 0;
        //private const int MAX = 255;
        public StatBar()
        {
            InitializeComponent();
        }

        ///No utilitzem aquest mètode perquè trenca el binding! Confiem en que la ProgressBar aplica Minim i Maxim autonomament
        //public void ValorChanged()
        //{
        //    Valor = Math.Clamp(Valor, MIN, MAX);
        //}

        
    }
}
