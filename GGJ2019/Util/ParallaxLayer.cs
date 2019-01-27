using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Util
{
    public class ParalaxLayer : Component, IUpdatable
    {
        public Nez.Camera camera;
        public float paralaxRatio = 0.2f;
        public Vector2 offset = new Vector2();

        private ParalaxLayer()
        {
            setUpdateOrder(int.MaxValue);
        }
        public ParalaxLayer(Nez.Camera camera) : this()
        {
            this.camera = camera;
        }

        public override void onAddedToEntity()
        {
            entity.updateOrder = int.MaxValue;
        }
        public void update()
        {
            //follow camera at some specific relationship
            this.entity.position = new Vector2((camera.position.X * paralaxRatio) + offset.X, (camera.position.Y * paralaxRatio) + offset.Y);
        }
    }
}
