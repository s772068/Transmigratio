/// <summary>
/// Класс для расчётов населения. 
/// Население области, цивилизации или всего мира - переменные типа Population
/// </summary>
[System.Serializable]
public class Population
{
    public int value;
    public int Value
    {
        get { return value; }
        set { if (value < 0) value = 0; }
    }
}
