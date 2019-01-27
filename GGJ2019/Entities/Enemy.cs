﻿using GGJ2019.Components;
using GGJ2019.Constants;
using GGJ2019.Effects;
using GGJ2019.Scenes;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GGJ2019.Components.AnimationManager;

namespace GGJ2019.Entities
{
    public class Enemy : Entity
    {
        FlashWhiteMaterial material;
        AnimationManager animationManager;
        public Enemy(List<Subtexture> subtextures, TiledTileLayer collisionLayer, Effect flashEffect) : base("enemy")
        {
            var sprite = addComponent(new Sprite<Animations>(subtextures[32]));
            sprite.renderLayer = (int)Constants.RenderLayers.Object;
            material = new FlashWhiteMaterial(flashEffect);
            sprite.material = material;

            animationManager = new AnimationManager(sprite, subtextures);
            addComponent(animationManager);
            sprite.play(Animations.RobotIdle);

            var box = addComponent(new BoxCollider(16, 16));
            box.name = Strings.MoveCollider;
            box.physicsLayer = PhysicsLayers.move;
            box.collidesWithLayers = PhysicsLayers.tiles;

            addComponent(new TiledMapMover(collisionLayer));

            var hitBox = addComponent(new BoxCollider(16, 14));
            hitBox.name = Strings.HitCollider;
            hitBox.physicsLayer = PhysicsLayers.enemyHit;
            hitBox.collidesWithLayers = PhysicsLayers.playerBullet;
            hitBox.isTrigger = true;


            var hp = new Health(3, onHit, onDeath);
            addComponent(hp);
            var controller = new EnemyController();
            addComponent(controller);

        }

        private void onHit()
        {
            //material.Flashing = true;
            //Core.schedule(0.3f, (t) => material.Flashing = false);
            animationManager.Play(Animations.RobotHit);
        }

        private void onDeath()
        {
            int explosions = Nez.Random.range(2, 5);
            var gs = (GameScene)scene;
            for(int i = 0; i < explosions; i++)
            {
                float timeOffset = Nez.Random.range(0f, 0.3f);
                float x = Nez.Random.range(-20f, 20f);
                float y = Nez.Random.range(-20f, 20f);
                Vector2 offset = new Vector2(x, y);
                Core.schedule(timeOffset, (t) =>
                {
                    Explosion explosion = new Explosion(gs.explosionSubtextures);
                    explosion.position = position + offset;
                    gs.addEntity(explosion);
                });
            }
            this.destroy();
        }
    }
}
