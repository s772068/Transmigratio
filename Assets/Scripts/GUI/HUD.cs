using UnityEngine;

public class HUD : BaseController {
    [SerializeField] private GUI_CountryPanel countryPanel;
    //[SerializeField] private GUI_EventPanel eventPanel;

    private LocalizationController localization;
    //private MigrationController migration;
    //private EventsController events;
    //private WmskController wmsk;
    //private SaveController save;

    private int _migrationIndex;
    private int _countryIndex;
    private int _eventIndex;

    public override GameController GameController {
        set {
            localization = value.Get<LocalizationController>();
            //migration = value.Get<MigrationController>();
            //events = value.Get<EventsController>();
            //wmsk = value.Get<WmskController>();
            //save = value.Get<SaveController>();
        }
    }

    #region Datas
    private GUI_CountryPanel.Data RegionData => new() {
        CountryName = "������������",
        CloseString = "��������",
        Groups = new GUI_ParamsGroup.Data[] {
            new() {
                Label = "��������",
                Height = 280,
                BackgroundColor = new Vector4(63, 123, 50, 212),
                Params = new GUI_Param.Data[] {
                    new() {
                        Label = "���������",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            BackgroundColor = new Vector4(255, 162, 0, 255)
                        }
                    },
                    new() {
                        Label = "������",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            BackgroundColor = new Vector4(255, 162, 0, 255)
                        }
                    },
                    new() {
                        Label = "�����",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            Range = new Vector2(0, 1250000),
                            FillColor = new Vector4(0, 178, 0, 128),
                            BackgroundColor = new Vector4(176, 160, 160, 255)
                        }
                    },
                    new() {
                        Label = "�����",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            Range = new Vector2(0,378787),
                            FillColor = new Vector4(255, 0, 0, 128),
                            BackgroundColor = new Vector4(176, 160, 160, 255)
                        }
                    }
                }
            },
            new() {
                Label = "���� � �������",
                Height = 330,
                BackgroundColor = new Vector4(162, 132, 178, 199),
                Params = new GUI_Param.Data[] {
                    new() {
                        Label = "���������",
                        IsClickable = true,
                        Value = 55320,
                        Progress = new() {
                            BackgroundColor = new Vector4(255, 162, 0, 255)
                        }
                    },
                    new() {
                        Label = "������ ��-��",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            BackgroundColor = new Vector4(255, 162, 0, 255)
                        }
                    },
                    new() {
                        Label = "��. �����",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            Range = new Vector2(0, 1250000),
                            BackgroundColor = new Vector4(255, 162, 0, 255)
                        }
                    },
                    new() {
                        Label = "�����",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            Range = new Vector2(0,378787),
                            BackgroundColor = new Vector4(255, 162, 0, 255)
                        }
                    },
                    new() {
                        Label = "�����������",
                        IsClickable = true,
                        Value = 125000,
                        Progress = new() {
                            Range = new Vector2(0,378787),
                            BackgroundColor = new Vector4(255, 162, 0, 255)
                        }
                    }
                }
            }
        }
    };
    #endregion

    public bool OpenCountryPanel(ref S_Country country) {
        if(countryPanel.gameObject.activeSelf) return false;
        countryPanel.gameObject.SetActive(true);
        countryPanel.Init(RegionData);
        countryPanel.OnClose = () => countryPanel.gameObject.SetActive(false);
        return true;
    }

    //public bool OpenEventPanel(S_Event data, int countryIndex, int eventIndex) {
    //    if(eventPanel.gameObject.activeSelf) return false;
    //    _eventIndex = eventIndex;
    //    eventPanel.gameObject.SetActive(true);
    //    eventPanel.Init(data);
    //    eventPanel.OnClickResult = (int index) =>  events.ClickResult(countryIndex, eventIndex, index);
    //    eventPanel.OnClose = () => eventPanel.gameObject.SetActive(false);
    //    return true;
    //}

    public bool UpdateCountryPanel(ref S_Country country, int index) {
        if(index != _countryIndex) return false;
        if(!countryPanel.gameObject.activeSelf) return false;
        countryPanel.UpdatePanel(ref country);
        return true;
    }
}
