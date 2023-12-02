using UnityEngine;
using Flexalon;

public static class GameObjectHelper {
    public static void SetActiveFlexalon(this GameObject owner, bool isActive) {
        owner.GetComponent<FlexalonObject>().SkipLayout = !isActive;
        owner.SetActive(isActive);
    }
}
