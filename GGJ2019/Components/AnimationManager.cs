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
            PlayerCalmIdle,
            PlayerCalmRun,
            PlayerSigh,
            GreenIdle,
            GreenRun,
            McGuffin
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
            var calmRunSpriteAnim = new SpriteAnimation(subtextures.GetRange(36, 5));
            calmRunSpriteAnim.fps = 8;
            sprite.addAnimation(Animations.PlayerCalmRun, calmRunSpriteAnim);
            sprite.addAnimation(Animations.PlayerIdle, new SpriteAnimation(subtextures.GetRange(32, 4)));
            var calmIdle = new SpriteAnimation(new List<Subtexture>(){
                subtextures[32],
                subtextures[32],
                subtextures[32],
                subtextures[32],
                subtextures[32],
                subtextures[32],
                subtextures[32],
                subtextures[33],
                subtextures[34],
                subtextures[34],
                subtextures[34],
                subtextures[34],
                subtextures[34],
                subtextures[35] });
            calmIdle.fps = 8;
            sprite.addAnimation(Animations.PlayerCalmIdle, calmIdle);
            sprite.addAnimation(Animations.McGuffin, new SpriteAnimation(subtextures.GetRange(19, 6)));

            var sigh = new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[34],
                subtextures[34],
                subtextures[33],
                subtextures[32],
                subtextures[32],
                subtextures[32],
                subtextures[32],
                subtextures[34]
            });
            sigh.setLoop(false);
            sprite.addAnimation(Animations.PlayerSigh, sigh);
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
