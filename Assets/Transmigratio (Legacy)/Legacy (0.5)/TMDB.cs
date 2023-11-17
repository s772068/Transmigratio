using Mono.Cecil;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TMDB", menuName = "TMDB", order = 1)]
public class TMDB : ScriptableObject
{

    
    //public TM_Localization locale;

    public Map map = new Map();
    public Humanity humanity = new Humanity();

    public PieColor colorsForDecorators = new PieColor();

    public void GameInitialization()
    {
        humanity.Init();
        map.Init();
        colorsForDecorators.Init2();
    }

    public string SaveGame()
    {
        string jsonString = "";
        return jsonString;
    }
    public void NextTurn()
    {
        //TurnResultsToFile(map.MakeJSONOfTurn());
        map.CalcGlobalVars();
        humanity.CalcGlobalVars();
    }

    /*
    public void TurnResultsToFile(string jsonString)
    {
        File.WriteAllText(Application.streamingAssetsPath + "/Config/turn" + currentTurn.ToString() + ".json", jsonString);
    }
    */
}