using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez;
using Microsoft.Xna.Framework;
using GGJ2019.Scenes;

namespace GGJ2019.Components
{
    public class McGuffinController : Component, ITriggerListener, IUpdatable
    {
        bool collected = false;
        bool doneAnimating = false;
        Entity player;

        public void onTriggerEnter(Collider other, Collider local)
        {
            collect(other, local);
        }

        public void onTriggerExit(Collider other, Collider local)
        {

        }

        public void onTriggerStay(Collider other, Collider local)
        {
            collect(other, local);
        }
        private void collect(Collider other, Collider local)
        {
            if (!collected)
            {
                //set all the things to make
                collected = true;
                player = other.entity;
                local.active = false;
                var pc = player.getComponent<PlayerController>();
                pc.turnOff();
                var gs = (GameScene)entity.scene;
                foreach(var en in gs.enemies)
                {
                    if(en.scene != null)
                    {
                        en.destroy();
                    }
                }
                var car = gs.car;
                var cc = car.getComponent<CarController>();
                cc.headingHome = true;
                var finalPos = entity.position - new Vector2(0, 32);
                entity
                    .tweenPositionTo(entity.position - new Vector2(0f, 80f), 0.3f)
                    .setEaseType(Nez.Tweens.EaseType.SineOut)
                    .setNextTween(entity
                        .tweenPositionTo(finalPos, 0.8f)
                        .setEaseType(Nez.Tweens.EaseType.SineIn)
                        .setCompletionHandler((a) =>
                        {
                            doneAnimating = true;
                            entity.rotation = 0f;
                            //do whatever stuff we need to with player
                            Core.schedule(1.2f, (t) => this.entity.enabled = false);
                            pc.calmDown();
                        }))
                    .start();
            }
        }
        public void update()
        {
            if(collected && !doneAnimating)
            {
                entity.rotation += (float)Math.PI / 8;
            }
        }
    }
}
