using UnityEngine;
public class ColumnBonusUser : BonusUser
{
    [SerializeField] private Row _represented;
    public override Bonus Type => Bonus.Column;

    protected override void Apply()
    {
        var column = _represented.GetVerticalCombination();
        foreach (var slot in column.Slots)
            slot.FadeFog();
    }
}
