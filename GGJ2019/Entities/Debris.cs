using GGJ2019.Components;
using Nez;
using Nez.Tiled;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez.Textures;
using Nez.Sprites;

namespace GGJ2019.Entities
{
    public class Debris : Entity
    {
        float maxSpeedX = 200f;
        float maxYSpeed = 500f;
        float minYSpeed = 300f;
        public Debris(TiledTileLayer collisionLayer, Vector2 position, List<Subtexture> subtextures) : base()
        {
            //get initial vel
            float xSpeed = Nez.Random.range(-maxSpeedX, maxSpeedX);
            float ySpeed = Nez.Random.range(-maxYSpeed, minYSpeed);
            //sprite
            int spriteNum = Nez.Random.range(144, 148);
            var sprite = new Sprite(subtextures[spriteNum]);
            sprite.renderLayer = (int)Constants.RenderLayers.Decorative;
            //sprite.color = new Color(90, 83, 83);
            addComponent(sprite);
            //move collide box
            var box = new BoxCollider(4, 4);
            box.name = Constants.Strings.MoveCollider;
            addComponent(box);
            //tiled mover
            var tiledMover = new TiledMapMover(collisionLayer);
            addComponent(tiledMover);
            //debris controller
            var controller = new DebrisController();
            controller.SetInitialy(new Vector2(xSpeed, ySpeed));
            addComponent(controller);

            this.position = position;
        }
    }
}
