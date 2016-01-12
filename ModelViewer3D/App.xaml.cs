using System;
using System.Linq;
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
            var executingAssembly = Assembly.GetExecutingAssembly();

            var assemblyResources = executingAssembly
                .GetManifestResourceNames()
                .Where(resource => resource.EndsWith(".dll"));

            foreach (var resource in assemblyResources)
            {
                using (var stream = executingAssembly.GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    Assembly.Load(assemblyData);
                }
            }

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                String assemblyName = new AssemblyName(args.Name).Name;
                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

                return loadedAssemblies.FirstOrDefault(asm => asm.GetName().Name == assemblyName);
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
                    ServiceLocator.WireframeGenerator);
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
