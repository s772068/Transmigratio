using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MigrationTrialsGroup : MonoBehaviour {
    [SerializeField] private TMP_Text title;
    [SerializeField] private MigrationTrialElement element;
    [SerializeField] private Transform content;

    private List<MigrationTrialElement> trials = new();

    // private void Awake() {
    //     title.text = StringLoader.Load("Trials");
    // }

    public void AddTrial(Sprite sprite) {
        var trial = Instantiate(element, content);
        trial.Sprite = sprite;
        trials.Add(trial);
    }

    public void ClearTrials() {
        for(int i = 0; i < trials.Count; ++i) {
            trials[i].Destroy();
        }
        trials.Clear();
    }
}
