using UnityEngine;

public class Tutor : MonoBehaviour
{
    private static bool s_shown;

    public void Close()
    {
        s_shown = true;
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        if (s_shown)
            Destroy(gameObject);
    }
}
