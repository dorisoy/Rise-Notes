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
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Pickers;
using System.Threading.Tasks;
using Windows.UI;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x410

namespace WIn11
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    /// 


    
    public sealed partial class MainPage : Page
    {
        bool changed;

        
        public MainPage()
        {
            this.InitializeComponent();

            

            if (String.IsNullOrWhiteSpace(textbox.Text))
            {
                changed = false;
            }
            else
            {
                changed = true;
            }

            var titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async (sender, args) =>
            {
                args.Handled = true;

                var curView = CoreApplication.GetCurrentView();
                var newWindow = curView.CoreWindow;


                if (changed == true)
                {
                    
                    ContentDialog saveDialog = new ContentDialog
                    {
                        Title = "Exit without saving the edits?",
                        Content = "Are you sure you want to exit without saving your progress?",
                        CloseButtonText = "No",
                        SecondaryButtonText = "Yes",
                        DefaultButton = ContentDialogButton.Close
                    };

                    var result = await saveDialog.ShowAsync();
                    if (result == ContentDialogResult.Secondary)
                    {
                        Application.Current.Exit();
                    }
                    else
                    {
                        SaveExit();

                    }
                }
                else
                {
                    Application.Current.Exit();
                }

            };
                

            // Hide default title bar.
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // Set XAML element as a draggable region.
            Window.Current.SetTitleBar(AppTitleBar);

            // Register a handler for when the size of the overlaid caption control changes.
            // For example, when the app moves to a screen with a different DPI.
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // Register a handler for when the title bar visibility changes.
            // For example, when the title bar is invoked in full screen mode.
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            //Register a handler for when the window changes focus
            Window.Current.Activated += Current_Activated;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // Update title bar control size as needed to account for system size changes.
            AppTitleBar.Height = coreTitleBar.Height;

            // Ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
            else
            {
                AppTitleBar.Visibility = Visibility.Collapsed;
            }
        }

        // Update the TitleBar based on the inactive/active state of the app
        private void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                AppTitle.Foreground = inactiveForegroundBrush;
            }
            else
            {
                AppTitle.Foreground = defaultForegroundBrush;
            }
        }

        // Update the TitleBar content layout depending on NavigationView DisplayMode
        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            // If the back button is not visible, reduce the TitleBar content indent.
            

            Thickness currMargin = AppTitleBar.Margin;

            // Set the TitleBar margin dependent on NavigationView display mode
            if (sender.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Top)
            {
                AppTitleBar.Margin = new Thickness(topIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness(minimalIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
            else
            {
                AppTitleBar.Margin = new Thickness(expandedIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }
        }

       

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            textbox.Text = "";
            changed = false;
            Edit.Opacity = 0;
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.Title = "New Document";
        }

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".txt");
            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            using (var inputStream = await file.OpenReadAsync())
            using (var classicStream = inputStream.AsStreamForRead())
            using (var streamReader = new StreamReader(classicStream))
            {
                while (streamReader.Peek() >= 0)
                {
                    textbox.Text=(string.Format("{0}", streamReader.ReadLine()));
                }
            }
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.Title = file.Name;
            
        }

        private async void Save()
        {
            FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedFileName = "new text";
            picker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {

                CachedFileManager.DeferUpdates(file);
                string fileContent = textbox.Text;
                await FileIO.WriteTextAsync(file, fileContent);

                FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
                if (updateStatus == FileUpdateStatus.Complete)
                {

                }
                else
                {

                }
            }
            else
            {

            }
            changed = false;
            if (changed == true)
            {
                Edit.Opacity = 0.6;
            }
            else
            {
                Edit.Opacity = 0;
            }
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            appView.Title = file.Name;
        }

        private async void SaveExit()
        {
            FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
            picker.SuggestedFileName = "new text";
            picker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
            StorageFile file = await picker.PickSaveFileAsync();
            if (file != null)
            {

                CachedFileManager.DeferUpdates(file);
                string fileContent = textbox.Text;
                await FileIO.WriteTextAsync(file, fileContent);

                FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
                if (updateStatus == FileUpdateStatus.Complete)
                {

                }
                else
                {

                }
            }
            else
            {

            }

            Application.Current.Exit();
            
        }

        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            Save();
            
        }
        

        private void textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            changed = true;
            if (changed == true)
            {
                Edit.Opacity = 0.6;
            }
            else
            {
                Edit.Opacity = 0;
            }
        }

        public static async Task<string> ShowAddDialogAsync(string title)
        {
            var inputTextBox = new TextBox { AcceptsReturn = false };
            (inputTextBox as FrameworkElement).VerticalAlignment = VerticalAlignment.Bottom;
            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = title,
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
                return inputTextBox.Text;
            else
                return "Segoe UI";
        }

        private async void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            textbox.FontFamily = new FontFamily(await ShowAddDialogAsync("What Font?"));
        }

        private async void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            ContentDialog AboutDialog = new ContentDialog
            {
                Title = "Notes",
                Content = "preAlpha 1.0",
                CloseButtonText = "Ok!",
                DefaultButton = ContentDialogButton.Close
            };

            var result = await AboutDialog.ShowAsync();
        }

        private void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            textbox.FontSize = (textbox.FontSize + 1);
        }

        private void MenuFlyoutItem_Click_6(object sender, RoutedEventArgs e)
        {
            textbox.FontSize = (textbox.FontSize - 1);
        }

        private async void compactOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(250, 310);
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);
            exitbutt.Visibility = Visibility.Visible;
            menuBar.Visibility = Visibility.Collapsed;
            AppFontIcon.Visibility = Visibility.Collapsed;
            AppTitle.HorizontalAlignment = HorizontalAlignment.Center;
            Thickness margin = AppTitle.Margin;
            margin.Left = 40;
            AppTitle.Margin = margin;
            Thickness marginEdit = Edit.Margin;
            Edit.Margin = marginEdit;
            marginEdit.Left = 33;
        }
        private async void standardModeButton_Click(object sender, RoutedEventArgs e)
        {
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            exitbutt.Visibility = Visibility.Collapsed;
            menuBar.Visibility = Visibility.Visible;
            Thickness margin = AppTitle.Margin;
            Thickness marginEdit = Edit.Margin;
            margin.Left = 15;
            marginEdit.Left = 8;
            AppTitle.Margin = margin;
            Edit.Margin = marginEdit;
            AppFontIcon.Visibility = Visibility.Visible;
        }
    }
}
