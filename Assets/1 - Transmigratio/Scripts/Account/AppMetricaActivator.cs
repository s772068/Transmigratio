using Io.AppMetrica;

namespace Account
{
    public class AppMetricaActivator
    {
        public void Activate(string key)
        {
            AppMetrica.Activate(new AppMetricaConfig(key)
            {
                FirstActivationAsUpdate = true,
                Logs = true,

            });
        }
    }
}
