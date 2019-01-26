
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
    public class Bullet : Entity
    {
        public const float bulletSpeed = 500f;

        public Bullet(Direction dir, bool angled, Vector2 position) : base("bullet")
        {
            this.position = position ; 
            float x = bulletSpeed * (dir == Direction.Left ? -1 : 1);
            Vector2 vel = new Vector2(x, 0);
            
            if (angled)
            {
                vel = new Vector2(x, -Math.Abs(x/2f));
                if(x > 0f)
                {
                    this.position = position + new Vector2(4f, 8f);
                }
                else
                {
                    this.position = position + new Vector2(-4f, 4f);
                }
                
            }
            var mover = addComponent(new BulletController(vel, (r) => this.destroy()));

            var wallCollider = addComponent(new BoxCollider(16, 8));
            wallCollider.physicsLayer = PhysicsLayers.move;
            wallCollider.collidesWithLayers = PhysicsLayers.tiles;


            var enCollider = addComponent(new BoxCollider(16, 8));
            enCollider.isTrigger = true;
            enCollider.name = Strings.Bullet;
            enCollider.physicsLayer = PhysicsLayers.playerBullet;
            enCollider.collidesWithLayers = PhysicsLayers.enemyHit;

            var bs = addComponent(new PrototypeSprite(16, 8));
            bs.renderLayer = (int)Constants.RenderLayers.Object;
            bs.color = new Color(244, 180, 27);
            bs.followParentEntityRotation = false;
            if (angled)
            {
                if(x > 0f)
                {
                    bs.rotation = -(float)Math.PI / 8f;
                }
                else
                {
                    bs.rotation = (float)Math.PI / 8f;
                }
            }
        }
    }
}
