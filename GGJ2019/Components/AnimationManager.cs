using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Components
{
    public class AnimationManager : Component
    {
        public enum Animations
        {
            PlayerIdle,
            PlayerRun,
            GreenIdle,
            GreenRun
        }

        public Sprite<Animations> sprite;
        List<Subtexture> subtextures;

        public AnimationManager(Sprite<Animations> sprite, List<Subtexture> subtextures)
        {
            this.sprite = sprite;
            this.subtextures = subtextures;
        }
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            var runSpriteAnim = new SpriteAnimation(subtextures.GetRange(36, 5));
            runSpriteAnim.fps = 16;
            var runAnim = sprite.addAnimation(Animations.PlayerRun, runSpriteAnim);
            var idleAnim = sprite.addAnimation(Animations.PlayerIdle, new SpriteAnimation(subtextures.GetRange(32, 4)));
            
        }

        public override void onRemovedFromEntity()
        {
            base.onRemovedFromEntity();
            sprite = null;
            subtextures = null;
        }

        public void Play(Animations animation)
        {
            var currentAnimation = sprite.currentAnimation;
            if (currentAnimation != animation)
                sprite.play(animation);
        }
    }
}
