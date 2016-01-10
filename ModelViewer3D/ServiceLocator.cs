using ModelViewer3D.Core.MeshConverters;
using ModelViewer3D.Core.MeshConverters.Impl;
using ModelViewer3D.Core.MeshGeometryGenerators;
using ModelViewer3D.Core.MeshGeometryGenerators.Impl;
using ModelViewer3D.Core.Scenes;
using ModelViewer3D.Core.Scenes.Impl;
using ModelViewer3D.Deserializers;
using ModelViewer3D.Deserializers.Impl;

namespace ModelViewer3D
{
    internal static class ServiceLocator
    {
        private static readonly IMeshDeserializer meshDeserializer;
               
        private static readonly IMeshConverter meshConverter;
             
        private static readonly ISceneFactory sceneFactory;
            
        private static readonly IMeshGeometry3DGenerator wireframeGenerator;

        static ServiceLocator()
        {
            //Register type bindings
            meshDeserializer = new FileExtensionMeshDeserializer();
            meshConverter = new MeshConverter();
            sceneFactory = new SceneFactory(meshConverter);
            wireframeGenerator = new WireframeGenerator();
        }

        public static IMeshDeserializer MeshDeserializer
        {
            get { return meshDeserializer; }
        }

        public static IMeshConverter MeshConverter
        {
            get { return meshConverter; }
        }

        public static ISceneFactory SceneFactory
        {
            get { return sceneFactory; }
        }

        public static IMeshGeometry3DGenerator WireframeGenerator
        {
            get { return wireframeGenerator; }
        }
    }
}
