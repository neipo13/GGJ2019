using GGJ2019.Constants;
using GGJ2019.Entities;
using GGJ2019.Scenes;
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
using static GGJ2019.Entities.Car;

namespace GGJ2019.Components
{
    public class CarController : Component, ITriggerListener, IUpdatable
    {
        public bool drivingIn, headingHome, homeScene = false;
        Sprite<CarAnimations> sprite;
        float driveInLocation = 70f;
        ColliderTriggerHelper triggerHelper;

        public CarController(bool homeScene = false)
        {
            this.homeScene = homeScene;
        }

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            sprite = entity.getComponent<Sprite<CarAnimations>>();
            if (homeScene)
            {
                sprite.flipX = true;
                driveUp();
            }
            else
            {
                crashIn();
            }
            triggerHelper = new ColliderTriggerHelper(entity);
        }

        public void onTriggerEnter(Collider other, Collider local)
        {
            if(!drivingIn && headingHome)
            {
                leave();
            }
        }

        public void onTriggerExit(Collider other, Collider local)
        {
        }

        public void onTriggerStay(Collider other, Collider local)
        {
        }

        public void crashIn()
        {
            //tween in & spawn player at end
            sprite.play(CarAnimations.Move);
            var carDriveLength = 0.5f;
            var scene = (GameScene)entity.scene;
            scene.carIntro.Play();
            Core.schedule(carDriveLength/4f, (t) => {
                scene.CameraShake();
                //spawn a bunch of debris
                for(int i = 0; i < 20; i++)
                {
                    scene.addEntity(new Debris(scene.collisionLayer, new Vector2(0f, entity.position.Y), scene.tiles, 1f));
                }
            });
            entity
                .tweenPositionTo(new Vector2(driveInLocation, entity.position.Y), carDriveLength)
                .setEaseType(Nez.Tweens.EaseType.SineOut)
                .setCompletionHandler((a) =>
                {
                    drivingIn = false;
                    //spawn player;
                    scene.player = new Player(scene.inputs[0], scene.tiles, scene.collisionLayer, scene.followEntity, new Vector2(350f, -300f));
                    scene.player.position = entity.position - new Vector2(0f, 10f);
                    scene.followEntity.position = scene.player.position;
                    scene.addEntity(scene.player);
                    sprite.play(CarAnimations.Idle);
                })
                .start();

        }

        public void driveUp()
        {
            var scene = (HomeScene)entity.scene;
            scene.carFinale.Play();
            sprite.play(CarAnimations.Move);
            entity.position = new Vector2(500f, 96f);
            entity
                .tweenPositionTo(new Vector2(180f, entity.position.Y),3.5f)
                .setEaseType(Nez.Tweens.EaseType.SineOut)
                .setCompletionHandler((a) =>
                {
                    //fade out to Title
                    Core.schedule(2f, (t) => Core.startSceneTransition(new FadeTransition(() => new TitleScene())));
                    sprite.play(CarAnimations.Idle);

                })
                .start();
        }

        public void leave()
        {
            var scene = (GameScene)entity.scene;
            scene.player.enabled = false;
            sprite.play(CarAnimations.Move);
            scene.carExit.Play();
            entity
               .tweenPositionTo(new Vector2(-driveInLocation, entity.position.Y), 2.5f)
               .setEaseType(Nez.Tweens.EaseType.SineIn)
               .setCompletionHandler((a) =>
               {
                   Core.scene = new HomeScene();
               })
               .start();
        }

        public void update()
        {
            triggerHelper.update();
        }
    }
}
