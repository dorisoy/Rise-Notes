using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Il modello di elemento Finestra di dialogo contenuto è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Notes.Views
{
    public sealed partial class TermsDialog : ContentDialog
    {
        public TermsDialog()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.InitializeComponent();
            SecondaryButtonText = resourceLoader.GetString("/Setup/textBut2/SecondaryButtonText");
        }

        bool secondPart = true;

        IPropertySet roamingProperties = ApplicationData.Current.RoamingSettings.Values;

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            //PrimaryButtonText = resourceLoader.GetString("/Setup/Cont/PrimaryButtonText");
            GridLicense.Visibility = Visibility.Collapsed;
            GridFile.Visibility = Visibility.Visible;
            if (secondPart == false)
            {
                //roamingProperties["HasBeenHereBefore"] = bool.FalseString;
                secondPart = true;
                //prg.Value = 50;
            }
            else
            {
                roamingProperties["HasBeenHereBefore"] = bool.FalseString;
                roamingProperties["FileAccess"] = bool.TrueString;
                bool result = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-broadfilesystemaccess"));
                //prg.Value = 100;
            }

        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (secondPart == true)
            {
                this.Closing += ContentDialog_Closing2;
                roamingProperties["HasBeenHereBefore"] = bool.FalseString;
            }
            else
            {
                Application.Current.Exit();
            }
        }
        private void ContentDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            args.Cancel = true;
        }

        private void ContentDialog_Closing2(ContentDialog sender, ContentDialogClosingEventArgs args)
        {
            args.Cancel = false;
        }
    }
}
