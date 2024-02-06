using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    private static string ToLoad { set; get; } = "MainMenu";

    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _textValueField;

    public static void LoadSceneWithName(string name)
    {
        ToLoad = name;
        SceneManager.LoadScene("Loading");
    }

    public static void ReloadCurrent()
    {
        LoadSceneWithName(SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
        StartCoroutine(LoadLevelAsync());
    }

    private IEnumerator LoadLevelAsync()
    {
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(ToLoad);

        while (!loadingOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadingOperation.progress / 0.9f);
            if (progress < 0.1)
                progress = 0.1f;
            _slider.value = progress;
            _textValueField.text = "Loading... " + ((int)(progress * 100f)).ToString() + "%";
            yield return null;
        }
    }
}