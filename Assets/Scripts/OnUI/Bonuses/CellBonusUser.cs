using UnityEngine;

public class CellBonusUser : BonusUser
{
    [SerializeField] private Slot _slot;
    public override Bonus Type => Bonus.Cell;

    protected override void Apply()
    {
        _slot.FadeFog();
    }
}
