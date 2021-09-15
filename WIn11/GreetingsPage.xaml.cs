using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using WIn11;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Notes
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class GreetingsPage : Page
    {
        public GreetingsPage()
        {
            this.InitializeComponent();

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage), null);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://github.com/BlkDev9/Notes");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);

            if (success)
            {

            }
            else
            {
               
            }
        }

        private async void gb_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://support.microsoft.com/en-us/windows/-windows-accesso-al-file-system-e-privacy-a7d90b20-b252-0e7b-6a29-a3a688e5c7be#ID0EBD=Windows_10");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);

            if (success)
            {

            }
            else
            {

            }
        }
    }
}
