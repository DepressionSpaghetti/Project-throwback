using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(PlayerInput))]
public class ControlManager : MonoBehaviour
{
    private static ControlManager _Instance;

    public static ControlManager Instance
    {
        get
        {
            if(!_Instance)
            {
                _Instance = new GameObject("ControlManager").AddComponent<ControlManager>();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }

    PlayerInput playerInputComponent;
    InputActionAsset inputActions;

    //InputAction declarations
    InputAction moveAction;
    InputAction attackAction1;
    InputAction attackAction2;
    InputAction interactAction;
    InputAction switchEngageTargetAction;
    InputAction reloadAction;

    //Event declarations
    public event Action<Vector2> Move;
    public event Action<bool> Attack1;
    public event Action Attack2;
    public event Action Interact;
    public event Action SwitchEngageTarget;
    public event Action Reload;

    //test
    public Vector2 moveInput;
    public bool attack1Input;
    public bool attack2Input;
    public bool interactInput;
    public bool switchEngageTargetInput;
    public bool reloadInput;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        var trigger = Instance;
    }

    private void Awake()
    {
        if(_Instance != null && _Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        _Instance = this;
        DontDestroyOnLoad(this.gameObject);

        playerInputComponent = GetComponent<PlayerInput>();
        inputActions = Resources.Load<InputActionAsset>("PlayerInputManager");

        if (inputActions != null)
        {
            playerInputComponent.actions = inputActions;
            //playerInputComponent.defaultControlScheme = "Keyboard&Mouse";
            playerInputComponent.defaultControlScheme = "Gamepad";
            playerInputComponent.neverAutoSwitchControlSchemes = false; //set to true in production
            playerInputComponent.defaultActionMap = "Player";
            playerInputComponent.notificationBehavior = PlayerNotifications.SendMessages;
            playerInputComponent.ActivateInput();
        }
        else Debug.LogError("Could not find the Input Action Asset in the Resources folder! Reverting to global defaults.");

        //Set actions
        moveAction = playerInputComponent.actions.FindAction("Move");
        attackAction1 = playerInputComponent.actions.FindAction("Attack 1");
        attackAction2 = playerInputComponent.actions.FindAction("Attack 2");
        interactAction = playerInputComponent.actions.FindAction("Interact");
        switchEngageTargetAction = playerInputComponent.actions.FindAction("Switch/engage target");
        reloadAction = playerInputComponent.actions.FindAction("Reload");

        //State tracking
        //move
        moveAction.performed += ctx => Move?.Invoke(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => Move?.Invoke(Vector2.zero);
        //Weapon manipulation
        //Attack1
        attackAction1.started += ctx => Attack1?.Invoke(true);
        attackAction1.canceled += ctx => Attack1?.Invoke(false);
        //Attack2
        attackAction2.performed += ctx => Attack2?.Invoke();
        //Switch/engage target
        switchEngageTargetAction.performed += ctx => SwitchEngageTarget?.Invoke();
        //Reload
        reloadAction.performed += ctx => Reload?.Invoke();
        //Interact
        interactAction.performed += ctx => Interact?.Invoke();

        Debug.Log("Controllers: " + Gamepad.all.Count);
        SwitchControlScheme();
    }

    private void Update()
    {
        
        moveInput = moveAction.ReadValue<Vector2>();
        attack1Input = attackAction1.ReadValue<float>() == 1 ? true : false;
        attack2Input = attackAction2.ReadValue<float>() == 1 ? true : false;
        interactInput = interactAction.ReadValue<float>() == 1 ? true : false;
        switchEngageTargetInput = switchEngageTargetAction.ReadValue<float>() == 1 ? true : false;
        reloadInput = reloadAction.ReadValue<float>() == 1 ? true : false;

    }

    public void SwitchControlScheme()
    {
        if (playerInputComponent.currentControlScheme == "Keyboard&Mouse")
            playerInputComponent.SwitchCurrentControlScheme("Gamepad");
        else
            playerInputComponent.SwitchCurrentControlScheme("Keyboard&Mouse");
    }
}
