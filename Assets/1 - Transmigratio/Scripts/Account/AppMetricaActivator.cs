using Io.AppMetrica;

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
