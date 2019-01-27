using Microsoft.Xna.Framework.Audio;
using Nez;
using Nez.Sprites;
using Nez.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Entities
{
    public enum ExplosionAnim
    {
        Explode
    }
    public class Explosion : Entity
    {
        Sprite<ExplosionAnim> sprite; 
        public Explosion(List<Subtexture> subtextures) : base()
        {
            sprite = new Sprite<ExplosionAnim>(subtextures[0]);
            sprite.addAnimation(ExplosionAnim.Explode, new SpriteAnimation(subtextures).setLoop(false));
            sprite.renderLayer = (int)Constants.RenderLayers.Foreground;
            sprite.play(ExplosionAnim.Explode);
            addComponent(sprite);

            sprite.onAnimationCompletedEvent += done;

        }

        private void done(ExplosionAnim anim)
        {
            sprite.onAnimationCompletedEvent -= done;
            if(this.scene != null)
            {
                this.destroy();
            }
        }
    }
}
