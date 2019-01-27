using Microsoft.Xna.Framework;
using Nez;
using Nez.AI.FSM;
using Nez.Sprites;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GGJ2019.Components.AnimationManager;

namespace GGJ2019.Components
{
    public enum EnemyStates
    {
        Dormant,
        Idle,
        Walk,
        Shoot,
        Chase,
        Patrol
    }
    public class EnemyController : Component, ITriggerListener//SimpleStateMachine<EnemyStates>
    {
        Health hp;
        Sprite<Animations> sprite;
        TiledMapMover mover;
        TiledMapMover.CollisionState collisionState = new TiledMapMover.CollisionState();
        BoxCollider moveBox;
        AnimationManager animationManager;

        float moveSpeed = 175f;
        public Vector2 velocity;
        float gravity = 800f;
        float runAccel = 1000f;
        float friction = 400f;
        Vector2 maxSpeedVec;
        bool isGrounded => collisionState.below;
        bool isMovingHorizontal => velocity.X > 0f || velocity.X < 0f;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            hp = entity.getComponent<Health>();
        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            hp?.TakeDamage();
            other.entity.destroy();
        }

        public void onTriggerExit(Collider other, Collider local)
        {
        }

        public void onTriggerStay(Collider other, Collider local)
        {
        }
    }
}
