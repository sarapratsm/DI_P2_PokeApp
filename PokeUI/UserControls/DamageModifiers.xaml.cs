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
using PokeData;

namespace PokeUI.UserControls
{

    /// <summary>
    /// Lógica de interacción para DamageModifiers.xaml
    /// </summary>
    public partial class DamageModifiers : UserControl
    {



        public DamageModifiers()
        {
            InitializeComponent();      
        }




        public string MultiplierText
        {
            get { return (string)GetValue(MultiplierTextProperty); }
            set { SetValue(MultiplierTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MultiplierText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MultiplierTextProperty =
            DependencyProperty.Register("MultiplierText", typeof(string), typeof(DamageModifiers), new PropertyMetadata(""));



        public List<PokeData.Models.Type> TypeList
        {
            get { return (List<PokeData.Models.Type>)GetValue(TypeListProperty); }
            set { SetValue(TypeListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TypeList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeListProperty =
            DependencyProperty.Register("TypeList", typeof(List<PokeData.Models.Type>), typeof(DamageModifiers), new PropertyMetadata(null));


    }


}
