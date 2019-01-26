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
    public class Player : Entity
    {
        public Entity followEntity;
        public Player(InputHandler input, List<Subtexture> subtextures, TiledTileLayer collisionLayer, Entity followEntity, Vector2 velocity = new Vector2()): base("player")
        {
            this.followEntity = followEntity;
            var sprite = addComponent(new Sprite<Animations>(subtextures[32]));
            sprite.renderLayer = (int)Constants.RenderLayers.Object;
            var weaponSprite = addComponent(new Sprite(subtextures[82]));
            weaponSprite.renderLayer = (int)Constants.RenderLayers.Foreground;
            weaponSprite.name = Strings.Weapon;
            
            var animManager = addComponent(new AnimationManager(sprite, subtextures));
            var box = addComponent(new BoxCollider(16, 16));
            box.name = Strings.MoveCollider;
            box.physicsLayer = PhysicsLayers.move;
            box.collidesWithLayers = PhysicsLayers.tiles;
            addComponent(new TiledMapMover(collisionLayer));
            var playerController = addComponent(new PlayerController(input, followEntity));
            playerController.velocity = velocity;

            var collectionBox = addComponent(new BoxCollider(12, 12));
            collectionBox.isTrigger = true;
            collectionBox.physicsLayer = PhysicsLayers.collect;
            collectionBox.collidesWithLayers = PhysicsLayers.mcguff;
            
        }
    }
}
