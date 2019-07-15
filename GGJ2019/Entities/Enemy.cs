using GGJ2019.Components;
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
        public Enemy(List<Subtexture> subtextures, TiledTileLayer collisionLayer, Effect flashEffect, EnemyType type = EnemyType.Patrol) : base("enemy")
        {
            var sprite = addComponent(new Sprite<Animations>(subtextures[32]));
            sprite.renderLayer = (int)Constants.RenderLayers.Object;
            material = new FlashWhiteMaterial(flashEffect);
            sprite.material = material;

            animationManager = new AnimationManager(sprite, subtextures);
            addComponent(animationManager);
            switch (type)
            {
                case EnemyType.Turret:
                    sprite.play(Animations.RobotTurret);
                    break;
                case EnemyType.ArcTurret:
                    sprite.play(Animations.RobotArcTurret);
                    break;
                default:
                    sprite.play(Animations.RobotIdle);
                    break;
            }

            var box = addComponent(new BoxCollider(16, 16));
            box.name = Strings.MoveCollider;
            box.physicsLayer = PhysicsLayers.move;
            box.collidesWithLayers = PhysicsLayers.tiles;

            addComponent(new TiledMapMover(collisionLayer));

            var hurtBox = addComponent(new BoxCollider(16, 14));
            hurtBox.name = Strings.HitCollider;
            hurtBox.physicsLayer = PhysicsLayers.enemyHit;
            hurtBox.collidesWithLayers = PhysicsLayers.playerBullet;
            hurtBox.isTrigger = true;

            var hitBox = addComponent(new BoxCollider(16, 12));
            hitBox.name = Strings.AttackCollider;
            hitBox.physicsLayer = PhysicsLayers.enemyBullet;
            hitBox.collidesWithLayers = PhysicsLayers.playerHit;
            hitBox.isTrigger = true;


            var hp = new Health(3, onHit, onDeath);
            addComponent(hp);
            var controller = new EnemyController(type);
            addComponent(controller);

        }

        private void onHit()
        {
            //material.Flashing = true;
            //Core.schedule(0.3f, (t) => material.Flashing = false);
            animationManager.Play(Animations.RobotHit);

            var gs = (GameScene)scene;
            gs.hitSound.Play(0.75f, 0f, 0f);
        }

        private void onDeath()
        {
            var gs = (GameScene)scene;
            gs.hitSound.Play(0.8f, 0.5f, 0f);
            //debris
            var collisionLayer = gs.collisionLayer;
            int debris = Nez.Random.range(5, 9);
            for(int i = 0; i < debris; i++)
            {
                gs.addEntity(new Debris(collisionLayer, this.position - new Vector2(0f, 10f), gs.tiles));
            }

            //explosions
            int explosions = Nez.Random.range(3,5);
            gs.explosionSound.Play(0.1f, 0f, 0f);
            for (int i = 0; i < explosions; i++)
            {
                float timeOffset = Nez.Random.range(0f, 0.15f);
                float x = Nez.Random.range(-20f, 20f);
                float y = Nez.Random.range(-20f, 20f);
                Vector2 offset = new Vector2(x, y);
                Core.schedule(timeOffset, (t) =>
                {
                    Explosion explosion = new Explosion(gs.explosionSubtextures);
                    explosion.position = position + offset;
                    gs.addEntity(explosion);
                    gs.CameraShake(shakeIntensity: 8f);
                    //gs.explosionSound.Play(0.3f, 0f, 0f);
                });
            }
            this.destroy();
        }


    }
}
