using System.Collections;
using UnityEngine;

public class DoorScriptSceneChange : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 openOffset = new Vector3(0f, 3f, 0f);
    [SerializeField] private float openDuration = 2f;
    private Transform door;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private Coroutine openCoroutine;
    private bool isOpen = false;

    void Awake()
    {
        if(transform.childCount >0)
            door = transform.GetChild(0);
        else 
            Debug.LogError("DoorScript: No child object found for the door.");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        closedPosition = door.position;
        openPosition = closedPosition + openOffset;
    }

    public void Interact()
    {
            isOpen = !isOpen;
            Vector3 targetPosition = isOpen ? openPosition : closedPosition;

            if (openCoroutine != null) StopCoroutine(openCoroutine);
            openCoroutine = StartCoroutine(SlideDoor(targetPosition));
    }

    private IEnumerator SlideDoor(Vector3 targetPosition)
    {
        float elapsedTime = 0f;
        Vector3 startingPosition = door.position;

        while(elapsedTime < openDuration)
        {
            elapsedTime += Time.deltaTime;
            door.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / openDuration);
            yield return null;
        }

        door.position = targetPosition;
    }
}
