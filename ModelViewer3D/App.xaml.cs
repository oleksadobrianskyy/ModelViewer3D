using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace ModelViewer3D
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static partial void LoadEmbeddedAssemblies();

        static partial void RegisterServiceLocator();

        static App()
        {
            App.LoadEmbeddedAssemblies();
            App.RegisterServiceLocator();
        }
    }
    
    #if !DEBUG 
    public partial class App
    {
        static partial void LoadEmbeddedAssemblies()
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
    }
    #endif

    public partial class App
    {
        static partial void RegisterServiceLocator()
        {
            IUnityContainer container = UnityBootstraper.RegisterTypes();
            IServiceLocator serviceLocator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
        }
    }
}
