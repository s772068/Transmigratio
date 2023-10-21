using UnityEngine;
using Zenject;

public class HUD : MonoSingleton<HUD> {
    [Inject] private SpritesHolder spritesHolder;
    [Inject] private PrefabsHolder prefabsHolder;

    private GUI_RegionPanel.Data RegionData => new() {
        CountryName = "������������",
        CloseString = "��������",
        Groups = new GUI_ParamsGroup.Data[] {
            new() {
                Label = "��������",
                Height = 280,
                BackgroundColor = new Color().Parse(63, 123, 50, 212),
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
                BackgroundColor = new Color().Parse(162, 132, 178, 199),
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

    private GUI_PathPanel.Data PathData => new() {
        Label = "���� ��������",
        Path = "�� � � �",
        Description = "���� ������������ � ����� ����� � ������� ������ �����. ������ � ������ ������������ ������, ��������, ���������� � �������",
        BreakString = "��������",
        CloseString = "��������",
        IconIndex = 0,
        Progress = new() {
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

    public void OpenPathPanel() {
        GUI_PathPanel path = Instantiate(prefabsHolder.PathPanel, transform);
        path.Init(PathData, spritesHolder);
    }

    public void OpenRegionPanel() {
        GUI_RegionPanel region = Instantiate(prefabsHolder.RegionPanel, transform);
        region.Init(RegionData, prefabsHolder);
    }
}
