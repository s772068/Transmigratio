using UnityEngine;

public class HUD : BaseController {
    [SerializeField] private GUI_PathPanel migrationPanel;
    [SerializeField] private GUI_RegionPanel regionPanel;
    [SerializeField] private GUI_EventPanel eventPanel;
    
    private MigrationController migrationController;
    private EventsController eventsController;
    private WmskController wmskController;
    private SaveController saveController;

    public int PathIndex { private get; set; }
    public int CountryIndex { private get; set; }

    public override GameController GameController {
        set {
            migrationController = value.Get<MigrationController>();
            eventsController = value.Get<EventsController>();
            wmskController = value.Get<WmskController>();
            saveController = value.Get<SaveController>();
        }
    }

    #region Datas
    private GUI_RegionPanel.Data RegionData => new() {
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

    private GUI_PathPanel.Data MigrationData => new() {
        Label = "���� ��������",
        Path = "�� � � �",
        Description = "���� ������������ � ����� ����� � ������� ������ �����. ������ � ������ ������������ ������, ��������, ���������� � �������",
        BreakString = "��������",
        CloseString = "��������",
        IconIndex = 0,
        Paramiter = new() {
            Label = "�������������",
            Value = 62,
            Metric = "%",
            Progress = new() {
                Range = new Vector2(0, 100),
                FillColor = new Vector4(255, 162, 0, 255),
                BackgroundColor = new Vector4(176, 160, 160, 255)
            }
        }
    };
    #endregion

    public bool OpenRegionPanel(ref S_Country country) {
        if(regionPanel.gameObject.activeSelf) return false;
        regionPanel.gameObject.SetActive(true);
        regionPanel.Init(RegionData);
        regionPanel.OnClose = () => regionPanel.gameObject.SetActive(false);
        return true;
    }

    public bool OpenMigrationPanel() {
        if (migrationPanel.gameObject.activeSelf) return false;
        migrationPanel.gameObject.SetActive(true);
        migrationPanel.Init(MigrationData);
        migrationPanel.OnClose = () => migrationPanel.gameObject.SetActive(false);
        return true;
    }

    public bool OpenEventPanel(S_Event data, int countryIndex, int eventIndex) {
        if(eventPanel.gameObject.activeSelf) return false;
        eventPanel.gameObject.SetActive(true);
        eventPanel.Init(data);
        eventPanel.OnClickResult = (int index) =>  eventsController.ClickResult(countryIndex, eventIndex, index);
        eventPanel.OnClose = () => eventPanel.gameObject.SetActive(false);
        return true;
    }

    public bool UpdateRegionPanel(ref S_Country country) {
        if(regionPanel.enabled) return false;
        regionPanel.UpdatePanel(ref country);
        return true;
    }

    public bool UpdateMigrationPanel(int value) {
        if (migrationPanel.enabled) return false;
        migrationPanel.UpdatePanel(value);
        return true;
    }
}
