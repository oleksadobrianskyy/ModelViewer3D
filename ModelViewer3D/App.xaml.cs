using System;
using System.Reflection;
using System.Windows;
using ModelViewer3D.Helpers;

namespace ModelViewer3D
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #if !DEBUG 
        static App()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                String resourceName = "ModelViewer3D.Libs." + new AssemblyName(args.Name).Name + ".dll";

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        return null;
                    }

                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
        #endif

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 0)
            {
                ErrorHandler.ShowMessageBox(Resource.FileNameIsNotProvided);
                this.Shutdown();
            }
            else if (e.Args.Length == 1)
            {
                String filePath = e.Args[0];
                MainWindow window = new MainWindow(
                    filePath, 
                    ServiceLocator.MeshDeserializer, 
                    ServiceLocator.SceneFactory, 
                    ServiceLocator.WireframeGenerator,
                    ServiceLocator.VerticesGenerator);
                window.Show();
            }
            else
            {
                ErrorHandler.ShowMessageBox(Resource.CanOpenOnlyOneFile);
                this.Shutdown();
            }
        }
    }
}
