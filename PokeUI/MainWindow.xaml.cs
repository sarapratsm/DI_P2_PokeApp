using PokeUI.Pagines;
using System.Windows;
using System.Windows.Navigation;

namespace PokeUI;

/// <summary>
/// Lògica d'interacció per a MainWindow.xaml.
/// Actua com a contenidor principal de l'aplicació i gestiona el Frame de navegació.
/// </summary>
public partial class MainWindow : Window
{
    // Instàncies de les pàgines (es mantenen en memòria per preservar l'estat bàsic)
    LlistaPrincipal principal = new LlistaPrincipal();
    PaginaEdicio edicio = new PaginaEdicio();

    // Variables de control d'estat de navegació
    int pagina = 1;     // 1: Llista, 2: Edició
    int nouPokemon = 2; // 0: Nou, 1: Edició, 2: Indefinit

    public MainWindow()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Mètode auxiliar per centralitzar la navegació entre pàgines.
    /// </summary>
    /// <param name="lloc">1 per anar a la Llista Principal, 2 per anar a l'Edició.</param>
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

    /// <summary>
    /// Esdeveniment que es dispara quan la finestra s'ha carregat completament.
    /// Inicialitza la vista mostrant la llista principal.
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        frm.Navigate(principal);
    }

    /// <summary>
    /// Esdeveniment que es dispara cada vegada que el Frame completa una navegació.
    /// Serveix per actualitzar l'estat intern de la finestra segons quina pàgina es mostra.
    /// </summary>
    private void frm_Navigated(object sender, NavigationEventArgs e)
    {
        if (frm.Content == principal) pagina = 1;
        if (frm.Content == edicio) pagina = 2;
    }
}