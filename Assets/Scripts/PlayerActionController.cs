using System.Collections;
using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    private Rigidbody2D _rb;
    private SpriteRenderer _sr;
    private Camera _camera;

    public bool IsBlocking { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _camera = Camera.main;
    }

    public void ExecuteActions(FrameData frameData)
    {
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
        IsBlocking = blockHeld;

        if (blockHeld)
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
        Debug.Log("Attack executed");
        StartCoroutine(TestAttack());
    }

    #endregion

    private IEnumerator TestAttack()
    {
        _sr.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        _sr.color = Color.white;
    }
}