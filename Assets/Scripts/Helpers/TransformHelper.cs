using UnityEngine;
using Flexalon;

public static class TransformHelper {
    public static void SetActiveFlexalon(this Transform owner, bool isActive) {
        owner.GetComponent<FlexalonObject>().SkipLayout = !isActive;
        owner.gameObject.SetActive(isActive);
    }
}
