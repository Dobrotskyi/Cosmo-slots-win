using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SlotsRow : MonoBehaviour
{
    public event Action Stoped;

    [SerializeField] private RectTransform _body;
    [SerializeField] private AnimationCurve _spinningCurve;
    [SerializeField] private ParticleSystem _stopEffect;
    private AudioSource _as;
    private float _startingSpeed = 50f;
    private int _displayedSlots = 1;
    private int _multiplySlotsBy = 3;

    public bool IsStoped { private set; get; }
    private Vector2 StartingPosition => new(_body.anchoredPosition.x, -_body.rect.height / 2 + _body.GetComponent<VerticalLayoutGroup>().spacing / 2);
    private Vector2 BottomLine
    {
        get
        {
            float y = RotationToSlot(_body.childCount - _displayedSlots / 2);
            return new(_body.anchoredPosition.x, y);
        }
    }

    public Slot GetActiveSlotByIndex(int index) => _body.GetActiveChildren().ElementAt(index).GetComponent<Slot>();

    public void FadeFog()
    {
        for (int i = 0; i < _body.childCount; i++)
            _body.GetChild(i).GetComponent<Slot>().FadeFog();
    }

    public SlotSequence GetVerticalCombination()
    {
        List<Slot> slots = new();
        foreach (Transform child in _body)
            if (child.gameObject.activeSelf)
                slots.Add(child.GetComponent<Slot>());

        return new(slots);
    }

    public void StartSpinning(float time)
    {
        IsStoped = false;
        foreach (Transform child in _body)
            child.gameObject.SetActive(true);
        StartCoroutine(Spin(time));
    }

    private float RotationToSlot(int index)
    {
        RectTransform slot1 = _body.GetChild(0).GetComponent<RectTransform>();
        RectTransform slot2 = _body.GetChild(1).GetComponent<RectTransform>();
        float step = Mathf.Abs(slot1.anchoredPosition.y - slot2.anchoredPosition.y);
        float slotHeight = slot1.rect.height;

        return StartingPosition.y + step * index;
    }

    private IEnumerator Spin(float spinningTime)
    {
        yield return null;

        float t = 0;
        float step = 0;

        RectTransform slot1 = _body.GetChild(0).GetComponent<RectTransform>();
        float slotHeight = slot1.rect.height;
        bool checkForRowEnd() => _body.anchoredPosition.y > BottomLine.y;

        float getEasing()
        {
            float normalizedProgress = t / spinningTime;
            return _spinningCurve.Evaluate(normalizedProgress);
        }

        while (t < spinningTime * 0.85f)
        {
            step = _startingSpeed * getEasing();
            t += Time.deltaTime;

            Vector2 newPosition = _body.anchoredPosition;
            newPosition.y += step;
            _body.anchoredPosition = newPosition;

            if (checkForRowEnd())
                _body.anchoredPosition = new(_body.anchoredPosition.x, RotationToSlot(Mathf.CeilToInt((float)_displayedSlots / 2)));

            yield return new WaitForEndOfFrame();
        }

        foreach (Transform child in _body)
            child.gameObject.SetActive(false);

        for (int i = 0; i < _displayedSlots; i++)
        {
            Transform slot = _body.GetChild(UnityEngine.Random.Range(0, _body.childCount));
            if (slot.gameObject.activeSelf)
                i--;
            else
                slot.gameObject.SetActive(true);
        }
        _body.anchoredPosition = new(_body.anchoredPosition.x, 0);

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

    private void Start()
    {
        _as = GetComponent<AudioSource>();
        StartCoroutine(Initialize());
        _displayedSlots = FindObjectOfType<SlotsGame>().VisibleSlots;
    }

    private IEnumerator Initialize()
    {
        yield return 0;
        SpawnSlotsInRow();
        yield return 0;
        _body.anchoredPosition = new(_body.anchoredPosition.x, RotationToSlot(UnityEngine.Random.Range(_displayedSlots / 2, _body.childCount - (Mathf.CeilToInt((float)_displayedSlots / 2)))));
    }

    private void SpawnSlotsInRow()
    {
        if (_multiplySlotsBy == 1)
            return;

        List<List<Transform>> rowsToMerge = new();

        List<Transform> currentSlots = new();
        foreach (Transform child in _body.transform)
            currentSlots.Add(child);

        for (int i = 0; i < _multiplySlotsBy; i++)
            rowsToMerge.Add(currentSlots.ToList());

        while (rowsToMerge.Count > 0)
        {
            var randomRow = rowsToMerge[UnityEngine.Random.Range(0, rowsToMerge.Count)];
            var randomSlot = randomRow[UnityEngine.Random.Range(0, randomRow.Count)];

            Transform spawned = Instantiate(randomSlot, _body);
            spawned.transform.SetSiblingIndex(UnityEngine.Random.Range(0, _body.childCount));

            randomRow.Remove(randomSlot);
            if (randomRow.Count == 0)
                rowsToMerge.Remove(randomRow);
        }
    }

#if UNITY_EDITOR
    public bool TestFog;
    private void Update()
    {
        if (TestFog)
        {
            TestFog = false;
            FadeFog();
        }
    }
#endif
}
