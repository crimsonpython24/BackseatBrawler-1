using UnityEngine;
using UnityEngine.InputSystem;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private InputActionReference _restartAction;
    private void Awake()
    {
        SetupGame();
        
        _restartAction.action.Enable();
        _restartAction.action.performed += RestartGame;
    }

    private void RestartGame(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.RestartLevel();
    }

    private static void SetupGame()
    {
        GameManager.Setup();
    }
}
