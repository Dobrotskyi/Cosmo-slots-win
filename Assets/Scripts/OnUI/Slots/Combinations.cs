using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SlotSequence
{
    [SerializeField] private List<Slot> _slots = new();

    public List<Slot> Slots => _slots;

    public SlotSequence(IEnumerable<Slot> slots)
    {
        _slots = slots.ToList();
    }
}
