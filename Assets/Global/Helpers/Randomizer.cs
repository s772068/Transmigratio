using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Randomizer {
    public static int Random(int max) => (int) (Time.realtimeSinceStartupAsDouble * 100000 % 1000) % max;
}
