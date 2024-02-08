using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlotsGame : MonoBehaviour
{
    public static event Action RoundStarted;
    public static event Action RoundEnded;
    public static event Action LastRowStoped;
    public static event Action Revealed;
    public static event Action<int> PlayerWon;

    [SerializeField] private List<SlotsRow> _rows = new();
    [SerializeField] private Button _handle;
    [SerializeField] private int _visibleSlots = 1;
    [SerializeField] private AudioSource _spinningAS;
    [SerializeField] private AudioSource _roundEndedAS;
    [SerializeField] private List<AudioClip> _roundEndClips;
    [SerializeField] private ParticleSystem _winningEffect;
    [SerializeField] private ParticleSystem _fogFadingEffect;
    private Bet _betting;

    public float SpinningTime { private set; get; } = 3f;
    public float TimeStep { private set; get; } = 2f;
    public int BetAmt { private set; get; }
    public int VisibleSlots => _visibleSlots;
    private bool AllRowsStoped => _rows.Count(r => r.IsStoped) == _rows.Count;

    public SlotSequence GetHorizontalRow(int index)
    {
        var slots = _rows.Select(r => r.GetActiveSlotByIndex(index)).ToArray();
        return new(slots);
    }

    public void Unlock()
    {
        StartCoroutine(GetResults());
    }

    private IEnumerator GetResults()
    {
        HashSet<Slot> results = new();

        switch (DifficultyLevel.Level)
        {
            case Levels.First:
                {
                    results = GetCombinationsFirstLevel();
                    break;
                }
            case Levels.Second:
                {
                    results = GetCombinationsSecondLevel();
                    break;
                }
            case Levels.Third:
                {
                    results = GetCombinationsThirdLevel();
                    break;
                }
        }

        for (int i = 0; i < VisibleSlots; i++)
            foreach (var item in GetHorizontalRow(i).Slots)
                item.FadeFog(false);

        _fogFadingEffect.Play();
        foreach (var item in results)
            item.InCombination();
        Revealed?.Invoke();
        float multipliers = 0;
        if (results.Count != 0)
            multipliers = results.Sum(s => s.Multiplier);

        yield return new WaitForSeconds(_fogFadingEffect.main.duration * 3);
        if (multipliers != 0)
        {
            int winning = (int)(BetAmt * multipliers);
            PLosingSound();
            _winningEffect.Play();
            PlayerWon?.Invoke(winning);
            PlayerCoins.AddCoins(winning);
        }
        else
        {
            WinningSound();
        }

        RoundEnded?.Invoke();
        PlayerCoins.InvokeIfNotEnough();
    }

    private HashSet<Slot> GetCombinationsFirstLevel()
    {
        List<SlotSequence> verticalCombinations = _rows.Select(r => r.GetVerticalCombination()).ToList();
        SlotSequence start = verticalCombinations[0];

        List<Slot> foundSlots = new();

        foreach (var slot in start.Slots.ToList())
        {
            for (int j = 1; j < verticalCombinations.Count; j++)
            {
                if (j == 2)
                {
                    var foundIn0Row = verticalCombinations[0].Slots.Where(s => s.Item == slot.Item);
                    if (foundIn0Row.Count() > 0)
                        foundSlots.AddRange(foundIn0Row);
                }

                var found = verticalCombinations[j].Slots.Where(s => s.Item == slot.Item);
                if (found.Count() > 0)
                    foundSlots.AddRange(found);
                else
                    break;
            }

            Debug.Log($"Found {foundSlots.Count(s => s.Item == slot.Item)} of type {slot.Item}");
        }

        Debug.Log($"Summed of {new HashSet<Slot>(foundSlots).Count} elements");

        return new HashSet<Slot>(foundSlots);
    }
    private HashSet<Slot> GetCombinationsSecondLevel() => GetCombination(false, true);
    private HashSet<Slot> GetCombinationsThirdLevel() => GetCombination(false, false);

    private HashSet<Slot> GetCombination(bool includeVert, bool includeDiag)
    {
        List<Slot> foundSlots = new();
        LinkedList<SlotSequence> verticalCombs = new(_rows.Select(r => r.GetVerticalCombination()));

        SlotSequence first = verticalCombs.First.Value;
        for (int i = 0; i < first.Slots.Count; i++)
        {
            var neighbours = GetNeighbours(verticalCombs.First, i, includeVert, includeDiag);
            if (neighbours.Count > 1)
            {
                foundSlots.AddRange(neighbours);
                if (i - 1 >= 0)
                    if (first.Slots[i].Item == first.Slots[i - 1].Item)
                        foundSlots.Add(first.Slots[i - 1]);

                if (i + 1 < first.Slots.Count)
                    if (first.Slots[i].Item == first.Slots[i + 1].Item)
                        foundSlots.Add(first.Slots[i + 1]);
            }
        }

        HashSet<Slot> result = new(foundSlots);
#if UNITY_EDITOR
        Debug.Log($"Summed of {result.Count} elements");
        foreach (var element in result)
            Debug.Log(element.Item);
#endif

        return result;
    }

    private List<Slot> GetNeighbours(LinkedListNode<SlotSequence> node, int index, bool includeVert = true, bool includeDiag = true)
    {
        Slot slot = node.Value.Slots[index];
        List<Slot> result = new();
        result.Add(slot);
        if (slot.Visited)
            return result;
        slot.Visited = true;

        if (includeDiag)
        {
            if (index - 1 >= 0 && node.Next != null)
            {
                if (node.Next.Value.Slots[index - 1].Item == slot.Item)
                    result.AddRange(GetNeighbours(node.Next, index - 1, true, includeDiag));
            }

            if (index + 1 < node.Value.Slots.Count && node.Next != null)
            {
                if (node.Next.Value.Slots[index + 1].Item == slot.Item)
                    result.AddRange(GetNeighbours(node.Next, index + 1, true, includeDiag));
            }
        }

        if (includeVert)
        {
            if (index - 1 >= 0)
                if (node.Value.Slots[index - 1].Item == slot.Item)
                    result.AddRange(GetNeighbours(node, index - 1, true, includeDiag));

            if (index + 1 < node.Value.Slots.Count)
                if (node.Value.Slots[index + 1].Item == slot.Item)
                    result.AddRange(GetNeighbours(node, index + 1, true, includeDiag));
        }

        if (node.Next != null && node.Next.Value.Slots[index].Item == slot.Item)
            result.AddRange(GetNeighbours(node.Next, index, true, includeDiag));

        return result;
    }

    public void LaunchMachine()
    {
        StopAllCoroutines();

        _winningEffect.Stop();
        _roundEndedAS.Stop();
        _spinningAS.Play();
        BetAmt = _betting.Amount;

        PlayerCoins.WithdrawCoins(BetAmt);

        RoundStarted?.Invoke();

        for (int i = 0; i < _rows.Count; i++)
            _rows[i].StartSpinning(SpinningTime + TimeStep * i);
    }

    private void OnEnable()
    {
        PlayerCoins.InvokeIfNotEnough();

        _betting = FindObjectOfType<Bet>(true);

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
        if (PlayerCoins.Amount < Bet.MIN_BET)
            return;
        SetButtonInteractable(true);
    }

    private void DisableButton() => SetButtonInteractable(false);

    private void SetButtonInteractable(bool value) => _handle.interactable = value;

    private void RowStoped()
    {
        if (!AllRowsStoped) return;

        LastRowStoped?.Invoke();
        _spinningAS.Stop();
    }

    private void WinningSound()
    {
        _roundEndedAS.clip = _roundEndClips[1];
        _roundEndedAS.Play();
    }

    private void PLosingSound()
    {
        _roundEndedAS.clip = _roundEndClips[0];
        _roundEndedAS.Play();
    }
}
