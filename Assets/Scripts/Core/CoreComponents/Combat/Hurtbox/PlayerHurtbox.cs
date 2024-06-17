using UnityEngine;

namespace Game.CoreSystem
{
    public class PlayerHurtbox : Hurtbox
    {
        protected override bool ExposeIFramesDuration => true;

        private SpriteBlink _spriteBlinker;

        protected override bool UseInvincibility => true;

        protected override void Awake()
        {
            base.Awake();

            _spriteBlinker = core.GetCoreComponent<SpriteBlink>();
        }

        public override bool Damage(object source, int amount, out int dealtDamage, int piercing = 0)
        {
            bool successfulHit = base.Damage(source, amount, out dealtDamage);

            if (successfulHit && Stats.IsAlive)
            {
                _spriteBlinker.PlayBlink(IFramesDuration);
            }

            return successfulHit;
        }
    }
}
