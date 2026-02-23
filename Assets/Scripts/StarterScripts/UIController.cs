using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

    private bool _isPaused;

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
        //Time.timeScale = 0f;
        _roundText.text = text;
        _roundCanvas.enabled = true;
        yield return new WaitForSecondsRealtime(time);
        _roundCanvas.enabled = false;
        //Time.timeScale = 1f;
    }

    /*
    public void ShowRoundCanvas(int roundNum)
    {
        _roundText.text = $"Round {roundNum.ToString()}";
        _roundCanvas.enabled = true;
        StartCoroutine(DisplayRound());
    }

    private IEnumerator DisplayRound()
    {
        yield return new WaitForSeconds(1f);
        _roundCanvas.enabled = false;
    }
    
    public void ShowRoundCleared()
    {
        _roundText.text = $"Round cleared!";
        _roundCanvas.enabled = true;
        StartCoroutine(DisplayRound());
    }
    */

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