using GGJ2019.Constants;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
using Nez;
using Nez.UI;
using Nez.UI.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Scenes
{
    public class TitleScene : Scene
    {
        InputHandler[] inputs;
        Entity entity;
        UICanvas canvas;

        bool transitioning = false;

        public override void initialize()
        {
            // required renderer stuff
            clearColor = Color.Black;
            var renderer = addRenderer(new DefaultRenderer());

            // setup inputs
            inputs = new InputHandler[NezGame.maxPlayers];
            for(int i = 0; i < NezGame.maxPlayers; i++)
            {
                inputs[i] = new InputHandler(i);
            }
            SetupTitle();
        }

        private void SetupTitle()
        {
            entity = addEntity(new Entity());
            canvas = entity.addComponent(new UICanvas());
            canvas.setRenderLayer((int)RenderLayers.UI);
            var style = new LabelStyle(new Color(223, 246, 245));
            var title = new EffectLabel("/1MERCENARY/1", style)
                .setAlignment(Align.center)
                .setFontScale(3f);
            var pressStart = new EffectLabel("/2Press /6[action]/6 to Start/2", style)
                .setAlignment(Align.center);
            canvas
                .stage
                .addElement(title)
                .setX(NezGame.designWidth / 2)
                .setY(NezGame.designHeight * 1 / 4);
            canvas
                .stage
                .addElement(pressStart)
                .setX(NezGame.designWidth / 2)
                .setY(NezGame.designHeight * 3 / 4);
        }

        public override void update()
        {
            base.update();

            if(!transitioning && inputs.Any((i) => i.AnyButtonPressed))
            {
                transitioning = true;
                Core.scene = new GameScene();
            }
        }
    }
}
