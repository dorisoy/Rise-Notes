using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Finestra di dialogo contenuto è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Notes.Views
{
    public sealed partial class VersionDialog : ContentDialog
    {
        public VersionDialog()
        {
            this.InitializeComponent();
            if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
            {
                bg.Source = new BitmapImage(new Uri("ms-appx:///Assets/dark.bmp"));
            }
            else
            {
                new BitmapImage(new Uri("ms-appx:///Assets/light.png"));
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAc11dY1UQ1pJWFRWOFA1MDk3MUtVRk5SWVlTQktPWS4u");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);

            if (success)
            {

            }
            else
            {

            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAc11dY1UQ0UxNjFVS0pCUkpKVkpVTUpUSktBRjVKUS4u");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);

            if (success)
            {

            }
            else
            {

            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://forms.office.com/pages/responsepage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAc11dY1URFAxTkRNMEZMVEsxWTlWRE9SRkVHWElOUi4u");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);

            if (success)
            {

            }
            else
            {

            }
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://github.com/BlkDev9/Rise-Notes/releases");

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
