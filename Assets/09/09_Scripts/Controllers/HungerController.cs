using System.Collections.Generic;
using UnityEngine;

public class HungerController : MonoBehaviour {
    [SerializeField] private EventPanel panel;
    [SerializeField] private Sprite markerIconSprite;
    [SerializeField] private Sprite panelSprite;

    private List<EventData> events = new();

    public void OpenPanel(int index) {
        panel.Open();
        panel.Image = panelSprite;
        panel.Title = StringLoader.Load("HungerTitle");
        panel.Description = StringLoader.Load("HungerDescription");
        panel.Territory = string.Format(StringLoader.Load("HungerFrom"),
                                  StringLoader.Load($"Region {events[index].piece.region.id}"),
                                  StringLoader.Load(events[index].piece.civilization.name));
        panel.AddDesidion(StringLoader.Load("AddFood"), 15);
        panel.AddDesidion(StringLoader.Load("AddSomeFood"), 5);
        panel.AddDesidion(StringLoader.Load("Nothing"), 0);
    }

    public void ClosePanel() {
        panel.Close();
    }

    public void AddEvent(EventData e) => events.Add(e);
    public void RemoveEvent(EventData index) => events.Remove(index);

    private void AddFood(CivPiece piece) {
        Debug.Log("AddFood");
    }

    private void AddSomeFood(CivPiece piece) {
        Debug.Log("AddSomeFood");
    }

    private void Nothing(CivPiece piece) {
        Debug.Log("Nothing");
    }
}
