using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class BulletHandler : MonoBehaviour
{
    [Header("Bullet Parameters")]
    [SerializeField] private float _bulletDamage = 10f;
    [SerializeField] private float _bulletSpeed = 20f;

    private Rigidbody _rigidbody;
    private GameObject owner;
    private IObjectPool<BulletHandler> _poolRef;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Setup(GameObject shooter, IObjectPool<BulletHandler> _pool)
    {
        owner = shooter;
        _poolRef = _pool;

        _rigidbody.linearVelocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
        _rigidbody.linearVelocity = transform.forward * _bulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamageable target))
        {
            ContactPoint contact = collision.GetContact(0);

            DamageInfo damageInfo = new DamageInfo
            {
                ammount = _bulletDamage,
                hitPoint = contact.point,
                hitDirection = transform.forward,
                attacker = owner
            };

            target.TakeDamage(damageInfo);
        }

        ReleaseToPool();
    }

    private void ReleaseToPool()
    {
        if(gameObject.activeSelf)
            _poolRef.Release(this);
    }
}