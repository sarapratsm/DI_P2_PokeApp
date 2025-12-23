using PokeUI.Pagines;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PokeUI;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    LlistaPrincipal principal = new LlistaPrincipal();

    public MainWindow()
    {
        InitializeComponent();

    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
    }

    private void btnEdit_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {

    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        frm.Navigate(principal);
    }
}