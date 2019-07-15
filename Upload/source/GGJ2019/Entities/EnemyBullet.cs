using GGJ2019.Components;
using GGJ2019.Constants;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
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
    public class EnemyBullet : Entity
    {
        public EnemyBullet(Vector2 velocity, Vector2 position, float gravity = 0f, float sizeX = 16f, float sizeY = 8f) : base()
        {

            this.position = position;
            var mover = addComponent(new BulletController(velocity, gravity: gravity, onCollisonWithAnything:(r) => {
                if(this.scene != null) this.destroy();
            }));


            var wallCollider = addComponent(new BoxCollider(sizeX, sizeY));
            wallCollider.physicsLayer = PhysicsLayers.move;
            wallCollider.collidesWithLayers = PhysicsLayers.tiles;


            var enCollider = addComponent(new BoxCollider(sizeX, sizeY));
            enCollider.isTrigger = true;
            enCollider.name = Strings.Bullet;
            enCollider.physicsLayer = PhysicsLayers.enemyBullet;
            enCollider.collidesWithLayers = PhysicsLayers.playerHit;

            var bs = addComponent(new PrototypeSprite(sizeX, sizeY));
            bs.renderLayer = (int)Constants.RenderLayers.Object;
            bs.color = new Color(230, 72, 46);
            //bs.followParentEntityRotation = false;
            if (velocity.Y > 0f || velocity.Y < 0f)
            {
                if (velocity.X > 0f)
                {
                    this.rotation = (float)Math.PI / 8f;
                }
                else
                {
                    this.rotation = -(float)Math.PI / 8f;
                }
            }
        }
    }
}
