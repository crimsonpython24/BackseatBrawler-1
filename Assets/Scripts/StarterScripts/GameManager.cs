using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.SceneManagement;
using Object = System.Object;

public class GameManager : Singleton<GameManager>
{
    private List<PlayerController> players;

    public List<PlayerController> Players
    {
        get
        {
            if (players == null)
            {
                players = UnityEngine.Object.FindObjectsByType<PlayerController>(FindObjectsSortMode.None).ToList();
            }
            return players;
        }
    }
    
    
    private UIController uiController;
    public UIController UIController
    {
        get
        {
            if (uiController == null)
            {
                uiController = GameObject.FindGameObjectWithTag("UIController").GetComponent<UIController>();
            }
            return uiController;
        }
    }
    
    private RoundController roundController;
    public RoundController RoundController
    {
        get
        {
            if (roundController == null)
            {
                roundController = GameObject.FindGameObjectWithTag("RoundController").GetComponent<RoundController>();
            }
            return roundController;
        }
    }
    
    
    public static void Setup()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        Time.fixedDeltaTime = 1f / 60f;
        
        var playerInputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);

        if (playerInputs.Length == 0)
        {
            Debug.LogError("No PlayerInput components found in scene!");
            return;
        }

        if (Gamepad.all.Count == 0)
        {
            Debug.LogError("No gamepads connected!");
            return;
        }

        int pairCount = Mathf.Min(playerInputs.Length, Gamepad.all.Count, 2);

        for (int i = 0; i < pairCount; i++)
        {
            PairDevice(playerInputs[i], Gamepad.all[i]);
        }

        Debug.Log($"Paired {pairCount} PlayerInputs.");
    }

    private static void PairDevice(PlayerInput input, InputDevice device)
    {
        if (input == null || device == null)
        {
            Debug.LogError("Cannot pair device: null reference");
            return;
        }

        var user = input.user;

        user.UnpairDevices();
        InputUser.PerformPairingWithDevice(device, user);

        input.SwitchCurrentControlScheme(device);

        Debug.Log($"Paired {input.gameObject.name} to {device.displayName}");
    }
    

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}