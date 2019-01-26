using GGJ2019.Components;
using GGJ2019.Constants;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using Nez.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GGJ2019.Components.AnimationManager;

namespace GGJ2019.Entities
{
    
    public class Car : Entity
    {
        public enum CarAnimations
        {
            Move,
            Idle
        }
        public Car(List<Subtexture> subtextures, bool isHomeScene = false): base("car")
        {
            var sprite = addComponent(new Sprite<CarAnimations>(subtextures[0]));
            sprite.addAnimation(CarAnimations.Move, new SpriteAnimation(subtextures.GetRange(0, 2)));
            sprite.addAnimation(CarAnimations.Idle, new SpriteAnimation(subtextures[0]));

            var trigger = addComponent(new BoxCollider(56, 24));
            trigger.name = Strings.CarCollider;
            trigger.isTrigger = true;
            trigger.physicsLayer = Constants.PhysicsLayers.car;
            trigger.collidesWithLayers = Constants.PhysicsLayers.collect;

            position = new Vector2(-200f, (16f * 14f) - 12f);
            var controller = new CarController(isHomeScene);
            addComponent(controller);
        }
    }
}
