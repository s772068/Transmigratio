using UnityEngine;

namespace Account
{
    public class Player : MonoBehaviour
    {
        private static Player _inst;
        private IServiceLocator _locator;

        [SerializeField] private string _appmetricaKey;
        private AppMetricaActivator _appmetrica;

        private FirebaseAnalytics _analytics;


        private void Awake()
        {
            if (_inst == null)
                _inst = GetComponent<Player>();
            else
                Destroy(gameObject);

            _locator = new ServiceLocator();
            _analytics = _locator.GetService<FirebaseAnalytics>();

            _appmetrica = new();
            _appmetrica.Activate(_appmetricaKey);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
#if Anroid
        _analytics.FirebaseAnalyticsInitialize();
#endif
        }
    }
}
