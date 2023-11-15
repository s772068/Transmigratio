using UnityEngine;
[System.Serializable]
public class PlayerResource
{
    public int quantity;

    public void Init()
    {
        Quantity = 100;
    }
    public void ChangeQuantity(int x)
    {
        Quantity += x;
    }
    public int Quantity
    {
        get { return quantity; }
        set 
        {
            if (value < 0) quantity = 0;
            else if (value > 100) quantity = 100;
            else quantity = value;
        }
    }
}