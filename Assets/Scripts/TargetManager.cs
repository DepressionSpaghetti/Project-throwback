using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class TargetManager : MonoBehaviour
{
    public List<GameObject> targets = new List<GameObject>();


    private static TargetManager _Instance;

    public static TargetManager Instance
    {
        get
        {
            if(!_Instance)
            {
                _Instance = new GameObject("GameStateManager").AddComponent<TargetManager>();
                DontDestroyOnLoad(_Instance.gameObject);
            }
            return _Instance;
        }
    }


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
    }

    public void RegisterTarget(GameObject target) => targets.Add(target);
    public void DeregisterTarget(GameObject target) => targets.Remove(target);

    public GameObject GetNextTarget(Transform player, GameObject currentTarget)
    {
        if (targets.Count == 0) return null;
        if(targets.Count == 1) return targets[0];

        targets.Sort
        (   
            (a, b) => Vector3.Distance(player.position, a.transform.position)
            .CompareTo(Vector3.Distance(player.position, b.transform.position))
        );

        int currentIndex = targets.IndexOf(currentTarget);
        int nextIndex = (currentIndex + 1) % targets.Count;

        return targets[nextIndex];
    }

}
