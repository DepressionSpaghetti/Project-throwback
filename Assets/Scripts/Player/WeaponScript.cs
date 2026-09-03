using System;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] int ammoCount = 10;
    [SerializeField] int maxAmmoCount = 30;
    
    [Header("Project Settings")]
    [SerializeField] private float recoilDuration = 0.25f;
    private float _nextAllowedFireTime = 0f;

    private ProjectileSpawner _spawner;
    private Animator _weaponAnimator;
    private bool _isFiringAuto;

    public int AmmoCount => ammoCount;

    void Awake()
    {
        _weaponAnimator = GetComponent<Animator>();
        _spawner = GetComponent<ProjectileSpawner>();
        ControlManager.Instance.Attack1 += OnFireAuto;
        ControlManager.Instance.Attack2 += OnFireSingle;
    }

    private void Update()
    {
        
    }

    private void OnFireSingle()
    {
        if (!PlayerController.Instance.InBattle) return;
        
        if(Time.time < _nextAllowedFireTime) return;

        if(ammoCount <= 0) return;

        _nextAllowedFireTime = Time.time + recoilDuration;

        PlayerController.Instance.IsRecoilLocked = true;

        _weaponAnimator.SetTrigger("singleShot");
        PlayerController.Instance.OnFireSingle();

        //_spawner.Fire(firePoint, gameObject);
    }

    private void OnFireAuto(bool isFiring)
    {
        if (!PlayerController.Instance.InBattle) return;
        
        if(!isFiring)
        {
            _weaponAnimator.SetBool("fullAuto", false);
            PlayerController.Instance.OnFireAuto(false);
            return;
        }
        
        if(ammoCount <= 0) return;

        if (PlayerController.Instance.IsRecoilLocked) return;

        PlayerController.Instance.IsRecoilLocked = true;

        _weaponAnimator.SetBool("fullAuto", true);
        PlayerController.Instance.OnFireAuto(true);
        //if(isFiring)
            //_spawner.Fire(firePoint, gameObject);
    }

    public void DepleteAmmo()
    {
        ammoCount = Mathf.Max(0, ammoCount - 1);

        _weaponAnimator.SetInteger("ammoCount", ammoCount);
    }

    public void ReloadAmmo()
    {
        ammoCount = (int)MathF.Max(0, maxAmmoCount);
        _weaponAnimator.SetInteger("ammoCount", ammoCount);
    }

    public void SlapBolt() => _weaponAnimator.SetTrigger("slapBolt");

    public void UnlockBoltCycle()
    {
        PlayerController.Instance.IsRecoilLocked = false;
    }
}
