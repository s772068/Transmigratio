using Io.AppMetrica;

public class AppMetricaActivator
{
    public void Activate(string key)
    {
        AppMetrica.Activate(new AppMetricaConfig(key)
        {
            FirstActivationAsUpdate = !IsFirstLaunch(),
            Logs = true,

        });
    }

    private bool IsFirstLaunch()
    {
        // Implement logic to detect whether the app is opening for the first time.
        // For example, you can check for files (settings, databases, and so on),
        // which the app creates on its first launch.
        return true;
    }
}
