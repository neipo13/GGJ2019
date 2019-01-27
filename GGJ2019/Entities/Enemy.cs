using GGJ2019.Components;
using GGJ2019.Constants;
using GGJ2019.Effects;
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
        public Enemy(List<Subtexture> subtextures, TiledTileLayer collisionLayer, Effect flashEffect) : base("enemy")
        {
            var sprite = addComponent(new Sprite<Animations>(subtextures[32]));
            sprite.renderLayer = (int)Constants.RenderLayers.Object;
            material = new FlashWhiteMaterial(flashEffect);
            sprite.material = material;

            var animManager = new AnimationManager(sprite, subtextures);
            addComponent(animManager);
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
            material.Flashing = true;
            Core.schedule(0.3f, (t) => material.Flashing = false);
        }

        private void onDeath()
        {
            this.destroy();
        }
    }
}
