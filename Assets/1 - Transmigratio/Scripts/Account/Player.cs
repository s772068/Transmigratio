using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private string _appmetricaKey;
    private Player _inst;
    private AppMetricaActivator _appmetrica;


    private void Awake()
    {
        if (_inst == null)
            _inst = GetComponent<Player>();
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        _appmetrica = new();
        _appmetrica.Activate(_appmetricaKey);
    }
}
