using UnityEngine;

namespace RegionDetails.Defoult.Descriptions {
    public class BaseDescription : MonoBehaviour {
        public void Destroy() => Destroy(gameObject);
    }
}
