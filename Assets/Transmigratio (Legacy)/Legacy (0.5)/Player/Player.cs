using UnityEngine;
/// <summary>
/// � ������ ����� ��������� ������ � �������� ������ - ���� �������������, �������� ���, � ����� ������� ��� �������� ������
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
