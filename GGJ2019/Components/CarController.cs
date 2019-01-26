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
    public class CarController : Component, ITriggerListener
    {
        bool drivingIn, headingHome = false;
        Sprite<CarAnimations> sprite;
        float driveInLocation = 70f;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
            sprite = entity.getComponent<Sprite<CarAnimations>>();
            crashIn();
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
                    var player = new Player(scene.inputs[0], scene.tiles, scene.collisionLayer, scene.followEntity, new Vector2(300f, 300f));
                    player.position = entity.position + Util.TiledPositionHelper.tiledCenteringVec;
                    scene.followEntity.position = player.position;
                    scene.addEntity(player);
                })
                .start();

        }
        public void leave()
        {

        }

        public void update()
        {
        }

    }
}
