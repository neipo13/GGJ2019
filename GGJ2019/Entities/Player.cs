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
    public class Player : Entity
    {
        public Entity followEntity;
        public Player(InputHandler input, List<Subtexture> subtextures, TiledTileLayer collisionLayer): base("player")
        {

            var sprite = addComponent(new Sprite<Animations>(subtextures[32]));
            sprite.renderLayer = (int)Constants.RenderLayers.Object;
            var animManager = addComponent(new AnimationManager(sprite, subtextures));
            var box = addComponent(new BoxCollider(16, 16));
            box.name = Strings.MoveCollider;
            box.physicsLayer = PhysicsLayers.move;
            box.collidesWithLayers = PhysicsLayers.tiles;
            addComponent(new TiledMapMover(collisionLayer));
            var playerController = addComponent(new PlayerController(input));
        }
    }
}
