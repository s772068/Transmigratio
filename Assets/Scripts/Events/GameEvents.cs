using System;

public static class GameEvents {
    public static void ClickMap(int index) => OnClickMap?.Invoke(index);
    public static Action<int> OnClickMap;
    //public static void UpdateCountry(S_Country country) => OnUpdateCountry?.Invoke(country);
    //public static Action<S_Country> OnUpdateCountry;
    //public static void Migration(S_Country from, S_Country to) => OnShowMigration?.Invoke(from, to);
    //public static Action<S_Country, S_Country> OnShowMigration;
}
