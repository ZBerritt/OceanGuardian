using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public LevelLoader loader;
    public void PlayGame()
    {
        loader.LoadNextLevel();
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
