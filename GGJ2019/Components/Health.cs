using Nez;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GGJ2019.Components
{
    public class Health : Component
    {
        int hp = 1;
        int maxHp = 3;
        Action onHit;
        Action onDeath;

        public Health(int hp = 1, Action onHit = null, Action onDeath = null) : base()
        {
            this.hp = hp;
            this.onHit = onHit;
            this.onDeath = onDeath;
        }

        public void TakeDamage(int dmg = 1)
        {
            if (dmg < 0) return;
            hp -= dmg;
            if (hp <= 0)
            {
                onDeath?.Invoke();
            }
            else
            {
                onHit?.Invoke();
            }
        }

        public void Heal(int heals = 1)
        {
            if (heals < 0) return;
            hp += heals;
            if (hp > maxHp)
            {
                hp = maxHp;
            }
        }
    }
}
