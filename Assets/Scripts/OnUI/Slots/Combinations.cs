using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SlotCombination
{
    [SerializeField] private List<Slot> _slots = new();
    [SerializeField] private float _multiplier = 1;
    private List<SlotType> _slotsItems = new();

    public IReadOnlyList<Slot> Slots => _slots;
    public IReadOnlyList<SlotType> SlotsItems
    {
        get
        {
            if (_slotsItems == null || _slotsItems.Count == 0)
                _slotsItems = _slots.Select(s => s.Item).ToList();
            return _slotsItems;
        }
    }
    public float Multiplier => _multiplier;

    public SlotCombination(IEnumerable<Slot> slots)
    {
        _slots = slots.ToList();
        _slotsItems = _slots.Select(s => s.Item).ToList();
    }
}

[Serializable]
public class WinningCombination
{
    [SerializeField] private float _multiplier = 1;
    [SerializeField] private List<SlotType> _slotsItems = new();
    public IReadOnlyList<SlotType> Items => _slotsItems;
    public float Multiplier => _multiplier;
}
