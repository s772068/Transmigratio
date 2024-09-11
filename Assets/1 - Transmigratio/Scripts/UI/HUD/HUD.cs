using UnityEngine;

/// <summary>
/// Интерфейс, всплывающие окна и тд
/// </summary>
public class HUD : StaticInstance<HUD> {
    [SerializeField] private Messanger _messanger;
    [SerializeField] private Transform _panelsParent;

    [SerializeField] private Migration _migration;

    public bool IsShowMigration = true;

    public Transform PanelsParent => _panelsParent;

    protected override void Awake() {
        base.Awake();
    }

    private void OnShowMessage(string message) {
        _messanger.gameObject.SetActive(true);
        _messanger.Message = message;
    }

    public void ShowMigration() {
        if (!IsShowMigration) return;
        //migration
    }
}
