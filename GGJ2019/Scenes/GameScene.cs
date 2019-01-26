using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GGJ2019.Constants;
using GGJ2019.Entities;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Nez.Tiled;
using GGJ2019.Util;

namespace GGJ2019.Scenes
{
    public class GameScene : Scene
    {
        InputHandler[] inputs;


        public List<Subtexture> tiles;
        public List<Subtexture> characters;
        PlatformSnapFollowCamera followCamera;

        public override void initialize()
        {
            base.initialize();

            // required renderer stuff
            clearColor = new Color(40, 204, 223);
            var renderer = addRenderer(new DefaultRenderer());


            // setup inputs
            inputs = new InputHandler[NezGame.maxPlayers];
            for (int i = 0; i < NezGame.maxPlayers; i++)
            {
                inputs[i] = new InputHandler(i);
            }

            //load textures
            var texture = content.Load<Texture2D>("img/basic-prototyping");
            tiles = Subtexture.subtexturesFromAtlas(texture, 16, 16);
            var t2 = content.Load<Texture2D>("img/characters");
            characters = Subtexture.subtexturesFromAtlas(t2, 32, 32);

            //load tiled
            var tiledMap = content.Load<TiledMap>($"tiled/level");
            var tiledEntity = this.createEntity("debugMap");
            var tileMapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledMap, "collision"));
            tileMapComponent.setLayersToRender("collision");
            tileMapComponent.renderLayer = (int)Constants.RenderLayers.Background;
            tileMapComponent.physicsLayer = PhysicsLayers.tiles;
            var collisionLayer = tileMapComponent.collisionLayer;

            var followEntity = addEntity(new Entity());
            var noHit = followEntity.addComponent<CircleCollider>();
            noHit.physicsLayer = Constants.PhysicsLayers.noHit;
            var player = new Player(inputs[0], tiles, collisionLayer, followEntity);
            var spawnObj = tiledMap.getObjectGroup("playerSpawn").objects.First();
            player.position = spawnObj.position + Util.TiledPositionHelper.tiledCenteringVec;
            followEntity.position = player.position;
            addEntity(player);

            followCamera = camera.addComponent(new PlatformSnapFollowCamera(followEntity));
            followCamera.mapSize = new Vector2(tiledMap.widthInPixels, tiledMap.heightInPixels);
            followCamera.mapLockEnabled = true;

        }
    }
}
