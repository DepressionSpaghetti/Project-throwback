using UnityEngine;

public class EnemyController : MonoBehaviour
{


    private void OnEnable()
    {
        TargetManager.Instance.RegisterTarget(this.gameObject);
    }

    private void OnDisable()
    {
        TargetManager.Instance.DeregisterTarget(this.gameObject);
        if (TargetManager.Instance.targets.Count == 0) PlayerController.Instance.EndBattle();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
