using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics.Printing;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Printing;

namespace Notes
{
    public sealed partial class MainPage : Page
    {
        bool changed = false;
        public MainPage()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            this.InitializeComponent();

            var view = ApplicationView.GetForCurrentView();

            IPropertySet roamingProperties = ApplicationData.Current.RoamingSettings.Values;
            if (roamingProperties.ContainsKey("FileAccess"))
            {
                Fa.IsEnabled = false;
            }
            else
            {
                Fa.IsEnabled = true;
            }

            Color cl = txt.Document.Selection.CharacterFormat.ForegroundColor;

            this.inkCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse |
                                                      CoreInputDeviceTypes.Pen |
                                                      CoreInputDeviceTypes.Touch;
            InkDrawingAttributes drawingAttributes = new InkDrawingAttributes();
            drawingAttributes.Color = Colors.White;
            drawingAttributes.IgnorePressure = false;
            drawingAttributes.FitToCurve = true;

            inkCanvas.InkPresenter.InputProcessingConfiguration.RightDragAction =
            InkInputRightDragAction.LeaveUnprocessed;

            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(drawingAttributes);



            Painter.Translation += new Vector3(0, 0, 152);
            gridTools.Translation += new Vector3(0, 0, 132);
            Tip.Title = resourceLoader.GetString("EdTip/Title");
            Tip.Subtitle = resourceLoader.GetString("EditTip/Subtitle");

            Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async (sender, args) =>
            {
                args.Handled = true;

                var curView = CoreApplication.GetCurrentView();
                var newWindow = curView.CoreWindow;

                if (changed == true)
                {
                    try
                    {
                        ContentDialog saveDialog = new ContentDialog
                        {
                            Title = resourceLoader.GetString("ExitSav"),
                            Content = resourceLoader.GetString("ExitDesc"),
                            PrimaryButtonText = resourceLoader.GetString("SaveExitFile"),
                            SecondaryButtonText = resourceLoader.GetString("Exit"),
                            CloseButtonText = resourceLoader.GetString("Stop"),
                            DefaultButton = ContentDialogButton.Primary
                        };
                        var result = await saveDialog.ShowAsync();
                        if (result == ContentDialogResult.Secondary)
                        {
                            Application.Current.Exit();
                        }
                        if (result == ContentDialogResult.Primary)
                        {
                            SaveExit();
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                else
                {
                    Application.Current.Exit();
                }

            };

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            UpdateTitleBarLayout(coreTitleBar);

            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            AppTitleBar.Height = coreTitleBar.Height;

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

            }
        }

        bool startedApp = true;

        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            const int topIndent = 16;
            const int expandedIndent = 48;
            int minimalIndent = 104;

            Thickness currMargin = AppTitleBar.Margin;

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

        string fileType = ".txt";
        string textMain = "";

        private async void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            Windows.Storage.Pickers.FileOpenPicker open =
                new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".txt");
            open.FileTypeFilter.Add(".rtf");
            txt.TextDocument.ClearUndoRedoHistory();
            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    fileName.Text = file.Name;
                    fileType = ".rtf";
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    var reader = DataReader.FromBuffer(buffer);
                    reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string text = reader.ReadString(buffer.Length);
                    txt.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);
                    txt.Document.GetText(Windows.UI.Text.TextGetOptions.UseObjectText, out textMain);
                    richTmp.Text = textMain;
                    startedApp = false;
                    changed = false;
                    if (fileType == file.FileType)
                    {
                        fileType = ".rtf";
                        fileTypeBar.Text = "Rich Text Document (.rtf)";
                    }
                    else
                    {
                        fileType = ".txt";
                        fileTypeBar.Text = "Text Document (.txt)";
                    }
                }
                catch (Exception)
                {
                    ContentDialog errorDialog = new ContentDialog()
                    {
                        Title = resourceLoader.GetString("FileError"),
                        Content = resourceLoader.GetString("FileDesc"),
                        PrimaryButtonText = "Ok",
                        DefaultButton = ContentDialogButton.Primary
                    };
                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void NewSave()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (changed == false)
            {
                txt.TextDocument.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, "");
                Edit.Opacity = 0;
                startedApp = true;
                if (fileType == ".rtf")
                {
                    fileTypeBar.Text = "Rich Text Document (.rtf)";
                }
                if (fileType == ".txt")
                {
                    fileTypeBar.Text = "Text Document (.txt)";
                }
            }
            else
            {
                ContentDialog saveDialog = new ContentDialog
                {
                    Title = resourceLoader.GetString("ExitSav"),
                    PrimaryButtonText = resourceLoader.GetString("SaveExitFile"),
                    CloseButtonText = resourceLoader.GetString("Stop"),
                    SecondaryButtonText = resourceLoader.GetString("Cont"),
                    DefaultButton = ContentDialogButton.Primary
                };
                var result = await saveDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    if (fileType == ".rtf")
                    {
                        FileSavePicker savePicker = new FileSavePicker();
                        savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;


                        savePicker.FileTypeChoices.Add("Rich Text Document", new List<string>() { ".rtf" });

                        savePicker.SuggestedFileName = "New Document";

                        StorageFile file = await savePicker.PickSaveFileAsync();
                        if (file != null)
                        {
                            CachedFileManager.DeferUpdates(file);

                            using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                                await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                            {
                                txt.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                                txt.TextDocument.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, "");
                                Edit.Opacity = 0;
                                startedApp = true;
                                fileType = ".rtf";
                                fileTypeBar.Text = "Rich Text Document (.rtf)";
                            }


                            FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                            if (status != FileUpdateStatus.Complete)
                            {
                                Windows.UI.Popups.MessageDialog errorBox =
                                    new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                                await errorBox.ShowAsync();
                            }
                        }
                    }
                    else
                    {
                        FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
                        picker.SuggestedFileName = "New Document";
                        picker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                        StorageFile file = await picker.PickSaveFileAsync();
                        if (file != null)
                        {
                            CachedFileManager.DeferUpdates(file);
                            string fileContent = textMain;
                            await FileIO.WriteTextAsync(file, fileContent);

                            FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
                            if (updateStatus == FileUpdateStatus.Complete)
                            {
                                txt.TextDocument.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, "");
                                Edit.Opacity = 0;
                                startedApp = true;
                                fileType = ".txt";
                                fileTypeBar.Text = "Text Document (.txt)";
                                txt.TextDocument.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, "");
                                startedApp = true;
                            }
                        }
                    }
                }

                if (result == ContentDialogResult.Secondary)
                {
                    if (fileType == ".rtf")
                    {
                        fileTypeBar.Text = "Rich Text Document (.rtf)";
                    }
                    if (fileType == ".txt")
                    {
                        fileTypeBar.Text = "Text Document (.txt)";
                    }
                    txt.TextDocument.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, "");
                    Edit.Opacity = 0;
                    startedApp = true;
                }
            }
        }

        private async void Save()
        {
            if (fileType == ".rtf")
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;


                savePicker.FileTypeChoices.Add("Rich Text Document", new List<string>() { ".rtf" });

                savePicker.SuggestedFileName = "New Document";

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);

                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                        await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        txt.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);
                        txt.Document.GetText(Windows.UI.Text.TextGetOptions.UseObjectText, out textMain);
                        richTmp.Text = textMain;
                        startedApp = false;
                        changed = false;
                        Edit.Opacity = 0;
                        fileName.Text = file.Name;
                        var Tb = new StartPage();
                        var selectedTabViewItem = (TabViewItem)Tb.Tabs.SelectedItem;
                        if (selectedTabViewItem == null) return;
                        selectedTabViewItem.Header = file.Name;
                    }


                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != FileUpdateStatus.Complete)
                    {
                        Windows.UI.Popups.MessageDialog errorBox =
                            new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                }

                if (file != null)
                {
                    fileName.Text = file.Name;
                }
            }
            else
            {
                FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
                picker.SuggestedFileName = "New Document";
                picker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);
                    string fileContent = textMain;
                    await FileIO.WriteTextAsync(file, fileContent);

                    FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (updateStatus == FileUpdateStatus.Complete)
                    {
                        txt.Document.GetText(Windows.UI.Text.TextGetOptions.UseObjectText, out textMain);
                        richTmp.Text = textMain;
                        startedApp = false;
                        changed = false;
                        Edit.Opacity = 0;
                        fileName.Text = file.Name;
                        var Tb = new StartPage();
                        var selectedTabViewItem = (TabViewItem)Tb.Tabs.SelectedItem;
                        if (selectedTabViewItem == null) return;
                        selectedTabViewItem.Header = file.Name;
                    }
                }
            }
        }

        private async void SaveExit()
        {
            if (fileType == ".rtf")
            {
                FileSavePicker savePicker = new FileSavePicker();
                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;


                savePicker.FileTypeChoices.Add("Rich Text Document", new List<string>() { ".rtf" });

                savePicker.SuggestedFileName = "New Document";

                StorageFile file = await savePicker.PickSaveFileAsync();
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);

                    using (IRandomAccessStream randAccStream =
                        await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        txt.Document.SaveToStream(TextGetOptions.UseObjectText, randAccStream);

                    }


                    FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                    if (status != FileUpdateStatus.Complete)
                    {
                        Windows.UI.Popups.MessageDialog errorBox =
                            new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                        await errorBox.ShowAsync();
                    }
                }
            }
            else
            {
                FileSavePicker picker = new Windows.Storage.Pickers.FileSavePicker();
                picker.SuggestedFileName = "New Document";
                picker.FileTypeChoices.Add("Text Document", new List<string>() { ".txt" });
                StorageFile file = await picker.PickSaveFileAsync();
                if (file != null)
                {
                    CachedFileManager.DeferUpdates(file);
                    string fileContent = textMain;
                    await FileIO.WriteTextAsync(file, fileContent);

                    FileUpdateStatus updateStatus = await CachedFileManager.CompleteUpdatesAsync(file);
                }
            }
            Application.Current.Exit();
        }

        private List<string> fonts = new List<string>()
        {
            "Arial", "Calibri", "Cambria", "Cambria Math", "Comic Sans MS", "Courier New",
            "Ebrima", "Gadugi", "Georgia",
            "Leelawadee UI", "Lucida Console", "Malgun Gothic", "Microsoft Himalaya", "Microsoft JhengHei",
            "Microsoft JhengHei UI", "Microsoft New Tai Lue", "Microsoft PhagsPa",
            "Microsoft Tai Le", "Microsoft YaHei", "Microsoft YaHei UI",
            "Microsoft Yi Baiti", "Mongolian Baiti", "MV Boli", "Myanmar Text",
            "Nirmala UI", "Segoe Print", "Segoe UI", "Segoe UI Variable Display", "Segoe UI Emoji",
            "Segoe UI Historic", "Segoe UI Symbol", "SimSun", "Times New Roman",
            "Trebuchet MS", "Verdana", "Yu Gothic",
            "Yu Gothic UI"
        };

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            var suitableItems = new List<string>();
            var splitText = sender.Text.ToLower().Split(" ");
            foreach (var font in fonts)
            {
                var found = splitText.All((key) =>
                {
                    return font.ToLower().Contains(key);
                });
                if (found)
                {
                    suitableItems.Add(font);
                }
            }
            if (suitableItems.Count == 0)
            {

            }
            sender.ItemsSource = suitableItems;
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            txt.FontFamily = new FontFamily(args.SelectedItem.ToString());
        }

        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private async void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            ContentDialog dial = new Notes.ViewModels.ContentDialog1();
            await dial.ShowAsync();
        }

        private async void compactOverlayButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);
            compactOptions.CustomSize = new Windows.Foundation.Size(320, 350);
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);
            exitbutt.Visibility = Visibility.Visible;
            EditAnimatedIcon.Visibility = Visibility.Collapsed;
            Painter.Visibility = Visibility.Collapsed;
            Undo.Visibility = Visibility.Collapsed;
            Redo.Visibility = Visibility.Collapsed;
            FileAct.Visibility = Visibility.Collapsed;
        }
        private async void standardModeButton_Click(object sender, RoutedEventArgs e)
        {
            bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.Default);
            exitbutt.Visibility = Visibility.Collapsed;
            Thickness Paintmargin = EditAnimatedIcon.Margin;
            Paintmargin.Right = 5;
            Undo.Visibility = Visibility.Visible;
            Redo.Visibility = Visibility.Visible;
            removedSep.Visibility = Visibility.Visible;
            FileAct.Visibility = Visibility.Visible;
            EditAnimatedIcon.Visibility = Visibility.Visible;
        }





        public async Task<string> GetFileText(string filePath)
        {
            var stringContent = "";

            var file = await StorageFile.GetFileFromPathAsync(filePath);

            if (file != null)
            {
                stringContent = await Windows.Storage.FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }



            return stringContent;
        }

        IPropertySet pr = ApplicationData.Current.RoamingSettings.Values;

        private PrintManager printMan;
        private PrintDocument printDoc;
        private IPrintDocumentSource printDocSource;

        #region Register for printing

        string title = "New Document";

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            base.OnNavigatedTo(e);
            var args = e.Parameter as FileActivatedEventArgs;
            var appView = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            fileName.Text = title;
            if (args != null)
            {
                if (args.Kind == Windows.ApplicationModel.Activation.ActivationKind.File)
                {
                    string strFileName = args.Files[0].Name;
                    string strFile = args.Files[0].Path;
                    if (args.Files != null)
                    {
                        changed = false;
                        try
                        {
                            var text = await GetFileText(@strFile);
                            txt.Document.GetText(Windows.UI.Text.TextGetOptions.UseObjectText, out textMain);
                            richTmp.Text = textMain;
                            startedApp = false;
                            changed = false;
                            appView.Title = strFileName;
                            txt.Document.SetText(Windows.UI.Text.TextSetOptions.FormatRtf, text);

                        }
                        catch
                        {
                            ContentDialog fileError = new ContentDialog()
                            {
                                Title = "Oops...",
                                Content = resourceLoader.GetString("FileDesc"),
                                PrimaryButtonText = "OK"
                            };
                            await fileError.ShowAsync();
                        }

                    }
                }
            }
        }

        #endregion

        #region Showing the print dialog

        bool firstRun = true;

        private async void PrintButtonClick(object sender, RoutedEventArgs e)
        {
            IPropertySet pr = ApplicationData.Current.RoamingSettings.Values;
            pr["fileAct"] = bool.FalseString;
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (PrintManager.IsSupported())
            {
                if (firstRun == true)
                {
                    firstRun = false;
                    printMan = PrintManager.GetForCurrentView();
                    printMan.PrintTaskRequested += PrintTaskRequested;

                    printDoc = new PrintDocument();
                    printDocSource = printDoc.DocumentSource;
                    printDoc.Paginate += Paginate;
                    printDoc.GetPreviewPage += GetPreviewPage;
                    printDoc.AddPages += AddPages;
                }

                await PrintManager.ShowPrintUIAsync();
                txt.Foreground = new SolidColorBrush(Colors.Black);


            }
            else
            {
                ContentDialog noPrintingDialog = new ContentDialog()
                {
                    Title = "Printing...",
                    Content = resourceLoader.GetString("PrintError"),
                    PrimaryButtonText = "OK"
                };
                await noPrintingDialog.ShowAsync();
            }
        }

        private void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs args)
        {
            var printTask = args.Request.CreatePrintTask("Print", PrintTaskSourceRequrested);

            printTask.Completed += PrintTaskCompleted;


        }

        private void PrintTaskSourceRequrested(PrintTaskSourceRequestedArgs args)
        {
            args.SetSource(printDocSource);
        }

        #endregion

        #region Print preview

        private void Paginate(object sender, PaginateEventArgs e)
        {
            printDoc.SetPreviewPageCount(1, PreviewPageCountType.Final);
        }

        private void GetPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            printDoc.SetPreviewPage(e.PageNumber, txt);
        }

        #endregion

        #region Add pages to send to the printer

        private void AddPages(object sender, AddPagesEventArgs e)
        {
            printDoc.AddPage(txt);

            printDoc.AddPagesComplete();
        }

        #endregion

        #region Print task completed



        private async void PrintTaskCompleted(PrintTask sender, PrintTaskCompletedEventArgs args)
        {
            // Notify the user when the print operation fails.
            if (args.Completion == PrintTaskCompletion.Failed)
            {
                var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    ContentDialog noPrintingDialog = new ContentDialog()
                    {
                        Title = "Printing...",
                        Content = resourceLoader.GetString("FileDesc"),
                        PrimaryButtonText = "OK"
                    };
                    await noPrintingDialog.ShowAsync();
                });
            }

        }

        #endregion

        private void MenuFlyoutItem_Click_7(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (AudioTg.IsChecked)
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }
            else
            {
                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }

        }

        bool SimpleMode = false;

        private void MenuFlyoutItem_Click_8(object sender, RoutedEventArgs e)
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            if (SimpleMode == true)
            {
                SimpleMode = false;
                ful.Text = resourceLoader.GetString("Window/Text");
                ful.Icon = new SymbolIcon(Symbol.GoToToday);
                Undo.Visibility = Visibility.Visible;
                Redo.Visibility = Visibility.Visible;
                removedSep.Visibility = Visibility.Visible;
                ExtraBt.Opacity = 100;
                ExtraBt.IsHitTestVisible = true;
                sepCl.Opacity = 100;
            }
            else
            {
                SimpleMode = true;
                ful.Text = resourceLoader.GetString("FullScreen/Text");
                ful.Icon = new SymbolIcon(Symbol.AlignCenter);
                Undo.Visibility = Visibility.Collapsed;
                Redo.Visibility = Visibility.Collapsed;
                removedSep.Visibility = Visibility.Collapsed;
                ExtraBt.Opacity = 0;
                ExtraBt.IsHitTestVisible = false;
                sepCl.Opacity = 0;
            }
        }

        private void MenuFlyoutItem_Click_9(object sender, RoutedEventArgs e)
        {
            FindBoxHighlightMatches();

        }

        private async void FindBoxHighlightMatches()
        {
            var resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
            var inputTextBox = new TextBox { AcceptsReturn = false };
            (inputTextBox as FrameworkElement).VerticalAlignment = VerticalAlignment.Bottom;
            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = resourceLoader.GetString("SearchQuest"),
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = resourceLoader.GetString("SearchButton"),
                SecondaryButtonText = resourceLoader.GetString("Stop"),
                DefaultButton = ContentDialogButton.Primary
            };
            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                FindBoxRemoveHighlights();

                Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
                Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

                string textToFind = inputTextBox.Text;
                if (textToFind != null)
                {
                    ITextRange searchRange = txt.Document.GetRange(0, 0);
                    while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                    {
                        searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                        searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                    }
                }
            }

            else
            {

            }


        }

        private void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = txt.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush defaultBackground = txt.Background as SolidColorBrush;
            SolidColorBrush defaultForeground = txt.Foreground as SolidColorBrush;

            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }

        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            txt.Document.GetText(TextGetOptions.UseCrlf, out string currentRawText);

            // reset colors to correct defaults for Focused state
            ITextRange documentRange = txt.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush background = new SolidColorBrush(Colors.Transparent);


            if (background != null)
            {
                documentRange.CharacterFormat.BackgroundColor = background.Color;
                if (App.Current.RequestedTheme == ApplicationTheme.Dark)
                {
                    txt.Foreground = new SolidColorBrush(Colors.White);
                    txt.Document.Selection.CharacterFormat.ForegroundColor = Colors.White;
                }
                else
                {
                    txt.Foreground = new SolidColorBrush(Colors.Black);
                    txt.Document.Selection.CharacterFormat.ForegroundColor = Colors.Black;
                }
            }
        }

        private void txt_TextChanged(object sender, RoutedEventArgs e)
        {
            if (fileType == ".txt")
            {
                Painter.Visibility = Visibility.Collapsed;
                EditAnimatedIcon.Visibility = Visibility.Collapsed;
            }
            else
            {
                Painter.Visibility = Visibility.Visible;
                EditAnimatedIcon.Visibility = Visibility.Visible;
            }
            if (changed == false)
            {
                Undo.IsEnabled = false;
            }
            else
            {
                Undo.IsEnabled = true;
            }
            if (changes == 0)
            {
                Redo.IsEnabled = false;
            }
            string textStart;
            txt.Document.GetText(TextGetOptions.UseObjectText, out textStart);
            txt.Document.GetText(TextGetOptions.UseObjectText, out textMain);
            if (startedApp == false)
            {
                if (textMain == richTmp.Text)
                {
                    changed = false;
                }
                else
                {
                    changed = true;
                }
            }
            else
            {
                if (textStart == "")
                {
                    changed = false;
                }
                else
                {
                    changed = true;
                }
            }

            if (changed == true)
            {
                Edit.Opacity = 0.6;
            }
            else
            {
                Edit.Opacity = 0;
            }
        }

        private void Details_Click(object sender, RoutedEventArgs e)
        {
            Thickness margin = contentFrame.Margin;
            if (Details.IsChecked)
            {
                nameBar.Visibility = Visibility.Visible;
                margin.Bottom = 30;
            }
            else
            {
                nameBar.Visibility = Visibility.Collapsed;
                margin.Bottom = 0;
            }
        }

        private void EditAnimatedIcon_Click(object sender, RoutedEventArgs e)
        {
            if (Painter.Visibility == Visibility.Collapsed)
            {
                Painter.Visibility = Visibility.Visible;
                tools.Visibility = Visibility.Visible;
                popIn.Begin();
            }
            else
            {
                Painter.Visibility = Visibility.Collapsed;
                tools.Visibility = Visibility.Collapsed;
            }
        }

        private async void SaveIconBut_Click(object sender, RoutedEventArgs e)
        {
            if (inkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count > 0)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                savePicker.FileTypeChoices.Add("Image", new System.Collections.Generic.List<string> { ".png" });

                Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
                if (null != file)
                {
                    try
                    {
                        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await inkCanvas.InkPresenter.StrokeContainer.SaveAsync(stream);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void Grid_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            var grid = sender as Grid;
            try
            {
                grid.Width = grid.ActualWidth - e.Delta.Translation.X;
            }
            catch
            {
            }
        }

        int changes = 0;

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            txt.Document.Undo();
            Redo.IsEnabled = true;
            changes ++;
            if (changed == false)
            {
                Undo.IsEnabled = false;
            }
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            changes --;
            txt.Document.Redo();
            if (changes == 0)
            {
                Redo.IsEnabled = false;
            }
            if (changed != false)
            {
                Undo.IsEnabled = true;
            }
        }

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            redoButton.IsEnabled = false;
            redoButton.Opacity = 10;
        }

        private void TextBlock_PointerEntered_1(object sender, PointerRoutedEventArgs e)
        {
            if (changed == true)
            {
                Tip.IsOpen = true;
            }
        }

        private void strk()
        {
            IReadOnlyList<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            UndoStrokes = new Stack<InkStroke>();
            if (strokes.Count > 0)
            {
                strokes[strokes.Count - 1].Selected = true;
                UndoStrokes.Push(strokes[strokes.Count - 1]);

                inkCanvas.InkPresenter.StrokeContainer.DeleteSelected();
            }
        }

        private void undoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                strk();
                redoButton.IsEnabled = true;
            }
            catch
            {
            }
        }

        public Stack<InkStroke> UndoStrokes { get; set; }

        private void redoButton_Click(object sender, RoutedEventArgs e)
        {
            IReadOnlyList<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
            if (UndoStrokes.Count > 0)
            {
                var UndoSt = UndoStrokes.Pop();
                var strokeBuilder = new InkStrokeBuilder();
                strokeBuilder.SetDefaultDrawingAttributes(UndoSt.DrawingAttributes);
                System.Numerics.Matrix3x2 matr = UndoSt.PointTransform;
                IReadOnlyList<InkPoint> inkPoints = UndoSt.GetInkPoints();
                InkStroke stroke = strokeBuilder.CreateStrokeFromInkPoints(inkPoints, matr);
                inkCanvas.InkPresenter.StrokeContainer.AddStroke(stroke);
            }
        }

        private void FontBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var suitableItems = new List<string>();
            var splitText = sender.Text.ToLower().Split(" ");
            foreach (var font in fonts)
            {
                var found = splitText.All((key) =>
                {
                    return font.ToLower().Contains(key);
                });
                if (found)
                {
                    suitableItems.Add(font);
                }
            }
            if (suitableItems.Count == 0)
            {

            }
            sender.ItemsSource = suitableItems;
        }

        private List<double> Size { get; } = new List<double>()
        {
                8,
                9,
                10,
                11,
                12,
                14,
                16,
                18,
                20,
                24,
                28,
                36,
                48,
                72
        };

        private void FontS_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            int s = int.Parse(args.SelectedItem.ToString());
            txt.FontSize = s;
        }

        private void Tip_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Tip.IsOpen = false;
        }


        private void FontS_LostFocus(object sender, RoutedEventArgs e)
        {
            Thickness m = FontS.Margin;
            m.Right = 41;
            FontS.Margin = m;
        }

        private void FontBox_GotFocus(object sender, RoutedEventArgs e)
        {
            Thickness m = FontS.Margin;
            m.Right = -14;
            FontS.Margin = m;
            FontBox.Width = 170;
            var suitableItems = new List<string>();
            var splitText = FontBox.Text.ToLower().Split(" ");
            foreach (var font in fonts)
            {
                var found = splitText.All((key) =>
                {
                    return font.ToLower().Contains(key);
                });
                if (found)
                {
                    suitableItems.Add(font);
                }
            }
            if (suitableItems.Count == 0)
            {

            }
            FontBox.ItemsSource = suitableItems;
        }

        private void FontBox_LostFocus(object sender, RoutedEventArgs e)
        {
            Thickness m = FontS.Margin;
            m.Right = 41;
            FontS.Margin = m;
            FontBox.Width = 115;
        }

        private void DateTime_Click(object sender, RoutedEventArgs e)
        {
            string currentText;
            txt.Document.GetText(TextGetOptions.UseObjectText, out currentText);
            txt.Document.SetText(TextSetOptions.FormatRtf, DateTime.Now.ToString() + "\n" + currentText);
        }

        private void Rectangle_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ExitStoryboard.Begin();
        }

        private void handSk_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            EnterStoryboard.Begin();
        }

        private void sendBut_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IReadOnlyList<InkStroke> strokes = inkCanvas.InkPresenter.StrokeContainer.GetStrokes();
                strokes[strokes.Count - 1].Selected = true;
                inkCanvas.InkPresenter.StrokeContainer.CopySelectedToClipboard();
                strokes[strokes.Count - 1].Selected = false;
            }
            catch
            {
            }
        }
        private async void Weldial()
        {
            ContentDialog dial = new Notes.Views.VersionDialog();
            await dial.ShowAsync();
        }

        private void inkCanvas_PointerPressed(object sender, RoutedEventArgs e)
        {
            redoButton.IsEnabled = false;
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            Weldial();
        }

        private void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            txt.Document.Selection.Paste(0);
        }

        private void tipEnter_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (changed == true)
            {
                colorG.Background = new SolidColorBrush(Color.FromArgb(30, 0, 185, 255));
            }
        }

        private void tipEnter_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            colorG.Background = new SolidColorBrush(Color.FromArgb(0, 0, 185, 255));
        }

        private void Combo3_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FontS.SelectedIndex = 6;

            if ((ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7)))
            {
                FontS.TextSubmitted += FontS_TextSubmitted;
            }
        }

        private void FontS_TextSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
        {
            bool isDouble = double.TryParse(sender.Text, out double newValue);

            // Set the selected item if:
            // - The value successfully parsed to double AND
            // - The value is in the list of sizes OR is a custom value between 8 and 100
            if (isDouble && (Size.Contains(newValue) || (newValue < 72 && newValue > 8)))
            {
                // Update the SelectedItem to the new value. 
                sender.SelectedItem = newValue;
            }
            else
            {
                // If the item is invalid, reject it and revert the text. 
                sender.Text = sender.SelectedValue.ToString();

                var dialog = new ContentDialog
                {
                    Content = "The font size must be a number between 8 and 72.",
                    CloseButtonText = "Close",
                    DefaultButton = ContentDialogButton.Close
                };
                var task = dialog.ShowAsync();
            }

            // Mark the event as handled so the framework doesn’t update the selected item automatically. 
            args.Handled = true;
        }

        private async void Fa_Click(object sender, RoutedEventArgs e)
        {
            ContentDialog tm = new Notes.Views.TermsDialog();
            await tm.ShowAsync();
        }

        private async void FeedBack_Click(object sender, RoutedEventArgs e)
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

        private void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            fileType = ".txt";
            NewSave();
        }

        private void MenuFlyoutItem_Click_6(object sender, RoutedEventArgs e)
        {
            fileType = ".rtf";
            NewSave();
        }
    }
}