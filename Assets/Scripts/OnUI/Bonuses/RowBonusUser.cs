public class RowBonusUser : BonusUser
{
    private SlotsGame _slots;
    public override Bonus Type => Bonus.Row;

    private void Awake()
    {
        _slots = FindObjectOfType<SlotsGame>();
    }

    protected override void Apply()
    {
        foreach (var slot in _slots.GetHorizontalRow(transform.GetSiblingIndex()).Slots)
            slot.FadeFog();
    }
}
