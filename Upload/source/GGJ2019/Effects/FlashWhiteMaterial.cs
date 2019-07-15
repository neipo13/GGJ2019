using Microsoft.Xna.Framework.Graphics;
using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Effects
{
    public class FlashWhiteMaterial : Material
    {
        private bool flashing;
        private EffectParameter flashingParam;

        public bool Flashing
        {
            get
            {
                return flashing;
            }
            set
            {
                flashing = value;
                flashingParam?.SetValue(value);
            }
        }
        public FlashWhiteMaterial(Effect effect) : base(effect)
        {
            flashingParam = effect.Parameters["flashing"];
        }
    }
}
