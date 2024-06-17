using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface IKnockbackable
    {
        public void Knockback(Vector2 angle, float strength, bool ignoreConditions = false);
    }
}
