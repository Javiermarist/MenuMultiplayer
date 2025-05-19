using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    [SerializeField] private SceneAsset scene;
   public void LoadTheScene()
    {
        SceneManager.LoadScene(scene.name);
    }
}
