using GGJ2019.Scenes;
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
        Shoot,
        Chase,
        Patrol
    }
    public class EnemyController : SimpleStateMachine<EnemyStates>, ITriggerListener//SimpleStateMachine<EnemyStates>
    {
        Health hp;
        Sprite<Animations> sprite;
        TiledMapMover mover;
        TiledMapMover.CollisionState collisionState = new TiledMapMover.CollisionState();
        BoxCollider moveBox;
        AnimationManager animationManager;
        Entity player;
        Camera camera;

        Direction direction = Direction.Left;
        bool isTurret = false;
        float wakeDistance = NezGame.designWidth / 1.5f;
        float chaseDistance = 40f;
        float moveSpeed = 100;
        public Vector2 velocity;
        float gravity = 800f;
        float runAccel = 1000f;
        float friction = 400f;
        Vector2 maxSpeedVec;
        bool isGrounded => collisionState.below;
        bool isMovingHorizontal => velocity.X > 0f || velocity.X < 0f;

        public EnemyController(): base()
        {
            initialState = EnemyStates.Dormant;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            hp = entity.getComponent<Health>();
            animationManager = entity.getComponent<AnimationManager>();
            camera = entity.scene.camera;
            mover = entity.getComponent<TiledMapMover>();
            sprite = entity.getComponent<Sprite<Animations>>();
            var boxes = entity.getComponents<BoxCollider>();
            moveBox = boxes.FirstOrDefault(c => c.name == Constants.Strings.MoveCollider);
            maxSpeedVec = new Vector2(moveSpeed * 2f, moveSpeed * 3f);
            sprite.flipX = true;
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
        

        void Dormant_Enter() { }
        void Dormant_Tick()
        {
            animationManager.Play(Animations.RobotIdle);
            //check distance to camera
            if(Vector2.Distance(entity.position, camera.position) < wakeDistance)
            {
                currentState = EnemyStates.Patrol;
            }
        }
        void Dormant_Exit() { }

        void Patrol_Enter()
        {
            player = ((GameScene)entity.scene).player;
        }
        void Patrol_Tick()
        {
            //walk right or left till you hit a wall and turn
            if (!isTurret)
            {
                int multiplier = direction == Direction.Left ? -1 : 1;
                velocity.X = moveSpeed * multiplier;

                //apply gravity
                velocity.Y += gravity * Time.deltaTime;

                velocity = Vector2.Clamp(velocity, -maxSpeedVec, maxSpeedVec);
                mover.move(velocity * Time.deltaTime, moveBox, collisionState);
                if (collisionState.left)
                {
                    direction = Direction.Right;
                    sprite.flipX = false;
                }
                else if (collisionState.right)
                {
                    direction = Direction.Left;
                    sprite.flipX = true;
                }
                animationManager.Play(Animations.RobotRun);
            }
            //check distance to camera
            if (Vector2.Distance(entity.position, player.position) < chaseDistance)
            {
                if (isTurret)
                {

                }
                else
                {
                    //currentState = EnemyStates.Chase;
                }
            }
        }
        void Patrol_Exit() { }

        void Chase_Enter()
        {
        }
        void Chase_Tick() { }
        void Chase_Exit() { }

        void Shoot_Enter() { }
        void Shoot_Tick() { }
        void Shoot_Exit() { }
    }
}
