using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using ModelViewer3D.Core.MeshGeometry3DGenerators;
using ModelViewer3D.Core.Scenes;
using ModelViewer3D.Deserializers;
using ModelViewer3D.Helpers;
using ModelViewer3D.Resources;

namespace ModelViewer3D
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow
    {
        #region Constants

        private const String CommandLineSeparator = " ";

        private const Int32 DefaultDpi = 96;

        private const Int32 HiResDpi = 600;

        private const Int32 AvgDpi = 300;

        #endregion

        private readonly IScene scene;

        private readonly IWireframeGenerator wireframeGenerator;

        private readonly String filePath;

        private GeometryModel3D wireframeModel3D;

        private Boolean isTracking;

        private Point prevPosition;

        public MainWindow()
        {
            this.InitializeComponent();

            IServiceLocator serviceLocator = ServiceLocator.Current;
            IMeshDeserializer deserializer = serviceLocator.GetInstance<IMeshDeserializer>();
            ISceneFactory sceneFactory = serviceLocator.GetInstance<ISceneFactory>();
            this.wireframeGenerator = serviceLocator.GetInstance<IWireframeGenerator>();
            
            String[] args = Environment.GetCommandLineArgs();

            if (args.Length == 1)
            {
                ErrorHandler.ShowMessageBox(Resource.FileNameIsNotProvided);
                this.Close();
                return;
            }

            /*  
            if (args.Length > 2)
            {
                String exePath = args[0];
                String restArgs = String.Join(MainWindow.CommandLineSeparator, args.Skip(2));
                Process.Start(exePath, restArgs);
            }
            */

            try
            {
                this.filePath = args[1];
                this.scene = sceneFactory.Create(deserializer.Deserialize(this.filePath));

                this.Model3DGroup.Children.Add(new GeometryModel3D
                {
                    Geometry = this.scene.MeshGeometry3D,
                    Material = new DiffuseMaterial(Brushes.LightGray),
                    BackMaterial = new DiffuseMaterial(Brushes.LightGray)
                });

                this.ViewPort.Camera = this.scene.CameraManipulator.Camera;

                this.Title = Resource.AppName + " - " + Path.GetFileName(this.filePath);
            }
            catch (Exception exception)
            {
                ErrorHandler.ShowMessageBox(exception.Message);
                this.Close();
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

        private async void WireframeToggleSwitch_IsCheckedChanged(object sender, EventArgs e)
        {
            if (this.WireframeToggleSwitch.IsChecked == true)
            {
                if (this.wireframeModel3D == null)
                {
                    var progressDialog = await this.ShowProgressAsync(Resource.PleaseWait, "Generating wireframe");

                    this.wireframeModel3D = new GeometryModel3D
                    {
                        Geometry = this.wireframeGenerator.Generate(this.scene),
                        Material = new DiffuseMaterial(Brushes.Black)
                    };

                    await progressDialog.CloseAsync();
                }

                this.Model3DGroup.Children.Add(this.wireframeModel3D);
            }
            else if (this.wireframeModel3D != null)
            {
                this.Model3DGroup.Children.Remove(this.wireframeModel3D);
            }
        }

        private async void SaveImageLabel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog
            {
                FileName = Path.GetFileNameWithoutExtension(this.filePath),
                Filter = "png files (*.png)|*.png",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (saveDialog.ShowDialog() != true)
            {
                return;
            }
            
            var progressDialog = await this.ShowProgressAsync(Resource.PleaseWait, "Saving image");

            BitmapSource bmp = ElementToBitmap(this.ViewPort, AvgDpi);

            await Task.Factory.StartNew(() =>
            {
                using (Stream fileStream = saveDialog.OpenFile())
                {
                    PngBitmapEncoder png = new PngBitmapEncoder();
                    png.Frames.Add(BitmapFrame.Create(bmp));
                    png.Save(fileStream);
                }
            });

            await progressDialog.CloseAsync();
        }

        private async void PrintImageLabel_MouseDown(object sender, MouseButtonEventArgs e)
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

            var progressDialog = await this.ShowProgressAsync(Resource.PleaseWait, "Printing image");

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
            printDialog.PrintVisual(img, Path.GetFileName(this.filePath));

            await progressDialog.CloseAsync();
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
