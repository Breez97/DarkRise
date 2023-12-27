using UnityEngine;

namespace Assets.Scripts.Enemy.Ghost
{
    public class Vanish
    {
        private bool isActive;
        private float chance;
        private float reloadTime;
        private float lastVanishTime;

        public Vanish(bool isActive, float chance, float reloadTime = 10f)
        {
            this.isActive = isActive;
            this.chance = chance;
            this.reloadTime = reloadTime;
            this.lastVanishTime = -reloadTime;
        }

        public bool CanVanish()
        {
            return Time.time - lastVanishTime >= reloadTime;
        }

        public void UseVanish()
        {
            if (CanVanish())
            {
                lastVanishTime = Time.time;
            }
        }

        public bool IsActive { get => isActive; set => isActive = value; }
        public float Chance { get => chance; set => chance = value; }
        public float ReloadTime { get => reloadTime; set => reloadTime = value; }
    }
}
