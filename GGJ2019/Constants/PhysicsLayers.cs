using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Constants
{
    public static class PhysicsLayers
    {
        public const int tiles = 1 << 0;
        public const int move = 1 << 1;
        public const int playerHit = 1 << 2;
        public const int enemyHit = 1 << 3;
        public const int playerBullet = 1 << 4;
        public const int enemyBullet = 1 << 5;
        public const int noHit = 1 << 6;
        public const int mcguff = 1 << 7;
        public const int collect = 1 << 8;
    }
}
