using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Components
{
    public class DebrisController : Component, IUpdatable
    {
        BoxCollider moveBox;
        TiledMapMover mover;
        TiledMapMover.CollisionState collisionState = new TiledMapMover.CollisionState();
        Sprite sprite;

        Vector2 velocity;
        float friction;
        float gravity = 800f;


        public void SetInitialy(Vector2 initialVelocity, float friction = 400f)
        {
            velocity = initialVelocity;
            this.friction = friction;
        }
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            sprite = entity.getComponent<Sprite>();
            moveBox = entity.getComponent<BoxCollider>();
            mover = entity.getComponent<TiledMapMover>();
        }
        public void update()
        {
            if (collisionState.below)
            {
                velocity.X = Mathf.approach(velocity.X, 0f, friction * Time.deltaTime);
            }
            else
            {
                sprite.rotation += (float)Math.PI / 8f;
            }
            velocity.Y += gravity * Time.deltaTime;
            mover.move(velocity * Time.deltaTime, moveBox, collisionState);
        }
    }
}
