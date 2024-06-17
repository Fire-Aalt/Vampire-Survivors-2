using UnityEngine;

namespace Game.CoreSystem
{
    public class EnemyHurtbox : Hurtbox
    {
        private SpriteShaderController _shaderController;

        protected override bool UseInvincibility => false; 

        protected override void Awake()
        {
            base.Awake();

            _shaderController = core.GetCoreComponent<SpriteShaderController>();
        }

        public override bool Damage(object source, int amount, out int dealtDamage, int piercing = 0)
        {
            bool successfulHit = base.Damage(source, amount, out dealtDamage, piercing);

            if (successfulHit)
            {
                if (Stats.IsAlive)
                {
                    _shaderController.PlayFlash();
                }
                else
                {
                    _shaderController.PlayDissolve();
                }
            }

            return successfulHit;
        }
    }
}
