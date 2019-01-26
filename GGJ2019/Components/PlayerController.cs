using GGJ2019.Constants;
using GGJ2019.Entities;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
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
    public class PlayerController : Component, IUpdatable
    {
        TiledMapMover mover;
        TiledMapMover.CollisionState collisionState = new TiledMapMover.CollisionState();
        BoxCollider moveBox;
        Sprite<Animations> sprite;
        Sprite weaponSprite;
        AnimationManager animationManager;
        BoxCollider hitBox;
        InputHandler input;
        Entity followEntity;
        Vector2 followDistance = new Vector2(60f, 0f);
        bool angled = false;

        Direction direction = Direction.Right;
        ColliderTriggerHelper triggerHelper;

        AudioSource swingAudioSource;


        float moveSpeed = 175f;
        Vector2 velocity;
        float gravity = 800f;
        float runAccel = 1000f;
        float friction = 400f;
        Vector2 maxSpeedVec;
        bool isGrounded => collisionState.below;
        bool isMovingHorizontal => velocity.X > 0f || velocity.X < 0f;
        float jumpHeight = 16 * 5 + 4; //16px tiles * tiles high + buffer
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
            hitBox = boxes.FirstOrDefault(c => c.name == Constants.Strings.HitCollider);
            velocity = Vector2.Zero;
            maxSpeedVec = new Vector2(moveSpeed * 2f, moveSpeed * 3f);
            triggerHelper = new ColliderTriggerHelper(entity);
            animationManager.Play(Animations.PlayerIdle);
        }

        public void OnGameOver()
        {
            entity?.destroy();
        }

        public void update()
        {
            if (Time.timeScale < 0.1f) return;
            move();
            triggerHelper.update();
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
            if(input.YInput < 0f)
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
                var bullet = entity.scene.addEntity(new Bullet(lr, angled, entity.position));
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

            velocity = Vector2.Clamp(velocity, -maxSpeedVec, maxSpeedVec);

            var oldPos = entity.position;
            mover.move(velocity * Time.deltaTime, moveBox, collisionState);
            
            //snap to map x
            if(entity.position.X < sprite.width / 2)
            {
                entity.position = new Vector2(sprite.width / 2, entity.position.Y);
            }

            if (collisionState.below)
            {
                sprite.rotation = 0;
                weaponSprite.rotation = 0;
            }
            else
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
            //scene.PlaySoundEffect(Constants.Strings.JUMP_SOUND);
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
    }
}
