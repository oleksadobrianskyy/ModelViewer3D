using Microsoft.Practices.Unity;
using ModelViewer3D.Core.MeshConverters;
using ModelViewer3D.Core.MeshConverters.Impl;
using ModelViewer3D.Core.MeshGeometry3DGenerators;
using ModelViewer3D.Core.MeshGeometry3DGenerators.Impl;
using ModelViewer3D.Core.Scenes;
using ModelViewer3D.Core.Scenes.Impl;
using ModelViewer3D.Deserializers;
using ModelViewer3D.Deserializers.Impl;

namespace ModelViewer3D
{
    internal static class UnityBootstraper
    {
        public static IUnityContainer RegisterTypes()
        {
            IUnityContainer container = new UnityContainer();

            container.RegisterType<IMeshDeserializer, FileExtensionMeshDeserializer>();
            container.RegisterType<IMeshDeserializer, OutMeshDeserializer>("out");
            container.RegisterType<IMeshDeserializer, XmlMeshDeserializer>("xml");

            container.RegisterType<IMeshConverter, MeshConverter>();
            container.RegisterType<ISceneFactory, SceneFactory>();
            container.RegisterType<IWireframeGenerator, WireframeGenerator>();

            return container;
        }
    }
}
