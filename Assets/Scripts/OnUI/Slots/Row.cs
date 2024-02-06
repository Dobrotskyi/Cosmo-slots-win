using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Row : MonoBehaviour
{
    public event Action Stoped;

    [SerializeField] private RectTransform _row;
    [SerializeField] private AnimationCurve _spinningCurve;
    [SerializeField] private ParticleSystem _stopEffect;
    private AudioSource _as;
    private float _startingSpeed = 20f;
    private int _displayedSlots = 1;

    public bool IsStoped { private set; get; }
    public SlotType CurrentSlotItem => CurrentSlot.Item;
    public Slot CurrentSlot => _row.GetChild(GetClosestSlotIndex()).GetComponent<Slot>();
    private Vector2 StartingPosition => new(_row.anchoredPosition.x, -_row.rect.height / 2 + _row.GetComponent<VerticalLayoutGroup>().spacing / 2);
    private Vector2 StartingPositionNoSpacing => new(_row.anchoredPosition.x, -_row.rect.height / 2);
    private Vector2 BottomLine
    {
        get
        {
            float y = RotationToSlot(_row.childCount - _displayedSlots / 2);
            return new(_row.anchoredPosition.x, y);
        }
    }

    public SlotCombination GetVerticalCombination()
    {
        List<Slot> slots = new();
        foreach (Transform child in _row)
            if (child.gameObject.activeSelf)
                slots.Add(child.GetComponent<Slot>());

        return new(slots);
    }

    public void StartSpinning(float time)
    {
        IsStoped = false;
        foreach (Transform child in _row)
            child.gameObject.SetActive(true);
        StartCoroutine(Spin(time));
    }

    private float RotationToSlot(int index)
    {
        RectTransform slot1 = _row.GetChild(0).GetComponent<RectTransform>();
        RectTransform slot2 = _row.GetChild(1).GetComponent<RectTransform>();
        float step = Mathf.Abs(slot1.anchoredPosition.y - slot2.anchoredPosition.y);
        float slotHeight = slot1.rect.height;

        return StartingPosition.y + step * index;
    }

    private IEnumerator Spin(float spinningTime)
    {
        yield return null;

        float t = 0;
        float step = 0;

        RectTransform slot1 = _row.GetChild(0).GetComponent<RectTransform>();
        float slotHeight = slot1.rect.height;
        bool checkForRowEnd() => _row.anchoredPosition.y > BottomLine.y;

        //_row.anchoredPosition = new Vector2(_row.anchoredPosition.x, RotationToSlot(UnityEngine.Random.Range(0, _row.childCount)));

        float getEasing()
        {
            float normalizedProgress = t / spinningTime;
            return _spinningCurve.Evaluate(normalizedProgress);
        }

        while (t < spinningTime * 0.85f)
        {
            step = _startingSpeed * getEasing();
            t += Time.deltaTime;

            Vector2 newPosition = _row.anchoredPosition;
            newPosition.y += step;
            _row.anchoredPosition = newPosition;

            if (checkForRowEnd())
                _row.anchoredPosition = new(_row.anchoredPosition.x, RotationToSlot(Mathf.CeilToInt((float)_displayedSlots / 2)));

            yield return new WaitForEndOfFrame();
        }

        foreach (Transform child in _row)
            child.gameObject.SetActive(false);

        for (int i = 0; i < _displayedSlots; i++)
        {
            Transform slot = _row.GetChild(UnityEngine.Random.Range(0, _row.childCount));
            if (slot.gameObject.activeSelf)
                i--;
            else
                slot.gameObject.SetActive(true);
        }
        _row.anchoredPosition = new(_row.anchoredPosition.x, 0);

        _as.Play();
        CreateEffect(transform.position);
    }

    private void CreateEffect(Vector2 position)
    {
        ParticleSystem spawned = Instantiate(_stopEffect, position, Quaternion.identity);
        spawned.GetComponent<ParticlesCallback>().Stoped.AddListener(() =>
        {
            IsStoped = true;
            Stoped?.Invoke();
            Destroy(spawned.gameObject);
        });
    }

    private int GetClosestSlotIndex()
    {
        Transform closestChild = null;
        foreach (Transform child in _row.transform)
            if (child.gameObject.activeSelf)
            {
                if (closestChild == null)
                    closestChild = child;
                else if (Vector2.Distance(transform.position, child.position) < Vector2.Distance(transform.position, closestChild.position))
                    closestChild = child;
            }
        if (closestChild == null)
            closestChild = _row.GetChild(0);
        return closestChild.GetSiblingIndex();
    }

    private void Start()
    {
        _as = GetComponent<AudioSource>();
        StartCoroutine(Init());
        _displayedSlots = FindObjectOfType<SlotMachine>().VisibleSlots;
    }

    private IEnumerator Init()
    {
        yield return 0;
        SpawnSlotsInRow();
        yield return 0;
        _row.anchoredPosition = new(_row.anchoredPosition.x, RotationToSlot(UnityEngine.Random.Range(_displayedSlots / 2, _row.childCount - 1)));
    }

    private void SpawnSlotsInRow()
    {
        int rowsMerged = FindObjectOfType<SlotMachine>().VisibleSlots;
        if (rowsMerged == 1)
            return;

        List<List<Transform>> rowsToMerge = new();

        List<Transform> currentSlots = new();
        foreach (Transform child in _row.transform)
            currentSlots.Add(child);

        for (int i = 0; i < rowsMerged - 1; i++)
            rowsToMerge.Add(currentSlots.ToList());

        while (rowsToMerge.Count > 0)
        {
            var randomRow = rowsToMerge[UnityEngine.Random.Range(0, rowsToMerge.Count)];
            var randomSlot = randomRow[UnityEngine.Random.Range(0, randomRow.Count)];

            Transform spawned = Instantiate(randomSlot, _row);
            spawned.transform.SetSiblingIndex(UnityEngine.Random.Range(0, _row.childCount));

            randomRow.Remove(randomSlot);
            if (randomRow.Count == 0)
                rowsToMerge.Remove(randomRow);
        }
    }
}
