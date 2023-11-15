using UnityEngine;
/// <summary>
/// В игроке будут храниться данные о ресурсах игрока - очки вмешательства, экологии итд, а также функции для действий игрока
/// </summary>
[System.Serializable]
public class Player
{
    public Eco eco = new Eco();
    public Purity purity = new Purity();


    public void Init()
    {
        eco.Init();
        purity.Init();
    }
}
