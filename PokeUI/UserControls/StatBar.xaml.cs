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

    //[DependencyProperty<int>("Min", DefaultValue = 0, Description = "Min value", OnChanged = "OnMinChanged")]
    //[DependencyProperty<int>("Max", DefaultValue = 255, Description = "Max value", OnChanged = "OnMaxChanged")]
    [DependencyProperty<int>("Valor", DefaultValue = 0, Description = "El valor", OnChanged = "ValorChanged")]

    /// <summary>
    /// Lógica de interacción para StatBar.xaml
    /// </summary>
    public partial class StatBar : UserControl
    {
        private const int MIN = 0;
        private const int MAX = 255;
        public StatBar()
        {
            InitializeComponent();
        }

        //public void OnMinChanged(int old, int newValue)
        //{
        //    if (Min >= Max)
        //    {
        //        Min = Max - 1;
        //    }
        //    ValorChanged();
        //}

        //public void OnMaxChanged(int old, int newValue)
        //{
        //    if (Min >= Max)
        //    {
        //        Max = Min + 1;
        //    }
        //    ValorChanged();
        //}


        public void ValorChanged()
        {
            Valor = Math.Clamp(Valor, MIN, MAX);
            crearBarra();
        }

        private void crearBarra()
        {
            throw new NotImplementedException();
        }
    }
}
