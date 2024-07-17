using System.Collections;
using UnityEngine;
using System;

public class Timeline : PersistentSingleton<Timeline> {
    [SerializeField] private ButtonsRadioGroup _buttonsGroup;
    [SerializeField] private Vector2 _timeDelayLimit;
    private float _timeDelay;
    private float _timer;
    private bool _isPlay;
    private int _tick;

    public static Action TickLogic;
    public static Action TickShow;

    public int Tick => _tick;

    public void Pause() {
        _isPlay = false;
        _buttonsGroup.Click(0);
    }

    public void Play() {
        _timeDelay = _timeDelayLimit.y;
        if (!_isPlay) StartCoroutine(TickPlay());
        _buttonsGroup.Click(1);
    }

    public void Rewind() {
        _timeDelay = _timeDelayLimit.x;
        if (!_isPlay) StartCoroutine(TickPlay());
        _buttonsGroup.Click(2);
    }

    private IEnumerator TickPlay() {
        _isPlay = true;
        while (_isPlay) {
            yield return new WaitForFixedUpdate();
            _timer += Time.fixedDeltaTime;
            if(_timer >= _timeDelay) {
                _timer = 0;
                ++_tick;
                TickLogic?.Invoke();
                TickShow?.Invoke();
            }
        }
    }
}
