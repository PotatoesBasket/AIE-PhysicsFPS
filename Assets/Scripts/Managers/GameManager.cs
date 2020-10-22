using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager current = null;

    [SerializeField] GameObject pauseScreen = null;

    public static bool IsPaused { get; private set; } = true;

    public delegate void Pause();
    public static event Pause OnPause;

    private void Awake()
    {
        if (current == null)
            current = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (Input.GetKeyDown(KeyCode.P))
        {
            pauseScreen.SetActive(!IsPaused);
            SetPauseState(!IsPaused);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
            SetPauseState(true);
        }
    }

    public void SetPauseState(bool isPaused)
    {
        IsPaused = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused ? true : false;
        OnPause?.Invoke();
    }
}