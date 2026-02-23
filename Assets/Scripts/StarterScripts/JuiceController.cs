using System.Collections;
using UnityEngine;

public class JuiceController : Singleton<JuiceController>
{
    [Header("Slowdown")]
    [SerializeField] private float slowdownTimeScale = 0.1f;
    [SerializeField] private float slowdownRecoverSpeed = 10f;

    [Header("Screen Shake")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float shakeFrequency = 25f;

    private Coroutine slowdownRoutine;
    private Coroutine shakeRoutine;

    private Vector3 cameraStartPos;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        cameraStartPos = mainCamera.transform.localPosition;
    }

    // =====================================================
    // SLOWDOWN / HITSTOP
    // =====================================================
    public void Slowdown(float duration)
    {
        if (slowdownRoutine != null)
            StopCoroutine(slowdownRoutine);

        slowdownRoutine = StartCoroutine(SlowdownCoroutine(duration));
    }

    private IEnumerator SlowdownCoroutine(float duration)
    {
        Time.timeScale = slowdownTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        yield return new WaitForSecondsRealtime(duration);

        // Smooth recovery
        while (Time.timeScale < 1f)
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1f, slowdownRecoverSpeed * Time.unscaledDeltaTime);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            yield return null;
        }

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        slowdownRoutine = null;
    }

    // =====================================================
    // SCREEN SHAKE
    // =====================================================
    public void ScreenShake(float intensity, float duration)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(ScreenShakeCoroutine(intensity, duration));
    }

    private IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;

            Vector3 randomOffset = Random.insideUnitSphere * intensity;
            randomOffset.z = 0f; // 2D-safe
            mainCamera.transform.localPosition = cameraStartPos + randomOffset;

            yield return null;
        }

        mainCamera.transform.localPosition = cameraStartPos;
        shakeRoutine = null;
    }
}
