using UnityEngine;

public class LoadLevel : MonoBehaviour
{
    public void Load(string name)
    {
        Loader.LoadSceneWithName(name);
    }

    public void Reload()
    {
        Loader.ReloadCurrent();
    }
}
