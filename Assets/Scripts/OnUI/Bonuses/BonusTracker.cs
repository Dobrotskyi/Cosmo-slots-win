using System;
using System.Collections.Generic;
using UnityEngine;

public static class BonusTracker
{
    public static event Action BonusAmtChanged;

    public static readonly Dictionary<Bonus, int> PriceList = new()
    {
        {Bonus.Cell, 300 },
        {Bonus.Column, 400 },
        {Bonus.Row, 550 }
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
