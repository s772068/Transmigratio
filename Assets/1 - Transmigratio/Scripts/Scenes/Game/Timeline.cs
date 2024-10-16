using System.Collections;
using UnityEngine;
using System;
using UI;

public class Timeline : MonoBehaviour {
    [SerializeField] private ButtonsRadioGroup _buttonsGroup;
    [SerializeField] private Vector2 _timeDelayLimit;
    private float _timeDelay;
    private float _timer;
    private bool _isPlay;
    private int _tick;
    private int _windowsCount = 0;

    public static Timeline Instance { get; private set; }

    public int WindowsCount
    {
        get => _windowsCount;
        private set
        {
            _windowsCount = value;
            if (_windowsCount <= 0)
            {
                if (_windowsCount < 0)
                    _windowsCount = 0;
                MapData.IsClickableMarker = true;
                Resume();
            }
            else
            {
                MapData.IsClickableMarker = false;
                Pause();
            }
        }
    }

    public static Action TickLogic;
    public static Action TickShow;

    public int Tick => _tick;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        Panel.PanelOpen += WindowOpen;
        Panel.PanelClose +=  WindowClose;
    }

    private void OnDestroy()
    {
        Panel.PanelOpen -= WindowOpen;
        Panel.PanelClose -= WindowClose;
        Instance = null;
        StopAllCoroutines();
    }

    public void Pause() {
        _isPlay = false;
        _buttonsGroup.Select(0);
    }

    public void Stop()
    {
        _isPlay = false;
        _timeDelay = 0;
        _buttonsGroup.Select(0);
    }

    public void Resume()
    {
        if (_timeDelay == _timeDelayLimit.y)
            Play();
        else if (_timeDelay == _timeDelayLimit.x)
            Rewind();
        else
            Stop();
    }

    public void Play() {
        if (_windowsCount > 0)
        {
            Pause();
            return;
        }

        Debug.Log("Play");
        _timeDelay = _timeDelayLimit.y;
        if (!_isPlay) StartCoroutine(TickPlay());
        _buttonsGroup.Select(1);
    }

    public void Rewind() {
        if (_windowsCount > 0)
        {
            Pause();
            return;
        }

        _timeDelay = _timeDelayLimit.x;
        if (!_isPlay) StartCoroutine(TickPlay());
        _buttonsGroup.Select(2);
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

    private void WindowOpen(bool panel) => WindowsCount += 1;
    private void WindowClose(bool panel) => WindowsCount -= 1;
}
