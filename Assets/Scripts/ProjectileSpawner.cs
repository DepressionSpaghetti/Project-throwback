using UnityEngine;
using UnityEngine.Pool;

public class ProjectileSpawner : MonoBehaviour
{
    [Header("Capacity")]
    [SerializeField] private int _defaultPoolCapacity = 20;
    [SerializeField] private int _maxPoolSize = 100;

    [SerializeField] private BulletHandler _projectilePrefab;
    private IObjectPool<BulletHandler> _bulletPool;

    private void Awake()
    {
        _bulletPool = new ObjectPool<BulletHandler>(
            createFunc: () => Instantiate(_projectilePrefab),
            actionOnGet: (bullet) => bullet.gameObject.SetActive(true),
            actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),
            actionOnDestroy: (bullet) => Destroy(bullet.gameObject),
            collectionCheck: true,
            defaultCapacity: _defaultPoolCapacity,
            maxSize: _maxPoolSize
        );
    }
    
    public void Fire(Transform firePoint, GameObject shooter)
    {
        if (_projectilePrefab == null || firePoint == null) return;

        BulletHandler bullet = _bulletPool.Get();
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;

        bullet.Setup(shooter, _bulletPool);
    }
}
