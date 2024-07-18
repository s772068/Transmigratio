using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void OpenScene(int index) => SceneManager.LoadScene(index);
}
