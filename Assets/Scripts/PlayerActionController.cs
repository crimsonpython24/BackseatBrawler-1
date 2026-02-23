using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Combat")]
    [SerializeField] private float punchRange = 0.2f;
    [SerializeField] private float attackFlashDuration = 0.2f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Camera _camera;
    private PlayerController _owner;

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
        _owner = GetComponent<PlayerController>();

        AssignFighterSprite();
    }

    public void ExecuteActions(FrameData frameData)
    {
        if (_owner == null || !_owner.CanExecuteActions())
        {
            HandleBlocking(false);
            return;
        }

        HandleBlocking(frameData.BlockHeld);
        HandleMovement(frameData.Movement);

        if (frameData.PunchPressed && !IsBlocking)
        {
            Attack();
        }
    }

    #region Movement

    private void HandleMovement(Vector2 input)
    {
        if (input.sqrMagnitude < 0.01f)
            return;

        Vector2 move = Vector2.right * input.x;

        _rb.MovePosition(transform.position + Time.fixedDeltaTime * moveSpeed * new Vector3(move.x, 0f, 0f));
    }

    #endregion

    #region Blocking

    private void HandleBlocking(bool blockHeld)
    {
        bool canBlock = blockHeld && IsTargetInRange();
        IsBlocking = canBlock;

        if (canBlock)
        {
            _sr.color = Color.blue;
        }
        else
        {
            _sr.color = Color.white;
        }
    }

    #endregion

    #region Attack

    private void Attack()
    {
        StartCoroutine(PlayAttackFlash());

        PlayerController target = _owner.GetOpponent();
        if (target == null || target.CurrentState == PlayerController.PlayerState.Dead)
        {
            _owner.AddExhaustion(1f);
            return;
        }

        if (!IsTargetInRange())
        {
            _owner.AddExhaustion(1f);
            return;
        }

        PlayerActionController targetActionController = target.GetComponent<PlayerActionController>();

        if (targetActionController != null && targetActionController.IsBlocking)
        {
            _owner.AddExhaustion(2f);
            return;
        }

        target.RegisterHitTaken();
    }

    #endregion

    private bool IsTargetInRange()
    {
        PlayerController target = _owner.GetOpponent();
        if (target == null)
            return false;

        return Vector2.Distance(transform.position, target.transform.position) <= punchRange;
    }

    private IEnumerator PlayAttackFlash()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(attackFlashDuration);

        if (!IsBlocking)
        {
            _sr.color = Color.white;
        }
    }


    private Texture2D BuildPixelFighterTexture(bool isPlayerOne)
    {
        Texture2D texture = new Texture2D(32, 32);
        texture.filterMode = FilterMode.Point;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color body = isPlayerOne ? new Color(0.93f, 0.74f, 0.51f) : new Color(0.75f, 0.86f, 0.55f);
        Color accent = isPlayerOne ? new Color(0.84f, 0.24f, 0.28f) : new Color(0.28f, 0.45f, 0.88f);
        Color face = new Color(0.1f, 0.1f, 0.1f);

        for (int x = 0; x < 32; x++)
        {
            for (int y = 0; y < 32; y++)
            {
                texture.SetPixel(x, y, clear);
            }
        }

        FillRect(texture, 10, 20, 12, 8, body);
        FillRect(texture, 8, 8, 16, 12, body);
        FillRect(texture, 5, 10, 3, 7, accent);
        FillRect(texture, 24, 10, 3, 7, accent);
        FillRect(texture, 10, 2, 4, 6, accent);
        FillRect(texture, 18, 2, 4, 6, accent);
        FillRect(texture, 12, 22, 2, 2, face);
        FillRect(texture, 18, 22, 2, 2, face);

        texture.Apply();
        return texture;
    }

    private void FillRect(Texture2D texture, int startX, int startY, int width, int height, Color color)
    {
        for (int x = startX; x < startX + width; x++)
        {
            for (int y = startY; y < startY + height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }
    }

    private void AssignFighterSprite()
    {
        PlayerInput input = GetComponent<PlayerInput>();
        int playerIndex = input != null ? input.playerIndex : 0;

        Texture2D fighterTexture = BuildPixelFighterTexture(playerIndex == 0);
        Sprite generatedSprite = Sprite.Create(fighterTexture, new Rect(0, 0, fighterTexture.width, fighterTexture.height),
            new Vector2(0.5f, 0.5f), 32f);
        _sr.sprite = generatedSprite;
    }
}
