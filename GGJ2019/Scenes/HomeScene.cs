using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GGJ2019.Constants;
using GGJ2019.Entities;
using GGJ2019.Util.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Nez.Textures;
using Nez.Tiled;
using GGJ2019.Util;
using Nez.Sprites;

namespace GGJ2019.Scenes
{
    public class HomeScene : Scene
    {
        public override void initialize()
        {
            base.initialize();

            // required renderer stuff
            clearColor = new Color(40, 204, 223);
            var renderer = addRenderer(new DefaultRenderer());

            var backgroundTexture = content.Load<Texture2D>("img/house");
            var bgEntity = addEntity(new Entity());
            bgEntity.addComponent(new Sprite(backgroundTexture));
            bgEntity.position = new Vector2(NezGame.designWidth / 2f, NezGame.designHeight / 2f);

            var carTexture = content.Load<Texture2D>("img/car");
            var carSubtextures = Subtexture.subtexturesFromAtlas(carTexture, 56, 24);
            var car = new Car(carSubtextures, true);
            addEntity(car);

            var tileTexture = content.Load<Texture2D>("img/basic-prototyping");
            var tiles = Subtexture.subtexturesFromAtlas(tileTexture, 16, 16);

            CreateKid(tiles);
        }

        private void CreateKid(List<Subtexture> tiles)
        {
            var e = new Entity();
            e.position = new Vector2(69, 69);
            var sprite = new Sprite(tiles[42]);
            var ball = new Sprite(tiles[44]);
            e.addComponent(sprite);
            e.addComponent(ball);
            addEntity(e);
            ball
                .tween("localOffset", new Vector2(0f, -30f), 0.5f)
                .setEaseType(Nez.Tweens.EaseType.SineOut)
                .setLoops(Nez.Tweens.LoopType.PingPong, 99)
                .start();

            ball.tween("rotation", 100f, 10f).setEaseType(Nez.Tweens.EaseType.Linear).start();

        }
        
    }
}
