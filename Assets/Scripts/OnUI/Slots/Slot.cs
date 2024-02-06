using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField] private SlotType _item;
    public SlotType Item => _item;
}
