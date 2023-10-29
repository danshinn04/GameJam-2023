using UnityEngine;

public class Button : MonoBehaviour
{
    public void Play()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
