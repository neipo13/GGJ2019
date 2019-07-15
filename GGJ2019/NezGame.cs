using GGJ2019.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
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

        public static SoundEffectInstance musicInstance;
        public static SoundEffect gameMusic;
        public static SoundEffect postGameMusic;
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
            gameMusic = Content.Load<SoundEffect>("audio/gameplaymusic");
            NezGame.musicInstance = gameMusic.CreateInstance();
            NezGame.musicInstance.IsLooped = true;
            musicInstance.Volume = musicInstance.Volume / 2f;

            postGameMusic = Content.Load<SoundEffect>("audio/Music_PostGame");
            scene = Scene.createWithDefaultRenderer();
            base.Update(new GameTime());
            base.Draw(new GameTime());
            scene = new TitleScene();
            //scene = new HomeScene();
            //scene = new GameScene();
        }

        public static void TurnOnMusic(SoundEffect soundEffect)
        {
            NezGame.musicInstance = soundEffect.CreateInstance();
            NezGame.musicInstance.IsLooped = true;
            musicInstance.Volume = musicInstance.Volume / 2f;
            NezGame.musicInstance.Play();
        }

        public static void TurnOffMusic()
        {
            NezGame.musicInstance.Stop();
        }
    }
}
