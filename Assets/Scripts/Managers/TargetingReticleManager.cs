using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TargetingReticleManager : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private Sprite[] animationFrames;
    [SerializeField] private float framesPerSecond = 10f;

    [Header("Depth Settings")]
    [SerializeField] private float _cameraOffsetDistance = 1.2f;

    private SpriteRenderer spriteRenderer;
    private int currentFrameIndex = 0;
    private float timer = 0f;
    private Transform activeCamera;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if(Camera.main != null)
        {
            activeCamera = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        currentFrameIndex = 0;
        timer = 0f;

        if(animationFrames != null && animationFrames.Length > 0)
        {
            spriteRenderer.sprite = animationFrames[0];
        }
    }

    public void UpdatePosition(Vector3 enemyPivotPosition)
    {
        if (activeCamera == null) return;

        Vector3 camToEnemyDir = (enemyPivotPosition - activeCamera.position).normalized;
        Vector3 clearPosition = enemyPivotPosition - (camToEnemyDir * _cameraOffsetDistance);
        transform.position = clearPosition;
    }

    void LateUpdate()
    {
        if (animationFrames != null && animationFrames.Length > 0)
        {
            timer += Time.deltaTime;
            float timePerFrame = 1f / framesPerSecond;

            if (timer >= timePerFrame)
            {
                currentFrameIndex = (currentFrameIndex + 1) % animationFrames.Length;
                spriteRenderer.sprite = animationFrames[currentFrameIndex];
                timer -= timePerFrame;
            }
        }

        if (activeCamera != null)
        {
            transform.rotation = activeCamera.rotation;
        }
    }
}