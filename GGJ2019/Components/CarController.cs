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
            entity
                .tweenPositionTo(new Vector2(driveInLocation, entity.position.Y), 0.5f)
                .setEaseType(Nez.Tweens.EaseType.SineOut)
                .setCompletionHandler((a) =>
                {
                    //spawn player;
                    var scene = (GameScene)entity.scene;
                    scene.player = new Player(scene.inputs[0], scene.tiles, scene.collisionLayer, scene.followEntity, new Vector2(300f, 300f));
                    scene.player.position = entity.position + Util.TiledPositionHelper.tiledCenteringVec;
                    scene.followEntity.position = scene.player.position;
                    scene.addEntity(scene.player);
                    drivingIn = false;
                })
                .start();

        }

        public void driveUp()
        {
            entity.position = new Vector2(500f, 96f);
            entity
                .tweenPositionTo(new Vector2(driveInLocation, entity.position.Y), 0.5f)
                .setEaseType(Nez.Tweens.EaseType.SineOut)
                .setCompletionHandler((a) =>
                {
                    //spawn player;
                    var scene = (GameScene)entity.scene;
                    scene.player = new Player(scene.inputs[0], scene.tiles, scene.collisionLayer, scene.followEntity, new Vector2(300f, 300f));
                    scene.player.position = entity.position + Util.TiledPositionHelper.tiledCenteringVec;
                    scene.followEntity.position = scene.player.position;
                    scene.addEntity(scene.player);
                    drivingIn = false;
                })
                .start();
        }

        public void leave()
        {
            var scene = (GameScene)entity.scene;
            scene.player.enabled = false;
            entity
               .tweenPositionTo(new Vector2(-driveInLocation, entity.position.Y), 2.5f)
               .setEaseType(Nez.Tweens.EaseType.SineIn)
               .setCompletionHandler((a) =>
               {
                   Core.scene = new TitleScene();
               })
               .start();
        }

        public void update()
        {
            triggerHelper.update();
        }
    }
}
