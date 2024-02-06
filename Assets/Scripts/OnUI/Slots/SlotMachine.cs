using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlotMachine : MonoBehaviour
{
    public static event Action HandlePulled;
    public static event Action FirstRowStoped;
    public static event Action RoundEnded;

    [SerializeField] private List<Row> _rows = new();
    [SerializeField] private List<WinningCombination> _combinations = new();
    [SerializeField] private Button _handle;
    [SerializeField] private int _visibleSlots = 1;
    [SerializeField] private AudioSource _spinningAS;
    [SerializeField] private AudioSource _roundEndedAS;
    [SerializeField] private List<AudioClip> _roundEndClips;
    [SerializeField] private ParticleSystem _winningEffect;
    //private BettingField _bettingField;

    public float SpinningTime { private set; get; } = 3f;
    public float TimeStep { private set; get; } = 2f;
    public int Bet { private set; get; }
    public int VisibleSlots => _visibleSlots;
    public IList<WinningCombination> Combinations => _combinations.AsReadOnly();
    private bool IsRoundEnded => _rows.Count(r => r.IsStoped) == _rows.Count;

    public void LaunchMachine()
    {
        _winningEffect.Stop();
        _roundEndedAS.Stop();
        _spinningAS.Play();
        //Bet = _bettingField.Value;
        Bet = 100;
        //PlayerInfoHolder.WithdrawCoins(Bet);

        HandlePulled?.Invoke();

        for (int i = 0; i < _rows.Count; i++)
            _rows[i].StartSpinning(SpinningTime + TimeStep * i);
    }

    private void OnEnable()
    {
        //PlayerInfoHolder.TryNotEnoughCoinsInvoke();

        //_bettingField = FindObjectOfType<BettingField>(true);

        foreach (var row in _rows)
            row.Stoped += RowStoped;

        _handle.onClick.AddListener(DisableButton);
        RoundEnded += EnableButton;
    }

    private void OnDisable()
    {
        foreach (var row in _rows)
            row.Stoped -= RowStoped;

        _handle.onClick.RemoveListener(DisableButton);
        RoundEnded -= EnableButton;
    }

    private void EnableButton()
    {
        //if (PlayerInfoHolder.PlayerCoins <= PlayerInfoHolder.MIN_PLAYER_COINS)
        //    return;
        SetButtonInteractable(true);
    }

    private void DisableButton() => SetButtonInteractable(false);

    private void SetButtonInteractable(bool value) => _handle.interactable = value;

    private void RowStoped()
    {
        if (_rows.Count(r => r.IsStoped) == 1)
            FirstRowStoped?.Invoke();
        if (!IsRoundEnded) return;

        _spinningAS.Stop();

        List<SlotCombination> currentCombinations = new();
        if (VisibleSlots == 1)
            currentCombinations.Add(new(_rows.Select(r => r.CurrentSlot)));

        else
        {
            for (int i = 0; i < _rows.Count; i++)
                currentCombinations.Add(_rows[i].GetVerticalCombination());

            for (int i = 0; i < _rows.Count; i++)
            {
                Slot[] itemsInHorizontal = new Slot[_rows.Count];
                for (int j = 0; j < itemsInHorizontal.Length; j++)
                    itemsInHorizontal[j] = currentCombinations[j].Slots[i];

                currentCombinations.Add(new(itemsInHorizontal));
            }
        }

        float multipliers = 0;
        int x2Count = 0;
        foreach (var combination in currentCombinations)
        {
            List<WinningCombination> matches = FindWinningCombinationIn(combination).ToList();
            if (matches.Count > 0)
                multipliers += matches.Sum(m => m.Multiplier);
        }

        multipliers *= x2Count > 0 ? (int)Mathf.Pow(2, x2Count) : 1;

        if (multipliers != 0)
        {
            int winning = (int)(Bet * multipliers);
            PlayWonSound();
            _winningEffect.Play();
            //PlayerInfoHolder.AddCoins(winning);
        }
        else
        {
            PlayLostSound();
        }

        RoundEnded?.Invoke();
        //PlayerInfoHolder.TryNotEnoughCoinsInvoke();
    }

    private void PlayLostSound()
    {
        _roundEndedAS.clip = _roundEndClips[1];
        _roundEndedAS.Play();
    }

    private void PlayWonSound()
    {
        _roundEndedAS.clip = _roundEndClips[0];
        _roundEndedAS.Play();
    }

    private IEnumerable<WinningCombination> FindWinningCombinationIn(SlotCombination combination)
    {
        List<WinningCombination> matchingCombinations = new();

        for (int i = 0; i < _combinations.Count; i++)
            if (combination.SlotsItems.ContainsSequence(_combinations[i].Items))
                matchingCombinations.Add(_combinations[i]);

        if (matchingCombinations.Count == 0)
            return matchingCombinations;

        for (int i = 0; i < matchingCombinations.Count; i++)
            for (int j = 0; j < matchingCombinations.Count; j++)
                if (matchingCombinations[i].Items.Count > matchingCombinations[j].Items.Count)
                    if (matchingCombinations[i].Items.ContainsSequence(matchingCombinations[j].Items))
                        matchingCombinations.RemoveAt(j);

        return matchingCombinations;
    }
}
