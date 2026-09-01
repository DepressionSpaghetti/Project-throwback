using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    private Animator _playerAnimator;

    private float _movementInput;
    private bool _inBattle = false;

    //Movement parameters
    [Header("Movement Parameters")]
    [Tooltip("The speed at which the player rotates.")]
    [SerializeField] private float _rotationSpeed = 10f;

    //Targeting parameters
    [Header("Targeting Parameters")]
    [SerializeField] private GameObject _currentTarget;
    [SerializeField] private GameObject _targetingReticleUI;

    //IK
    [Header("IK Parameters")]
    [SerializeField] private Transform _smoothIKTarget;
    [SerializeField] private float _ikSmoothSpeed = 15f;


    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        _playerAnimator = GetComponent<Animator>();

        ControlManager.Instance.Move += OnMove;
        ControlManager.Instance.Interact += OnInteract;
        ControlManager.Instance.SwitchEngageTarget += OnSwitchEngageTarget;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateCharacter(_movementInput);
        RotateBattle();

    }

    private void LateUpdate()
    {
        if (_inBattle && _currentTarget != null)
        {
            //_targetingReticleUI.transform.position = _currentTarget.transform.position;
            //
            //// Optional: Keeps the reticle facing flat forward relative to the world/camera
            //_targetingReticleUI.transform.rotation = Quaternion.identity;
            if (_targetingReticleUI.TryGetComponent(out TargetingReticleManager reticleManager))
            {
                reticleManager.UpdatePosition(_currentTarget.transform.position);
            }
        }
    }


    private void OnMove(Vector2 moveInput)
    {
        if(_playerAnimator == null) return;

        _movementInput = moveInput.x;
        switch (moveInput)
        {
            //directional movement
            //forward
            case { x: 0, y: 1}:
                SetAnimatorParameters(forward: true, backward: false, turnLeft: false, turnRight: false);
                break;
            //backward
            case { x: 0, y: -1 }:
                SetAnimatorParameters(forward: false, backward: true, turnLeft: false, turnRight: false);
                break;
            //turning right
            case { x: 1, y: 0 }:
                SetAnimatorParameters(forward: false, backward: false, turnLeft: false, turnRight: true);
                break;
            //turning left
            case { x: -1, y: 0 }:
                SetAnimatorParameters(forward: false, backward: false, turnLeft: true, turnRight: false);
                break;
            //Combined movement
            //forward and turning right
            //case { x: 1, y: 1 }:
            //    SetAnimatorParameters(forward: true, backward: false, turnLeft: false, turnRight: true);
            //    break;
            ////forward and turning left
            //case { x: -1, y: 1 }:
            //    SetAnimatorParameters(forward: true, backward: false, turnLeft: true, turnRight: false);
            //    break;
            ////backward and turning right
            //case { x: 1, y: -1 }:
            //    SetAnimatorParameters(forward: false, backward: true, turnLeft: false, turnRight: true);
            //    break;
            ////backward and turning left
            //case { x: -1, y: -1 }:
            //    SetAnimatorParameters(forward: false, backward: true, turnLeft: true, turnRight: false);
            //    break;
            default:
                _movementInput = 0;
                SetAnimatorParameters(forward: false, backward: false, turnLeft: false, turnRight: false);
                break;
        }
    }

    private void OnInteract()
    {
        // Handle player interaction here
        Debug.Log("Player is interacting");
    }

    private void OnSwitchEngageTarget()
    {
        if(!_inBattle && TargetManager.Instance.targets.Count == 0)
        {
            return;
        }
        else if(_inBattle && TargetManager.Instance.targets.Count > 1)
        {
            SwitchTargets();
        }
        else if (!_inBattle && TargetManager.Instance.targets.Count > 0)
        {
            _inBattle = true;
            SwitchTargets();
            _targetingReticleUI.SetActive(true);
        }
    }

    private void SwitchTargets()
    {
        _currentTarget = TargetManager.Instance.GetNextTarget(transform, _currentTarget);
        //_targetingReticleUI.transform.position = _currentTarget.transform.position;
    }

    public void EndBattle()
    {
        _inBattle = false;
        _currentTarget = null;
        _targetingReticleUI.SetActive(false);
        ResetIK();
    }

    private void RotateBattle()
    {
        if(_inBattle)
        {
            Vector3 DirectionToTarget = _currentTarget.transform.position - transform.position;
            DirectionToTarget.y = 0; // Keep the rotation only on the horizontal plane

            if(DirectionToTarget != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(DirectionToTarget);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }

            if(_smoothIKTarget != null)
            {
                _smoothIKTarget.position = Vector3.MoveTowards
                (
                    _smoothIKTarget.position, _targetingReticleUI.transform.position, 
                    _ikSmoothSpeed * Time.deltaTime
                );
            }
        }
    }

    

    private void RotateCharacter(float _movementInput)
    {
        if(Mathf.Abs(_movementInput) > 0.1f)
            transform.Rotate(Vector3.up, _movementInput * _rotationSpeed * Time.deltaTime);
        //if(_inBattle && _currentTarget != null)
        //{
        //    Vector3 directionToTarget = _currentTarget.transform.position - this.transform.position;
        //    directionToTarget.y = 0; // Keep the rotation only on the horizontal plane
        //    if (directionToTarget != Vector3.zero)
        //    {
        //        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        //        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
        //    }
        //}

    }

    void SetAnimatorParameters(bool forward, bool backward, bool turnLeft, bool turnRight)
    {
        _playerAnimator.SetBool("isWalking", forward);
        _playerAnimator.SetBool("isWalkingBackwards", backward);
        _playerAnimator.SetBool("turningLeft", turnLeft);
        _playerAnimator.SetBool("turningRight", turnRight);

    }

    private void ResetIK()
    {
        if(!_inBattle && _smoothIKTarget != null)
        {
            Vector3 forwardPosition = transform.position + (transform.forward * 2f);

            _smoothIKTarget.position = Vector3.MoveTowards
                (
                    _smoothIKTarget.position, forwardPosition, _ikSmoothSpeed * Time.deltaTime
                );
        }
    }
}
