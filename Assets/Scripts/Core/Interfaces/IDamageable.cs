public interface IDamageable
{
    public bool Damage(object source, int amount, out int dealtDamage, int piercing = 0);
}
