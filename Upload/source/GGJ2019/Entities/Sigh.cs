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
    public class Sigh : Entity
    {
        const float lifeSpan = 2f;
        public Sigh(List<Subtexture> subtextures) : base()
        {
            //draw && move && delete self after some time
            var sprite = addComponent(new Sprite(subtextures[41]));
            sprite.renderLayer = (int)Constants.RenderLayers.Foreground;
            
            addComponent(new BulletController(new Vector2(40f, 0), friction: 30f));
            //sprite.tweenColorTo(new Color(255f, 255f, 255f, 0f), 0.3f).start();
            Core.schedule(lifeSpan, (t) => enabled = false);

        }
    }
}
