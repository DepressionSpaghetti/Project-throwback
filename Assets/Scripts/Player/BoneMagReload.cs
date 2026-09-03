using UnityEngine;

public class BoneMagReload : MonoBehaviour
{
    private Animator _animator;

    [Header("Bones")]
    [SerializeField] private Transform weaponMagBone;
    [SerializeField] private Transform playerLeftHand;

    private Vector3 _magHomePosition;
    private Quaternion _magHomeRotation;
    private bool hasSavedHome;

    private bool _isGrabbedByHand = false;
    private Vector3 _positionOffset;
    private Quaternion _rotationOffset;

    public void GrabMagWithHand()
    {
        if (weaponMagBone == null || playerLeftHand == null) return;

        if(!hasSavedHome)
        {
            _magHomePosition = weaponMagBone.localPosition;
            _magHomeRotation = weaponMagBone.localRotation;
            hasSavedHome = true;
        }

        _positionOffset = playerLeftHand.InverseTransformPoint(weaponMagBone.position);
        _rotationOffset = Quaternion.Inverse(playerLeftHand.rotation) * weaponMagBone.rotation;

        _isGrabbedByHand = true;
        Debug.Log("mag grabbed by hand");
    }

    public void ReleaseMagToWeapon()
    {
        _isGrabbedByHand = false;

        if(weaponMagBone != null)
        {
            weaponMagBone.localPosition = _magHomePosition;
            weaponMagBone.localRotation = _magHomeRotation;

            if (_animator != null) _animator.Update(0f);
        }
        Debug.Log("mag released to weapon");
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (_isGrabbedByHand && weaponMagBone != null && playerLeftHand != null)
        {
            weaponMagBone.position = playerLeftHand.TransformPoint(_positionOffset);
            weaponMagBone.rotation = playerLeftHand.rotation * _rotationOffset;
        }
    }
}
