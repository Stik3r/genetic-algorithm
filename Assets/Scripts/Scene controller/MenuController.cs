using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadModelling()
    {
        SceneManager.LoadScene("SelectMap");
    }

    public void ExitApplication()
    {
        Application.Quit(1);
    }
}
