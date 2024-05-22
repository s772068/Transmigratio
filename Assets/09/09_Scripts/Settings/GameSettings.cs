using System.Collections.Generic;

public static class GameSettings {
    public static int startPopulation = 1000;
    public static int yearsByTick = 10;

    public static List<string> civDetails = new() {
        "government",
        "ecoCulture",
        "prodMode",
        "civilizations"
    };

    public static List<string> regionDetails = new() {
        "climate",
        "terrain",
        "flora",
        "fauna"
    };
}
