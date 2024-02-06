using UnityEngine;

public class Layout : MonoBehaviour
{
    private Animator _animator;

    public void Close()
    {
        _animator.SetTrigger("Close");
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _animator.SetTrigger("Open");
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }
}
