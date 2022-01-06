using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Il modello di elemento Finestra di dialogo contenuto è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Notes.ViewModels
{
    public sealed partial class ContentDialog1 : ContentDialog
    {
        public ContentDialog1()
        {
            this.InitializeComponent();
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
            var uriBing = new Uri(@"https://github.com/BlkDev9/Notes");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://forms.office.com/pages/responsepage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAc11dY1UMUdKWlVSTE0yN0JKMEpXWkc5T1ZBMkpUWC4u");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://github.com/Rise-Software/Rise-Notes/issues/new/choose");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }

        private void InsiderBt_Click(object sender, RoutedEventArgs e)
        {
            ins.IsOpen = true;
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAc11dY1UMUdKWlVSTE0yN0JKMEpXWkc5T1ZBMkpUWC4u");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }

        private async void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://forms.office.com/Pages/ResponsePage.aspx?id=DQSIkWdsW0yxEjajBLZtrQAAAAAAAAAAAANAAc11dY1UQ0UxNjFVS0pCUkpKVkpVTUpUSktBRjVKUS4u");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            ver.IsOpen = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            DataPackage dataPackage = new DataPackage();
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText("Internal Version: 0.0.1.0");
            Clipboard.SetContent(dataPackage);
        }

        private async void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var uriBing = new Uri(@"https://github.com/BlkDev9/Rise-Notes/releases");

            var success = await Windows.System.Launcher.LaunchUriAsync(uriBing);
        }
    }
}
