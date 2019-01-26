using GGJ2019.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;

namespace GGJ2019
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class NezGame : Core
    {

        public const int TileSize = 16;
        public const int TilesWide = 16;
        public const int TilesHigh = 9;

        public static int designWidth => TilesWide * TileSize;
        public static int designHeight => TilesHigh * TileSize;

        /// <summary>
        /// Must be between 1 and 4
        /// </summary>
        public const int maxPlayers = 1;

        public NezGame() : base(256 * 4, 144 * 4, windowTitle: "Global Game Jam 2019")
        {
            Scene.setDefaultDesignResolution(designWidth, designHeight, Scene.SceneResolutionPolicy.BestFit, 0, 0);
            Window.AllowUserResizing = true;
            Window.Position = new Point(0, 0);
            Input.maxSupportedGamePads = maxPlayers;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            scene = Scene.createWithDefaultRenderer();
            base.Update(new GameTime());
            base.Draw(new GameTime());
            scene = new GameScene();
        }
    }
}
