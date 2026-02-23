using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private Canvas _roundCanvas;
    [SerializeField] private Canvas _levelEndCanvas;
    [SerializeField] private Canvas _pauseMenuCanvas;

    [Header("Misc")]
    [SerializeField] private TextMeshProUGUI _winText;
    [SerializeField] private TextMeshProUGUI _roundText;

    [Header("Input")]
    [SerializeField] private InputActionReference _pauseAction;

    [Header("HUD")]
    [SerializeField] private Color p1Color = new(0.95f, 0.35f, 0.35f);
    [SerializeField] private Color p2Color = new(0.35f, 0.55f, 0.95f);
    [SerializeField] private Color exhaustionColor = new(0.95f, 0.8f, 0.35f);
    [SerializeField] private Color hitsColor = new(0.95f, 0.35f, 0.35f);

    private bool _isPaused;

    private Texture2D _barTexture;
    private float _p1ExhaustionVisual;
    private float _p2ExhaustionVisual;
    private float _p1HitsVisual;
    private float _p2HitsVisual;

    private void Awake()
    {
        _barTexture = new Texture2D(1, 1);
        _barTexture.SetPixel(0, 0, Color.white);
        _barTexture.Apply();
    }

    private void OnEnable()
    {
        _pauseAction.action.performed += OnPausePressed;
        _pauseAction.action.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.action.performed -= OnPausePressed;
        _pauseAction.action.Disable();
    }

    private void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.Players.Count < 2)
            return;

        PlayerController p1 = GameManager.Instance.Players[0];
        PlayerController p2 = GameManager.Instance.Players[1];

        _p1ExhaustionVisual = Mathf.MoveTowards(_p1ExhaustionVisual, p1.Exhaustion, Time.deltaTime * 4f);
        _p2ExhaustionVisual = Mathf.MoveTowards(_p2ExhaustionVisual, p2.Exhaustion, Time.deltaTime * 4f);
        _p1HitsVisual = Mathf.MoveTowards(_p1HitsVisual, p1.HitsTaken, Time.deltaTime * 8f);
        _p2HitsVisual = Mathf.MoveTowards(_p2HitsVisual, p2.HitsTaken, Time.deltaTime * 8f);
    }

    private void OnGUI()
    {
        if (GameManager.Instance == null || GameManager.Instance.Players.Count < 2)
            return;

        PlayerController p1 = GameManager.Instance.Players[0];
        PlayerController p2 = GameManager.Instance.Players[1];

        DrawPlayerBars(new Rect(30f, 30f, 280f, 18f), "P1", p1Color, exhaustionColor, hitsColor, _p1ExhaustionVisual,
            p1.MaxExhaustion, _p1HitsVisual, p1.MaxHitsTaken);

        DrawPlayerBars(new Rect(Screen.width - 310f, 30f, 280f, 18f), "P2", p2Color, exhaustionColor, hitsColor,
            _p2ExhaustionVisual, p2.MaxExhaustion, _p2HitsVisual, p2.MaxHitsTaken);
    }

    private void DrawPlayerBars(Rect topRect, string playerLabel, Color frameColor, Color exColor, Color hitColor,
        float exhaustion, float maxExhaustion, float hits, float maxHits)
    {
        GUI.color = frameColor;
        GUI.DrawTexture(new Rect(topRect.x - 4, topRect.y - 22, topRect.width + 8, 70f), _barTexture);

        GUI.color = Color.black;
        GUI.DrawTexture(topRect, _barTexture);
        GUI.DrawTexture(new Rect(topRect.x, topRect.y + 26f, topRect.width, topRect.height), _barTexture);

        GUI.color = exColor;
        GUI.DrawTexture(new Rect(topRect.x + 2f, topRect.y + 2f, (topRect.width - 4f) * (exhaustion / maxExhaustion), topRect.height - 4f), _barTexture);

        GUI.color = hitColor;
        GUI.DrawTexture(new Rect(topRect.x + 2f, topRect.y + 28f, (topRect.width - 4f) * (hits / maxHits), topRect.height - 4f), _barTexture);

        GUI.color = Color.white;
        GUI.Label(new Rect(topRect.x + 4f, topRect.y - 20f, 180f, 20f), playerLabel);
        GUI.Label(new Rect(topRect.x + 4f, topRect.y + 1f, 250f, 20f), $"Exhaustion {Mathf.CeilToInt(exhaustion)}/{Mathf.RoundToInt(maxExhaustion)}");
        GUI.Label(new Rect(topRect.x + 4f, topRect.y + 27f, 250f, 20f), $"Hits {Mathf.CeilToInt(hits)}/{Mathf.RoundToInt(maxHits)}");
    }

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;

        Time.timeScale = _isPaused ? 0f : 1f;

        EnablePauseMenu(_isPaused);
    }

    public void EnablePauseMenu(bool enable)
    {
        _pauseMenuCanvas.enabled = enable;
    }

    public IEnumerator ShowRoundBanner(string text, float time)
    {
        _roundText.text = text;
        _roundCanvas.enabled = true;
        yield return new WaitForSecondsRealtime(time);
        _roundCanvas.enabled = false;
    }

    public void EnableWinCanvas(bool playerWon)
    {
        if (playerWon)
        {
            _winText.color = Color.green;
            _winText.text = "You won!";
        }
        else
        {
            _winText.color = Color.red;
            _winText.text = "You died!";
        }

        _levelEndCanvas.enabled = true;
    }
}
