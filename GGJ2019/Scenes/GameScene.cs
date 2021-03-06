﻿using System;
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
using Nez.Sprites;
using Microsoft.Xna.Framework.Audio;
using GGJ2019.Components;

namespace GGJ2019.Scenes
{
    public class GameScene : Scene
    {
        public InputHandler[] inputs;


        public List<Subtexture> tiles;
        public List<Subtexture> carSubtextures;
        public List<Subtexture> characters;
        public List<Subtexture> explosionSubtextures;
        PlatformSnapFollowCamera followCamera;
        public TiledTileLayer collisionLayer;
        public Entity followEntity;
        public Player player;
        public Car car;
        public List<Entity> enemies = new List<Entity>();

        public SoundEffect JumpSound;
        public SoundEffect ShootSound;
        public SoundEffect carIntroSound;
        public SoundEffect carExitSound;
        public SoundEffect explosionSound;
        public SoundEffect hitSound;
        public SoundEffect fanfareSound;
        public SoundEffect goalReachedSound;
        public SoundEffect sighSound;

        CameraShake shaker;

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
            var t3 = content.Load<Texture2D>("img/car");
            carSubtextures = Subtexture.subtexturesFromAtlas(t3, 56, 24);
            var t4 = content.Load<Texture2D>("img/explosion");
            explosionSubtextures = Subtexture.subtexturesFromAtlas(t4, 32, 32);

            //load tiled
            var tiledMap = content.Load<TiledMap>($"tiled/level");
            var tiledEntity = this.createEntity("debugMap");
            var tileMapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledMap, "collision"));
            tileMapComponent.setLayersToRender("collision", "backgroundDecorative");
            tileMapComponent.renderLayer = (int)Constants.RenderLayers.Background;
            tileMapComponent.physicsLayer = PhysicsLayers.tiles;
            collisionLayer = tileMapComponent.collisionLayer;

            followEntity = addEntity(new Entity());
            var noHit = followEntity.addComponent<CircleCollider>();
            noHit.physicsLayer = Constants.PhysicsLayers.noHit;
            var spawnObj = tiledMap.getObjectGroup("playerSpawn").objects.First();
            followEntity.position = spawnObj.position;
            car = addEntity(new Car(carSubtextures, false));

            followCamera = camera.addComponent(new PlatformSnapFollowCamera(followEntity));
            followCamera.mapSize = new Vector2(tiledMap.widthInPixels, tiledMap.heightInPixels);
            followCamera.mapLockEnabled = true;
            camera.position = new Vector2(0f, 500f);

            var mcGufObj = tiledMap.getObjectGroup("mcguffin").objects.First();
            var mcGuffin = new McGuffin(tiles);
            mcGuffin.position = mcGufObj.position + Util.TiledPositionHelper.tiledCenteringVec;
            addEntity(mcGuffin);

            var enemyObjs = tiledMap.getObjectGroup("enemies").objects;
            foreach(var enemyObj in enemyObjs)
            {
                int type = 0;
                string typeStr = enemyObj.properties.ContainsKey("EnemyType") ? enemyObj.properties["EnemyType"] : "0";
                int.TryParse(typeStr, out type);
                var flashingEffect = content.Load<Effect>("effects/flashWhite");
                var enemy = new Enemy(tiles, collisionLayer, flashingEffect, (Components.EnemyType)type);
                enemy.position = enemyObj.position + Util.TiledPositionHelper.tiledCenteringVec;
                addEntity(enemy);
                enemies.Add(enemy);

            }

            var bgTex = content.Load<Texture2D>("img/sky");
            var bg = new Entity();
            var bgSpr = new Sprite(bgTex);
            //bgSpr.localOffset = new Vector2(bgTex.Width / 2, 0);
            bg.addComponent(bgSpr);
            bgSpr.renderLayer = (int)RenderLayers.WayBack;
            var parallax = new ParalaxLayer(camera);
            //parallax.offset = new Vector2(0, 56);
            parallax.paralaxRatio = ((float)NezGame.designWidth / (float)tiledMap.widthInPixels) / ((float)bgTex.Width / (float)tiledMap.widthInPixels);
            bg.addComponent(parallax);
            addEntity(bg);

            JumpSound = content.Load<SoundEffect>("audio/JUMP");
            ShootSound = content.Load<SoundEffect>("audio/SHOOT");
            carIntroSound = content.Load<SoundEffect>("audio/CAR_INTRO");
            carExitSound = content.Load<SoundEffect>("audio/CAR_EXIT");
            explosionSound = content.Load<SoundEffect>("audio/ENEMY_EXPLOSION");
            hitSound = content.Load<SoundEffect>("audio/ENEMY_HIT");
            fanfareSound = content.Load<SoundEffect>("audio/FANFARE2");
            goalReachedSound = content.Load<SoundEffect>("audio/GOAL_REACHED");
            sighSound = content.Load<SoundEffect>("audio/ggjSigh");

            shaker = camera.addComponent(new CameraShake());
        }

        public void CameraShake(float shakeIntensity = 15f, float shakeDegredation = 0.9f, Vector2 shakeDirection = default(Vector2))
        {
            shaker.shake(shakeIntensity, shakeDegredation, shakeDirection);
        }
    }
}
