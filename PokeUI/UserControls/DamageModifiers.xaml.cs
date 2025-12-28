using System.Windows;
using System.Windows.Controls;

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



        //private void generarEtiquetesTipus()
        //{
        //    // A. Limpiamos las etiquetas del Pokémon anterior
        //    stkTypes.Children.Clear();


        //    // B. Para cada tipo, creamos los controles visuales
        //    foreach (var t in TypeList)
        //    {
        //        if (t == null) continue;

        //        // --- 1. Crear el Borde (La "chapita") ---
        //        Border borde = new Border();
        //        borde.CornerRadius = new CornerRadius(3);
        //        borde.Margin = new Thickness(0, 0, 5, 0);
        //        borde.Padding = new Thickness(8, 2, 8, 2);

        //        // --- 2. Asignar el Color desde la BD ---
        //        try
        //        {
        //            // Si el color viene como Hexadecimal ("#FF0000" o "Red")
        //            if (!string.IsNullOrEmpty(t.Color))
        //            {
        //                var converter = new BrushConverter();
        //                borde.Background = (Brush)converter.ConvertFromString(t.Color);
        //            }
        //            else
        //            {
        //                borde.Background = Brushes.Gray; // Fallback si es nulo
        //            }
        //        }
        //        catch
        //        {
        //            borde.Background = Brushes.Gray; // Fallback si el formato del color es inválido
        //        }

        //        // --- 3. Crear el Texto ---
        //        TextBlock texto = new TextBlock();
        //        texto.Text = t.TypeName; // El nombre del tipo
        //        texto.Foreground = Brushes.White;
        //        texto.FontSize = 11;
        //        texto.FontWeight = FontWeights.SemiBold;

        //        // --- 4. Ensamblar ---
        //        borde.Child = texto;          // Meter texto en borde
        //        stkTypes.Children.Add(borde); // Meter borde en el panel XAML
        //    }
        //}
    }


}
