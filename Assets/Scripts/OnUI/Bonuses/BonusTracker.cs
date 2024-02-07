using System;
using System.Collections.Generic;
using UnityEngine;

public static class BonusTracker
{
    public static event Action BonusAmtChanged;

    public static readonly Dictionary<Bonus, int> PriceList = new()
    {
        {Bonus.Cell, 0 },
        {Bonus.Column, 0 },
        {Bonus.Row, 0 }
    };

    public static int GetAmount(Bonus bonus) => PlayerPrefs.GetInt(bonus.ToString(), 0);

    public static void UseBonus(Bonus bonus)
    {
        PlayerPrefs.SetInt(bonus.ToString(), GetAmount(bonus) - 1);
        BonusAmtChanged?.Invoke();
    }

    public static void BuyBonus(Bonus bonus)
    {
        PlayerPrefs.SetInt(bonus.ToString(), GetAmount(bonus) + 1);
        BonusAmtChanged?.Invoke();
    }
}
