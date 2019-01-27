using GGJ2019.Constants;
using GGJ2019.Entities;
using GGJ2019.Scenes;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Nez;
using Nez.Audio;
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
    public class PlayerController : Component, IUpdatable, ITriggerListener
    {
        TiledMapMover mover;
        TiledMapMover.CollisionState collisionState = new TiledMapMover.CollisionState();
        BoxCollider moveBox;
        Sprite<Animations> sprite;
        Sprite weaponSprite;
        AnimationManager animationManager;
        BoxCollider hurtBox;
        InputHandler input;
        Entity followEntity;
        Vector2 followDistance = new Vector2(60f, 0f);
        bool angled = false;
        bool rotatesForJumps = true;
        bool collectedItem, headingHome, inCar = false;
        bool gotHit;
        float hitStunTimer = 0f;
        const float hitStunTime = 0.3f;
        readonly Vector2 hitStunForce = new Vector2(100f, -200f);

        Direction direction = Direction.Right;
        ColliderTriggerHelper triggerHelper;

        SoundEffect jumpSound;
        SoundEffect shootSound;


        float moveSpeed = 175f;
        public Vector2 velocity;
        float gravity = 800f;
        float runAccel = 1000f;
        float friction = 400f;
        Vector2 maxSpeedVec;
        bool isGrounded => collisionState.below;
        bool isMovingHorizontal => velocity.X > 0f || velocity.X < 0f;
        float jumpTilesHigh = 5;
        float jumpHeight => 16 * jumpTilesHigh + 4; //16px tiles * tiles high + buffer
        bool canJumpThisFrame
        {
            get { return collisionState.below || (offGroundInputBufferTimer > 0 && justJumpedBufferTimer <= 0); }
        }
        int offGroundInputBufferFrames = 8;
        int offGroundInputBufferTimer = 0;
        int landingInputBufferFrames = 4;
        int landingInputBufferTimer = 0;
        int justJumpedBufferTimer = 0;

        public PlayerController(InputHandler input, Entity followEntity)
        {
            this.input = input;
            this.followEntity = followEntity;
        }

        public override void onAddedToEntity()
        {
            mover = entity.getComponent<TiledMapMover>();
            animationManager = entity.getComponent<AnimationManager>();
            sprite = animationManager.sprite;
            weaponSprite = entity.getComponents<Sprite>().First((s) => s.name == Strings.Weapon);
            var boxes = entity.getComponents<BoxCollider>();
            moveBox = boxes.FirstOrDefault(c => c.name == Constants.Strings.MoveCollider);
            hurtBox = boxes.FirstOrDefault(c => c.name == Constants.Strings.HitCollider);
            //velocity = Vector2.Zero;
            maxSpeedVec = new Vector2(moveSpeed * 2f, moveSpeed * 3f);
            triggerHelper = new ColliderTriggerHelper(entity);
            animationManager.Play(Animations.PlayerIdle);

            var gs = (GameScene)entity.scene;
            jumpSound = gs.JumpSound;
            shootSound = gs.ShootSound;
        }

        public void OnGameOver()
        {
            entity?.destroy();
        }

        public void update()
        {
            if (Time.timeScale < 0.1f) return;
            if (gotHit)
            {
                updateHit();
            }
            else if (!collectedItem && !headingHome && !inCar)
            {
                updateNormal();
            }
            else if (!headingHome && !inCar)
            {
                velocity.Y += gravity * Time.deltaTime;

                velocity = Vector2.Clamp(velocity, -maxSpeedVec, maxSpeedVec);
                mover.move(velocity * Time.deltaTime, moveBox, collisionState);
            }
            else if (!inCar)
            {
                updateCalm();
            }

        }

        void flipSprites()
        {
            if (velocity.X > 0)
            {
                sprite.flipX = false;
                weaponSprite.flipX = false;
                weaponSprite.localOffset = new Vector2(6f, 2f);
                followEntity.position = entity.position + followDistance;
            }
            else if (velocity.X < 0)
            {
                sprite.flipX = true;
                weaponSprite.flipX = true;
                weaponSprite.localOffset = new Vector2(-6f, 2f);
                followEntity.position = entity.position - followDistance;
            }
            if (input.YInput < 0f)
            {
                angled = true;
                if (sprite.flipX)
                {
                    weaponSprite.rotation = (float)Math.PI / 8f;
                }
                else
                {
                    weaponSprite.rotation = -(float)Math.PI / 8f;
                }
            }
            else
            {
                angled = false;
            }
        }

        public void updateNormal()
        {
            move();
            triggerHelper.update();
            flipSprites();

            if (!isGrounded || !isMovingHorizontal)
            {
                animationManager.Play(Animations.PlayerIdle);
            }
            else if (isGrounded && isMovingHorizontal)
            {
                animationManager.Play(Animations.PlayerRun);
            }

            if (input.ShootButtonInput.isPressed)
            {
                Direction lr = Direction.Right;
                if (sprite.flipX)
                {
                    lr = Direction.Left;
                }
                var gs = (GameScene)entity.scene;
                var bullet = entity.scene.addEntity(new Bullet(lr, angled, entity.position));
                shootSound.Play();
                float shakeDir = lr == Direction.Left ? 1 : -1;
                gs.CameraShake(2f, 0.9f, new Vector2(shakeDir, 0));
            }
        }


        public void move()
        {
            //groundInputBuffer
            if (collisionState.wasGroundedLastFrame && !collisionState.below)
            {
                offGroundInputBufferTimer = offGroundInputBufferFrames;
            }

            //handle left/right
            var highEnoughToTurn = Math.Abs(velocity.X) >= moveSpeed * 3f / 4f;
            var direction = 0;
            if (highEnoughToTurn)
            {
                direction = Math.Sign(velocity.X);
            }
            if (Math.Abs(velocity.X) > moveSpeed && Math.Sign(velocity.X) == input.XInput)
                velocity.X = Mathf.approach(velocity.X, moveSpeed * input.XInput, friction * Time.deltaTime);  //Reduce back from beyond the max speed
            else
                velocity.X = Mathf.approach(velocity.X, moveSpeed * input.XInput, runAccel * Time.deltaTime);
            //drop through one way
            if (collisionState.isGroundedOnOneWayPlatform && input.YInput > 0 && input.JumpButtonInput.isPressed)
            {
                entity.setPosition(new Vector2(entity.position.X, entity.position.Y + 1));
                collisionState.clearLastGroundTile();
                landingInputBufferTimer = 0;
            }
            //jump
            else if (input.JumpButtonInput.isPressed)
            {
                if (canJumpThisFrame) Jump();
                else landingInputBufferTimer = landingInputBufferFrames;
            }
            // jump if you recently hit jump before you landed
            else if (landingInputBufferTimer > 0)
            {
                landingInputBufferTimer--;
                if (collisionState.below) Jump();
            }
            // handle variable jump height
            if (!collisionState.below && input.JumpButtonInput.isReleased)
            {
                velocity.Y *= 0.5f;
            }


            //apply gravity
            velocity.Y += gravity * Time.deltaTime;

            //velocity = Vector2.Clamp(velocity, -maxSpeedVec, maxSpeedVec);

            var oldPos = entity.position;
            mover.move(velocity * Time.deltaTime, moveBox, collisionState);

            //snap to map x
            if (entity.position.X < sprite.width / 2)
            {
                entity.position = new Vector2(sprite.width / 2, entity.position.Y);
            }

            if (collisionState.below)
            {
                sprite.rotation = 0;
                weaponSprite.rotation = 0;
            }
            else if (rotatesForJumps)
            {
                sprite.rotation += (float)Math.PI / 8 * (sprite.flipX ? -1 : 1);
                weaponSprite.rotation = sprite.rotation;
            }

            var leftSide = entity.position.X - moveBox.width / 2;
            var rightSide = entity.position.X + moveBox.width / 2;

            // dont let gravity just build while you're grounded
            if (collisionState.above || collisionState.below)
                velocity.Y = 0;
            // tick jump input buffer timer
            if (offGroundInputBufferTimer > 0)
                offGroundInputBufferTimer--;
            if (justJumpedBufferTimer > 0)
                justJumpedBufferTimer--;
        }

        void Jump()
        {
            velocity.Y = -Mathf.sqrt(2 * jumpHeight * gravity);
            landingInputBufferTimer = 0;
            justJumpedBufferTimer = offGroundInputBufferFrames;
            var scene = (Scenes.GameScene)entity.scene;
            jumpSound.Play();
        }

        private void setDirection()
        {
            var absInputValues = new Vector2(Math.Abs(input.axialInput.X), Math.Abs(input.axialInput.Y));

            if (absInputValues.X > absInputValues.Y)
            {
                if (input.axialInput.X > 0f)
                {
                    direction = Direction.Right;
                }
                else if (input.axialInput.X < 0f)
                {
                    direction = Direction.Left;
                }
            }
            else
            {
                if (input.axialInput.Y > 0f)
                {
                    direction = Direction.Down;
                }
                else if (input.axialInput.Y < 0f)
                {
                    direction = Direction.Up;
                }
            }
        }

        public void turnOff()
        {
            collectedItem = true;
            entity.removeComponent(weaponSprite);
            velocity = new Vector2();
            sprite.rotation = 0f;
            sprite.flipX = false;

        }

        public void calmDown()
        {
            animationManager.Play(Animations.PlayerSigh);
            sprite.onAnimationCompletedEvent += sighOver;
            moveSpeed /= 3f;
            rotatesForJumps = false;
            jumpTilesHigh = 3;

        }

        public void sighOver(Animations anim)
        {
            //add the sigh sprite
            var scene = (GameScene)entity.scene;
            var sigh = entity.scene.addEntity(new Sigh(scene.tiles));
            sigh.position = entity.position;
            sprite.onAnimationCompletedEvent -= sighOver;
            Core.schedule(2f, (t) =>
            {
                headingHome = true;
                var e = new Arrow(scene.carSubtextures);
                e.position = entity.position + new Vector2(30f, -60f);
                entity.scene.addEntity(e);
            });
        }

        void updateCalm()
        {
            move();
            flipSprites();
            if (!isGrounded || !isMovingHorizontal)
            {
                animationManager.Play(Animations.PlayerCalmIdle);
            }
            else if (isGrounded && isMovingHorizontal)
            {
                animationManager.Play(Animations.PlayerCalmRun);
            }
        }

        void updateHit()
        {
            animationManager.Play(Animations.PlayerHit);
            velocity.Y += gravity * Time.deltaTime;
            mover.move(velocity * Time.deltaTime, moveBox, collisionState);
            if (hitStunTimer < 0f)
            {
                gotHit = false;
            }
            hitStunTimer -= Time.deltaTime;
        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            if (local.name == Strings.HitCollider)
            {
                //change state
                gotHit = true;
                //change sprite
                animationManager.Play(Animations.PlayerHit);
                //knockback
                var lr = local.entity.position.X > other.entity.position.X ? 1 : -1;
                var force = hitStunForce;
                force.X *= lr;
                velocity = force;
                hitStunTimer = hitStunTime;

                var gs = (GameScene)entity.scene;
                gs.CameraShake(shakeIntensity: 10f);
            }
        }

        public void onTriggerStay(Collider other, Collider local)
        {

        }

        public void onTriggerExit(Collider other, Collider local)
        {

        }
    }
}
