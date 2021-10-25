using Microsoft.UI.Xaml.Controls;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Notes.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class AtAGlance : Page
    {
        public AtAGlance()
        {
            this.InitializeComponent();
            _new.Translation += new Vector3(0, 0, 120);
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var tb = new StartPage();
            tb.TxtTab();
        }
    }
}
