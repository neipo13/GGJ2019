using GGJ2019.Components;
using GGJ2019.Constants;
using GGJ2019.Util.Input;
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
    public class McGuffin : Entity
    {
        public McGuffin(List<Subtexture> subtextures) : base("goal")
        {
            var collider = addComponent(new BoxCollider(16, 16));
            collider.isTrigger = true;
            collider.name = Strings.McGuffinCollider;
            collider.collidesWithLayers = Constants.PhysicsLayers.collect;
            collider.physicsLayer = Constants.PhysicsLayers.mcguff;

            var sprite = addComponent(new Sprite<Animations>());
            var animManager = addComponent(new AnimationManager(sprite, subtextures));
            Core.schedule(2f, (t) => animManager.Play(Animations.McGuffin));

            addComponent(new McGuffinController());
            
        }
    }
}
