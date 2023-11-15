using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

using Newtonsoft.Json;
using System.Xml.Schema;
/// <summary>
/// ������� ������ � ������������ ������
/// </summary>
public class PopUp : MonoBehaviour
{

    [Header("���������� � ��������� �������")]
    public RectTransform panelInfo;
    public Text countryName;

    [Header("�������")]
    public Text terrainLabel;
    public Text terainCurrent;
    public Button terrainButton;
    
    public Text climateLabel;
    public Text climateCurrent;
    public Button climateButton;

    public Text floraLabel;
    public Text floraCurrent;
    public Slider floraSlider;
    public Button floraButton;

    public Text faunaLabel;
    public Text faunaCurrent;
    public Slider faunaSlider;
    public Button faunaButton;

    [Header("���������")]
    public Text populationLabel;
    public Text populationCurrent;
    public Button populationButton;

    public Text prodModeLabel;
    public Text prodModeCurrent;
    public Button prodModeButton;

    public Text ecoLabel;
    public Text ecoCurrent;
    public Button ecoButton;

    public Text govLabel;
    public Text govCurrent;
    public Button govButton;

    public Text civLabel;
    public Text civCurrent;
    public Button civButton;

    public Button hidePanelInfo; //������ ��������

    [Header("���������� � ��������� ���������")]
    public RectTransform paramDetails;
    public Text paramDetailsTitle;
    public Text richnessTitle;

    public GameObject radialPrefab;
    public GameObject specListPrefab;
    //public Transform pieParent;
    public GameObject pieParent;
    public Transform specListParent;
    public PieColor pieColor = new PieColor();
    public List<GameObject> pieObjects;
    public List<GameObject> specObjects;

    
    [Header("For the showcases")]
    public RectTransform pathPanel;
    public RectTransform cowPanel;
    public Dropdown colorChanger;

    public void ShowRegionPanel(TM_Region region)
    {
        HidePanel(paramDetails);
        
        //for the showcases
        PieColor pc = new PieColor();
        pc.Init2();
        //nameof(pc.colors[0])
        //colorChanger.AddOptions(pc.);
        
        panelInfo.gameObject.SetActive(true);
        countryName.text = region.name2;
        
        terainCurrent.text = region.terrain.currentMax;
        terrainButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.terrain, terrainLabel); });

        climateCurrent.text = region.climate.currentMax;
        climateButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.climate, climateLabel); });
        
        floraSlider.value = region.flora.richness;
        floraCurrent.text = region.flora.currentMax.ToString();
        floraSlider.interactable = false;
        floraButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.flora, floraLabel); });
        
        faunaSlider.value = region.fauna.richness;
        faunaCurrent.text = region.fauna.currentMax.ToString();
        faunaSlider.interactable = false;
        faunaButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.fauna, faunaLabel); });

        if (region.isPopulated)
        {
            populationCurrent.text = region.population.ToString();
            //populationButton.onClick.AddListener(delegate { })

            prodModeButton.gameObject.SetActive(true);
            //prodModeCurrent.text = region.productionMode.currentMax.ToString();
            prodModeCurrent.text = "����������� ���������";

            prodModeButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.productionMode, prodModeLabel); });

            ecoButton.gameObject.SetActive(true);
            //ecoCurrent.text = region.ecoCulture.currentMax.ToString();
            ecoCurrent.text = "��������-����������";
            ecoButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.ecoCulture, ecoLabel); });

            govButton.gameObject.SetActive(true);
            //govCurrent.text = region.government.currentMax.ToString();
            govCurrent.text = "�������";
            govButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.government, govLabel); });

            civButton.gameObject.SetActive(true);
            //civCurrent.text = region.civAttachment.currentMax;
            civCurrent.text = "���������������� �������";
            civButton.onClick.AddListener(delegate { ShowParamDetailsPanel(region.civAttachment, civLabel); });
        }
        else
        {
            populationCurrent.text = "0";
            prodModeButton.gameObject.SetActive(false);
            ecoButton.gameObject.SetActive(false);
            govButton.gameObject.SetActive(false);
            civButton.gameObject.SetActive(false);
        }
    }
    public void ShowParamDetailsPanel(Param param, Text title)
    {
        HidePanel(paramDetails);
        paramDetails.gameObject.SetActive(true);
        paramDetailsTitle.text = title.text;
        if (param.isRichnessApplicable)
        {
            richnessTitle.text = "��������� " + param.richness.ToString() + "%";
        }
        pieColor.Init();

        float radialAmount = 0f;
        float yPos = 317f;
        
        foreach (KeyValuePair<string, float> entry in param.quantities)
        {
            if (entry.Value == 0) { continue; }
            GameObject pieObj = Instantiate(radialPrefab, pieParent.transform);                       //���������-��������� ��� ��������� �������� � ����������� ������������, ������������ �� ������ ��������
            pieObj.GetComponent<Image>().color = pieColor.GetPieColorByKey(entry.Key);
            pieObj.name = entry.Key;

            radialAmount += (float)entry.Value / 100;
            pieObj.GetComponent<Image>().fillAmount = radialAmount;

            pieObjects.Add(pieObj);

            GameObject listObj = Instantiate(specListPrefab, specListParent);
            listObj.name = entry.Key;
            listObj.transform.localPosition = new Vector3(0, yPos, 0);
            yPos -= 50f;
            foreach (Component component in listObj.GetComponentsInChildren<Component>())
            {
                if (component.name == "Small Color") component.GetComponent<Image>().color = pieColor.GetPieColorByKey(entry.Key);                
                if (component.name == "Label") component.GetComponent<Text>().text = entry.Key;
                if (component.name == "Value") component.GetComponent<Text>().text = entry.Value.ToString();
            }

            specObjects.Add(listObj);
            
        }
        var sorted = pieObjects.OrderBy(go => go.GetComponent<Image>().fillAmount); //���������� �������� ��� ���������
        foreach (var t in sorted)
        {
            t.transform.SetAsFirstSibling();
        }
    }
    public void ShowPathOfMigrationPanel()
    {
        pathPanel.gameObject.SetActive(true);
    }

    public void HidePanel(RectTransform panel)
    {
        foreach (GameObject go in pieObjects) { Destroy(go); }
        foreach (GameObject go in specObjects) { Destroy(go); }

        pieObjects = new List<GameObject>();               //������ ��� ���������� �������� ���������-���������
        pieObjects.Clear();
        specObjects = new List<GameObject>();
        specObjects.Clear();

        panel.gameObject.SetActive(false);
    }
    public void ShowCow()
    {
        cowPanel.gameObject.SetActive(true);
    }

}