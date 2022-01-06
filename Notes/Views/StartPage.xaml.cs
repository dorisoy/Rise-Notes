using Microsoft.UI.Xaml.Controls;
using Notes.Model;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Notes
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            startApp();
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(CustomDragRegion);
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
                ShellTitlebarInset.MinWidth = sender.SystemOverlayRightInset;
            }
            CustomDragRegion.Height = ShellTitlebarInset.Height = sender.Height;
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                AppTitleBar.Visibility = Visibility.Visible;
            }
        }

        public void TabView_AddTabButtonClick(TabView sender, object args)
        {
            TxtTab(); 
        }

        public void startApp()
        {
            TabViewItem newTab = new TabViewItem();
            Frame frame = new Frame();
            newTab.Header = "Text document 1";
            newTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Style = (Style)this.Resources["TabViewItemStyle1"];
            newTab.Content = frame;
            frame.Navigate(typeof(MainPage));
            Tabs.TabItems.Add(newTab);
        }

        public void TxtTab()
        {
            TabViewItem newTab = new TabViewItem();
            Frame frame = new Frame();
            newTab.Content = frame;
            newTab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document };
            newTab.Header = "Text document " + (Tabs.TabItems.Count + 1);
            newTab.Style = (Style)this.Resources["TabViewItemStyle1"];
            frame.Navigate(typeof(MainPage));
            Tabs.TabItems.Add(newTab);
            Tabs.SelectedItem = newTab;
        }

        public void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
            if (sender.TabItems.Count < 1)
            {
                ApplicationView.GetForCurrentView().TryConsolidateAsync();
            }
        }
    }
}
