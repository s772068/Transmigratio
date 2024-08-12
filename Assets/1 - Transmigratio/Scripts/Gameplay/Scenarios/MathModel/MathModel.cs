using UnityEngine;

namespace Gameplay.Scenarios {
    [CreateAssetMenu(menuName = "Gameplay/MathModel", fileName = "MathModel")]
    public class MathModel : Base {
        [SerializeField] private EcoCulture.Data ecoCulture = new();
        [SerializeField] private ProdMode.Data prodMode = new();
        [SerializeField] private Government.Data government = new();
        [SerializeField] private Demography.Data demography = new();

        public override void Init() {
            EcoCulture.data = ecoCulture;
            ProdMode.data = prodMode;
            Government.data = government;
            Demography.data = demography;
        }

        public override void Play() {
            EcoCulture.Play(_piece);
            ProdMode.Play(_piece);
            Government.Play(_piece);
            Demography.Play(_piece);
        }
    }
}
