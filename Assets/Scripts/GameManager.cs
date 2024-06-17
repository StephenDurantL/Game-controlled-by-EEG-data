using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int world;
    [SerializeField] private int stage;

    public int World => world;
    public int Stage => stage;

    public int lives { get; private set; } = 3;

    public Image[] livesSprites;

    // Update the visual representation of remaining lives
    public void UpdateLivesDisplay()
    {
        for (int i = 0; i < livesSprites.Length; i++)
        {
            livesSprites[i].enabled = i < lives;
        }
    }

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        // Clear the instance reference when destroyed
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        // Cap the frame rate to 60 fps
        Application.targetFrameRate = 60;
        // Start a new game
        NewGame();
    }

    public void NewGame()
    {
        // Update the lives display and load the initial level
        UpdateLivesDisplay();
        LoadLevel(world, stage);
    }

    public void GameOver()
    {
        // Load the game over scene
        SceneManager.LoadScene("GameOver");
    }

    public void LoadLevel(int world, int stage)
    {
        // Set the current world and stage, then load the corresponding scene
        this.world = world;
        this.stage = stage;
        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void ResetLevel()
    {
        // Decrement lives, update the lives display, and reload the current level if lives remain
        lives--;
        UpdateLivesDisplay();
        if (lives > 0)
        {
            LoadLevel(world, stage);
        }
        else
        {
            GameOver();
        }
    }

    public void ResetLevel(float delay)
    {
        // Reset the level after a delay
        Invoke(nameof(ResetLevel), delay);
    }

    public void NextLevel()
    {
        // Load the next stage of the current world
        LoadLevel(world, stage + 1);
    }
}
