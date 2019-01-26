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
    public class Arrow : Entity
    {
        public Arrow(List<Subtexture> subtextures) : base()
        {
            //two sprites
            var arrowSprite = addComponent(new Sprite(subtextures[25]));
            var textSprite = addComponent(new Sprite(subtextures[9]));
            textSprite.localOffset = new Vector2(0, -20f);
            //first flashes infrequent - arrow
            Core.schedule(1f, true, (t) => arrowSprite.enabled = !arrowSprite.enabled);
            //second flashes more frequent - text
            Core.schedule(1f, true, (t) => textSprite.enabled = !textSprite.enabled);
        }
    }
}
