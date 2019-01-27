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
            var arrowSprite = addComponent(new Sprite(subtextures[2]));
            arrowSprite.scale = new Vector2(2f);
            arrowSprite.enabled = false;
            //first flashes infrequent - arrow
            Core.schedule(2f, (t) => {

                Core.schedule(1.5f, true, (t2) => arrowSprite.enabled = !arrowSprite.enabled);
            });
        }
    }
}
