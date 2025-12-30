using System.Windows;
using System.Windows.Controls;
using DependencyPropertyGenerator;

namespace PokeUI.UserControls
{
    [DependencyProperty<int>("Valor", DefaultValue = 0, Description = "El valor", OnChanged ="OnValorChanged")]
    [DependencyProperty<string>("StatText", DefaultValue = null, Description = "La estadistica associada")]

    /// <summary>
    /// Lógica de interacción para SlidingStatBar.xaml
    /// </summary>
    public partial class SlidingStatBar : UserControl
    {
        public SlidingStatBar()
        {
            InitializeComponent();
        }

        private void SlidingBar_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void sld_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Valor != (int)sld.Value)
            {
                Valor = (int)sld.Value;
            }
        }

        public void OnValorChanged()
        {
            //Comprovacions validesa
            if (Valor > 255)
            {
                Valor = 255;
            }
            else if (Valor < 0)
            {
                Valor = 0;
            }
            //Actualitzar slider
            if ((int)sld.Value != Valor)
            {
                sld.Value = Valor;
            }
        }

      
    }
}
