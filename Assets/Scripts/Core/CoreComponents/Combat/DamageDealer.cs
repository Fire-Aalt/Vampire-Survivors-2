using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DamageDealer : MonoBehaviour
{
    private BoxCollider2D _hitBox;
    public int damage;
    public LayerMask damageableMask;

    // Start is called before the first frame update
    void Start()
    {
        _hitBox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        var point = _hitBox.offset + (Vector2)_hitBox.transform.position;
        var colliders = Physics2D.OverlapBoxAll(point, _hitBox.size, 0f, damageableMask);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent(out IDamageable damageable)) 
            {
                damageable.Damage(this, damage, out int dealtDamage);
            }
        }
    }
}
