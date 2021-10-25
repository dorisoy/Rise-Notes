using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace Notes.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class SketcherFull : Page
    {
        public SketcherFull()
        {
            this.InitializeComponent();
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

        private void ClearAll_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.InkPresenter.StrokeContainer.Clear();
            redoButton.IsHitTestVisible = false;
            redoButton.Opacity = 10;
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
                redoButton.IsHitTestVisible = true;
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
        private void Rectangle_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ExitStoryboard.Begin();
        }

        private void handSk_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            EnterStoryboard.Begin();
        }
    }
}
