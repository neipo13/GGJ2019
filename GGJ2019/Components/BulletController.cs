using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Components
{
    public class BulletController : Component, IUpdatable
    {
        Mover mover;
        public Vector2 velocity;
        float friction = 0f;
        float gravity = 0f;
        float rotation = 0f;
        bool hasFriction => friction >= 1f;
        bool hasGravity => gravity >= 1f;
        bool hasRotation => rotation >= 0.1f;
        CollisionResult result;

        float maxDistance = NezGame.designWidth * 0.6f; //bullets die after moving n% of screenwidth on x
        float originalX = 0;

        Action<CollisionResult> onCollisionAny;
        public BulletController(Vector2 velocity, Action<CollisionResult> onCollisonWithAnything = null, float friction = 0f, float gravity = 0f)
        {
            this.velocity = velocity;
            this.onCollisionAny = onCollisonWithAnything;
            this.friction = friction;
            this.gravity = gravity;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            mover = entity.getComponent<Mover>();
            if (mover == null)
            {
                mover = entity.addComponent(new Mover());
            }
            originalX = entity.position.X;
        }
        public void update()
        {
            if (hasFriction)
            {
                var sign = Math.Sign(velocity.X);
                velocity.X = Mathf.approach(velocity.X, 0, friction * Time.deltaTime);
                velocity.Y = Mathf.approach(velocity.Y, 0, friction / 4 * Time.deltaTime);
                var newSign = Math.Sign(velocity.X);
                if (Mathf.approximately(velocity.X, 0f) || sign != newSign)
                {
                    this.entity.destroy();
                }
            }
            if (hasGravity)
            {
                velocity.Y += gravity * Time.deltaTime;
            }
            mover.move(velocity * Time.deltaTime, out result);
            if (result.collider != null)
            {
                onCollisionAny?.Invoke(result);
            }
            if(Math.Abs(entity.position.X - originalX) > maxDistance)
            {
                if(this.entity.scene != null)
                {
                    this.entity.destroy();
                }
            }
        }
    }
}
