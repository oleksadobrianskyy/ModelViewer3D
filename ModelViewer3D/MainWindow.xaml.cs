using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using ModelViewer3D.Core.MeshGeometryGenerators;
using ModelViewer3D.Core.Scenes;
using ModelViewer3D.Deserializers;
using ModelViewer3D.Helpers;

namespace ModelViewer3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        #region Constants

        private const Int32 DefaultDpi = 96;

        private const Int32 HiResDpi = 600;

        private const Int32 AvgDpi = 300;

        #endregion

        private readonly IScene scene;

        private readonly IMeshGeometry3DGenerator wireframeGenerator;

        private readonly TaskScheduler guiTaskScheduler;

        private readonly String fileName;

        private GeometryModel3D wireframeModel3D;

        private Boolean isTracking;

        private Point prevPosition;

        public MainWindow(
            String filePath,
            IMeshDeserializer deserializer,
            ISceneFactory sceneFactory,
            IMeshGeometry3DGenerator wireframeGenerator)
        {
            this.InitializeComponent();

            this.wireframeGenerator = wireframeGenerator;
            this.guiTaskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            this.fileName = Path.GetFileName(filePath);

            try
            {
                this.scene = sceneFactory.Create(deserializer.Deserialize(filePath));

                this.Model3DGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = this.scene.Geometry,
                    Material = new DiffuseMaterial(Brushes.LightGray),
                    BackMaterial = new DiffuseMaterial(Brushes.LightGray)
                });

                this.ViewPort.Camera = this.scene.CameraManipulator.Camera;

                this.Title = Resource.AppName + " - " + this.fileName;
            }
            catch (Exception exception)
            {
                ErrorHandler.ShowMessageBox(exception.Message);
                this.Close();
            }
        }

        private void BlockUI()
        {
            this.MenuFlyout.IsEnabled = false;
            this.MenuButton.Visibility = Visibility.Hidden;
            this.ViewPortPresenter.Visibility = Visibility.Hidden;
            this.ProgressRing.Visibility = Visibility.Visible;
        }

        private void UnblockUI()
        {
            this.ProgressRing.Visibility = Visibility.Hidden;
            this.ViewPortPresenter.Visibility = Visibility.Visible;
            this.MenuButton.Visibility = Visibility.Visible;
            this.MenuFlyout.IsEnabled = true;
        }

        private void ExecuteSync(Action action, Action complete)
        {
            this.ExecuteSync(null, action, complete);
        }

        private void ExecuteSync(Action before, Action action, Action complete)
        {
            try
            {
                if (before != null)
                {
                    // Executes before action.
                    before();
                }
            }
            finally
            {
                // Starts new task on thread pool.
                Task task = Task.Factory.StartNew(action);

                if (complete != null)
                {
                    // When task is completed execute complete on gui thread.
                    task.ContinueWith(t => complete(), this.guiTaskScheduler);
                }
            }
        }

        private void ViewPort_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.scene.ZoomManipulator.ZoomIn(e.Delta / Mouse.MouseWheelDeltaForOneLine);
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (!this.isTracking)
            {
                return;
            }

            Point currPosition = e.GetPosition(null);
            this.scene.RotationManipulator.Rotate(
                Constants.ScaleX * (this.prevPosition.X - currPosition.X),
                Constants.ScaleY * (this.prevPosition.Y - currPosition.Y));
            this.prevPosition = currPosition;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.isTracking = true;
            this.prevPosition = e.GetPosition(null);
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isTracking = false;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.isTracking = false;
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            this.MenuFlyout.IsOpen = true;
        }

        private void MenuFlyout_IsOpenChanged(object sender, RoutedEventArgs e)
        {
            this.MenuButton.Visibility = this.MenuFlyout.IsOpen ? Visibility.Hidden : Visibility.Visible;
        }

        private void WireframeToggleSwitch_IsCheckedChanged(object sender, EventArgs e)
        {
            this.BlockUI();

            if (this.WireframeToggleSwitch.IsChecked == true)
            {
                if (this.wireframeModel3D == null)
                {
                    this.wireframeModel3D = new GeometryModel3D
                    {
                        Geometry = this.wireframeGenerator.Generate(this.scene),
                        Material = new DiffuseMaterial(Brushes.Black)
                    };
                }

                this.Model3DGroup.Children.Add(this.wireframeModel3D);
            }
            else if (this.wireframeModel3D != null)
            {
                this.Model3DGroup.Children.Remove(this.wireframeModel3D);
            }

            this.UnblockUI();
        }

        private void SaveImageLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                FileName = Path.GetFileNameWithoutExtension(this.fileName),
                Filter = "png files (*.png)|*.png",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }

            // Must be executed on gui thread.
            this.BlockUI();
            BitmapSource bmp = ElementToBitmap(this.ViewPort, AvgDpi);

            this.ExecuteSync(
                action: () =>
                {
                    using (Stream fileStream = saveDialog.OpenFile())
                    {
                        PngBitmapEncoder png = new PngBitmapEncoder();
                        png.Frames.Add(BitmapFrame.Create(bmp));
                        png.Save(fileStream);
                    }
                },
                complete: this.UnblockUI);
        }

        private void PrintImageLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() != true)
            {
                return;
            }
            
            this.BlockUI();

            BitmapSource bitmap = ElementToBitmap(this.ViewPort);

            // Create Image element for hosting bitmap.
            Image img = new Image
            {
                Source = bitmap,
                Stretch = Stretch.None
            };

            // Get scale of the print to screen of WPF visual.
            double scale = Math.Min(
                printDialog.PrintableAreaWidth / bitmap.Width,
                printDialog.PrintableAreaHeight / bitmap.Height);

            //Transform the imgae to scale
            img.LayoutTransform = new ScaleTransform(scale, scale);

            // Get the size of the printer page.
            Size size = new Size(
                bitmap.Width * scale,
                bitmap.Height * scale);

            //update the layout of the visual to the printer page size.
            img.Measure(size);
            img.Arrange(new Rect(new Point(0, 0), size));

            // Print the Image element.
            printDialog.PrintVisual(img, this.fileName);

            this.UnblockUI();
        }

        private static BitmapSource ElementToBitmap(FrameworkElement element, Double dpi = HiResDpi)
        {
            // Scale dimensions from 96 dpi to provided dpi.
            double scale = dpi / DefaultDpi;

            RenderTargetBitmap bitmap = new RenderTargetBitmap(
                (Int32)(scale * element.ActualWidth),
                (Int32)(scale * element.ActualHeight),
                scale * DefaultDpi,
                scale * DefaultDpi,
                PixelFormats.Default);

            bitmap.Render(element);
            bitmap.Freeze();

            return bitmap;
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != ModifierKeys.Control)
            {
                return;
            }

            if (e.Key == Key.Add || e.Key == Key.OemPlus)
            {
                this.scene.ZoomManipulator.ZoomIn(1);
            }
            else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
            {
                this.scene.ZoomManipulator.ZoomIn(-1);
            }
        }
    }
}
