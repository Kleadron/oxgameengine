using Microsoft.Xna.Framework.Graphics;
using Ox.Engine;
using Ox.Engine.Component;
using Ox.Engine.Utility;
using Ox.Scene;

namespace Ox.Scripts
{
    public class ExampleGame : ComponentScript<OxComponent>
    {
        public ExampleGame(OxEngine engine, Transfer<OxComponent> component)
            : base(engine, component)
        {
            LoadScene();
            ConfigureScene();
        }

        private void LoadScene()
        {
            Engine.LoadDocument("ExampleScene.xml", SceneConfiguration.SceneDocumentType, DomainName);
        }

        private void ConfigureScene()
        {
            SceneSystem sceneSystem = Engine.GetService<SceneSystem>();
            sceneSystem.Fog.Color = Color.Black;
        }
    }
}
