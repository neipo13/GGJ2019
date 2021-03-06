﻿using GGJ2019.Entities;
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
        Patrol,
        Turret,
        ArcTurret
    }

    public enum EnemyType
    {
        Patrol,
        Turret,
        ArcTurret,
        Flying
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

        EnemyType type;
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

        public EnemyController(EnemyType type = EnemyType.Patrol): base()
        {
            initialState = EnemyStates.Dormant;
            this.type = type;
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
            if(other.entity.name == "bullet")
            {
                hp?.TakeDamage();
                other.entity.destroy();
            }
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
            //check distance to camera
            if(Vector2.Distance(entity.position, camera.position) < wakeDistance)
            {
                switch (type)
                {
                    case EnemyType.Turret:
                        currentState = EnemyStates.Turret;
                        break;
                    case EnemyType.ArcTurret:
                        currentState = EnemyStates.ArcTurret;
                        break;
                    default:
                        currentState = EnemyStates.Patrol;
                        break;
                }
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


        float shootTimer = 0f;
        const float shootTime = 1.5f;

        void Turret_Enter()
        {
            shootTimer = shootTime;
        }
        void Turret_Tick()
        {
            animationManager.Play(Animations.RobotTurret);
            shootTimer -= Time.deltaTime;
            if(shootTimer < 0f)
            {
                //shoot diagonal down
                float xVelMult = direction == Direction.Left ? -1 : 1;
                float bulletVel = 300f;
                Vector2 vel = new Vector2(bulletVel * xVelMult, Math.Abs(bulletVel / 2f));
                EnemyBullet bullet = new EnemyBullet(vel, this.entity.position);
                entity.scene.addEntity(bullet);
                shootTimer = shootTime;
            }
        }
        void Turret_Exit() { }


        void ArcTurret_Enter()
        {
            shootTimer = shootTime;
        }
        void ArcTurret_Tick()
        {
            animationManager.Play(Animations.RobotArcTurret);
            shootTimer -= Time.deltaTime;
            if (shootTimer < 0f)
            {
                //shoot diagonal down
                float xVelMult = direction == Direction.Left ? -1 : 1;
                float bulletVel = 300f;
                Vector2 vel = new Vector2(bulletVel * xVelMult, -Math.Abs(bulletVel / 2f));
                EnemyBullet bullet = new EnemyBullet(vel, this.entity.position, gravity, 12f, 12f);
                entity.scene.addEntity(bullet);
                shootTimer = shootTime;
            }
        }
        void ArcTurret_Exit() { }
    }
}
