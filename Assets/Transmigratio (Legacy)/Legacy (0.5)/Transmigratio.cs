using UnityEngine;
using System.IO;
using UnityEngine.UI;
using WorldMapStrategyKit;
//using static UnityEditor.PlayerSettings;
//using UnityEditor.Experimental.GraphView;

public class Transmigratio : MonoBehaviour
{
    //Transmigratio transmigratio;
    //public static Transmigratio transmigratio;
    public static Transmigratio Instance;

    //public Map map;
    //public Humanity humanity = new Humanity();


    public TMDB tmdb;                       // база данных ScriptableObjects
    
    public PopUp popUp;                     //управление всплывающими окнами
    

    [Header("UI objects")]
    public Text populationLabel;            // отображение суммарного населения
    public Text populationValue;
    public Text timeLabel;                  // отображение времени
    public Text timeValue;

    public void Start()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this) 
        {
            Destroy(gameObject);
        }
        //tmdb = new TMDB();
        tmdb.GameInitialization();

        tmdb.map.wmsk.OnCountryClick += OnClickFromMain;
    }
    private void OnClickFromMain(int countryIndex, int regionIndex, int buttonIndex)
    {
        popUp.ShowRegionPanel(tmdb.map.activeRegion);
    }
    /*
    public void MakePathOfMigration()
    {
        GameObject pathOfMigrationObject = Instantiate(pathPrefab);
        pathOfMigrationObject.name = "path";
        pathOfMigrationObject.GetComponentInChildren<Button>().onClick.AddListener(popUp.ShowPathOfMigrationPanel);
        //Destroy(GameObject.Find("pathline"));
        map.DrawPathOfMigration(map.activeRegion, map.RandomPopulatedNeighbour(map.activeRegion), pathOfMigrationObject);
        MarkerClickHandler handler = pathOfMigrationObject.GetComponent<MarkerClickHandler>();
        handler.allowDrag = false;
        handler.OnMarkerMouseDown += (go, buttonIndex) => popUp.ShowPathOfMigrationPanel();

        
    }
    
    public void MakeCow()
    {
        GameObject cow = Instantiate(cowPrefab);
        cow.name = "COW";
        Vector3 pos = Vector3.zero;
        pos.x = 0.19f;
        pos.y = 0.26f;
        map.wmsk.AddMarker2DSprite(cow, pos, 0.02f, enableEvents: true, autoScale: false);
        MarkerClickHandler handler = cow.GetComponent<MarkerClickHandler>();
        handler.allowDrag = false;
        handler.OnMarkerMouseDown += (go, buttonIndex) => popUp.ShowCow();
    }
    */

    /*
    public void TurnResultsToFile(string jsonString)
    {
        //File.WriteAllText(Application.streamingAssetsPath + "/Config/turn" + currentTurn.ToString() + ".json", jsonString);
    }
    */
}

