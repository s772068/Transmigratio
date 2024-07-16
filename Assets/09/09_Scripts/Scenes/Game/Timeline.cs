using System.Collections;
using UnityEngine;

public class Timeline : PersistentSingleton<Timeline> {
    [SerializeField] private ButtonsRadioGroup _buttonsGroup;
    [SerializeField] private Vector2 _timeDelayLimit;
    private float _timeDelay;
    private float _timer;
    private bool _isPlay;
    private int _tick;
    private int _windowsCount = 0;

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
                Resume();
            }
            else
                Pause();
        }
    }

    public int Tick => _tick;

    private void OnEnable()
    {
        EventPanel.EventPanelOpen += _ => WindowsCount += 1;
        EventPanel.EventPanelClose += _ => WindowsCount -= 1;
    }

    private void OnDisable()
    {
        EventPanel.EventPanelOpen -= _ => WindowsCount += 1;
        EventPanel.EventPanelClose -= _ => WindowsCount -= 1;
    }

    public void Pause() {
        Debug.Log("Pause");
        _isPlay = false;
        _buttonsGroup.Click(0);
    }

    public void Resume()
    {
        if (_timeDelay == _timeDelayLimit.y)
            Play();
        else if (_timeDelay == _timeDelayLimit.x)
            Rewind();
        else
            Pause();
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
        _buttonsGroup.Click(1);
    }

    public void Rewind() {
        if (_windowsCount > 0)
        {
            Pause();
            return;
        }

        _timeDelay = _timeDelayLimit.x;
        if (!_isPlay) StartCoroutine(TickPlay());
        Debug.Log("Rewind");
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
                GameEvents.TickLogic();
                GameEvents.TickShow();
            }
        }
    }
}
