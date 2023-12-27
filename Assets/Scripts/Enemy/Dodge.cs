using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Enemy
{
    public class Dodge
    {
        private float dodgeChance;
        private bool isActiveMode;
        private float dodgeDistance;
        private float timeDodge;
        private bool timeActiveDodgeMode;

        public Dodge(float dodgeChance, float dodgeDistance, bool isActiveMode, float timeDodge)
        {
            this.dodgeChance = dodgeChance;
            this.isActiveMode = isActiveMode;
            this.dodgeDistance = dodgeDistance;
            this.timeDodge = timeDodge;
            timeActiveDodgeMode = false;
        }

        public float DodgeChance { get => dodgeChance; set => dodgeChance = value; }
        public bool IsActiveMode { get => isActiveMode; set => isActiveMode = value; }
        public float DodgeDistance { get => dodgeDistance; set => dodgeDistance = value; }
        public bool TimeActiveDodgeMode { get => timeActiveDodgeMode; set => timeActiveDodgeMode = value; }
        public float TimeDodge { get => timeDodge; set => timeDodge = value; }
    }
}
