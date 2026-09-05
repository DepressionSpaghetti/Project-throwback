using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] int ammoCount = 10;
    [SerializeField] int maxAmmoCount = 30;
    
    [Header("Project Settings")]
    [SerializeField] private float singlerecoilDuration = 0.25f;
    [SerializeField] private float autoRecoilDuration = 0.1f;
    private float _nextAllowedFireTime = 0f;

    private Animator _weaponAnimator;
    private bool _isFiringAuto;
    private Coroutine _autoFireCoroutine;

    public int AmmoCount => ammoCount;

    void Awake()
    {
        _weaponAnimator = GetComponent<Animator>();
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

        _nextAllowedFireTime = Time.time + singlerecoilDuration;

        PlayerController.Instance.IsRecoilLocked = true;

        _weaponAnimator.SetTrigger("singleShot");
        PlayerController.Instance.OnFireSingle();

        if(ProjectileSpawner.Instance != null)
            ProjectileSpawner.Instance.Fire(firePoint, gameObject);
    }

    private void OnFireAuto(bool isFiring)
    {
        if (!PlayerController.Instance.InBattle) return;
        
        _isFiringAuto = isFiring;

        if (!isFiring)
        {
            _weaponAnimator.SetBool("fullAuto", false);
            PlayerController.Instance.OnFireAuto(false);

            if(_autoFireCoroutine != null)
            {
                StopCoroutine(_autoFireCoroutine);
                _autoFireCoroutine = null;
            }

            return;
        }
        
        if(ammoCount <= 0) return;

        if (PlayerController.Instance.IsRecoilLocked) return;

        PlayerController.Instance.IsRecoilLocked = true;

        _weaponAnimator.SetBool("fullAuto", true);
        PlayerController.Instance.OnFireAuto(true);
        if (ProjectileSpawner.Instance != null && _autoFireCoroutine == null)
        {
            _autoFireCoroutine = StartCoroutine(FireRoutine());
        }
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

    private IEnumerator FireRoutine()
    {
        while(_isFiringAuto && ammoCount > 0)
        {
            ProjectileSpawner.Instance.Fire(firePoint, gameObject);

            yield return new WaitForSeconds(autoRecoilDuration);
        }

        _weaponAnimator.SetBool("fullAuto", false);
        PlayerController.Instance.OnFireAuto(false);
        _autoFireCoroutine = null;
    }
}
