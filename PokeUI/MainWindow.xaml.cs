using PokeData.Models;
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
    PaginaEdicio edicio = new PaginaEdicio();
    int pagina =1;
    int nouPokemon = 2; //0 és un nou pokemon, 1 és una edicio, 2 quan no estem a la pagina correcta.

    public MainWindow()
    {
        InitializeComponent();

    }

    


    /// <summary>
    /// Al principi aquest mètode era més útil...
    /// </summary>
    /// <param name="lloc">1 és la Llista Principal</param>
    private void Navegacio(int lloc)
    {
        if (lloc == 1)
        {
            frm.Navigate(principal);
        }
        else if (lloc == 2)
        {
            frm.Navigate(edicio);
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        Navegacio(1);
    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
        if (frm.NavigationService.CanGoBack)
        {
            frm.NavigationService.GoBack();
            
        }


    }

    private void btnEdit_Click(object sender, RoutedEventArgs e)
    {
        edicio.ThePokemon = principal.ThePokemon;
        nouPokemon = 1;
        Navegacio(2);
    }

    private void btnCancel_Click(object sender, RoutedEventArgs e)
    {

    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
        //Fer un insert o un update a la BBDD
        if (nouPokemon == 0)
        {
            //insert

        }
        else if (nouPokemon == 1)
        {
            //update
        }
        else
        {
            //error.
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        edicio.ThePokemon = new PokeData.Models.Pokemon();
        nouPokemon = 0;
        Navegacio(2);
    }

    public void actualitzaBotons()
    {
        bool canGoBack = frm.NavigationService.CanGoBack;
        bool esPagina1 = (pagina == 1);
        bool esPagina2 = (pagina == 2);

        btnBack.IsEnabled = canGoBack;
        btnBack.Visibility = canGoBack ? Visibility.Visible : Visibility.Collapsed;


        btnAdd.IsEnabled = esPagina1;
        btnAdd.Visibility = esPagina1 ? Visibility.Visible : Visibility.Collapsed;

        btnEdit.IsEnabled = esPagina1;
        btnEdit.Visibility = esPagina1 ? Visibility.Visible : Visibility.Collapsed;

        btnCancel.IsEnabled = esPagina2;
        btnCancel.Visibility = esPagina2 ? Visibility.Visible : Visibility.Collapsed;

        btnSave.IsEnabled = esPagina2;
        btnSave.Visibility = esPagina2 ? Visibility.Visible : Visibility.Collapsed;
    }

    private void frm_Navigated(object sender, NavigationEventArgs e)
    {
        if (frm.Content == principal) pagina = 1;
        if (frm.Content == edicio) pagina = 2;
        actualitzaBotons();
    }

  
}