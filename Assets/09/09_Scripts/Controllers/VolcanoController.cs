using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using WorldMapStrategyKit;
using UnityEngine.Timeline;

/* Переменные
 * Панель
 * Картинка маркера
 * Картинка панели
 * Время ожидания до старта собитий
 * Лимит для последующего создания события
 * Место активации вулкана
 * Место которое может быть 
 * Полноценный процент удаления населения
 * Полноценный процент удаления пищи
 * Процент от процента удаления населения
 * Процент от процента удаления пищи
 * Событие которое автоматически срабатывает
 * Открывается ли панель при создании события
 */

/* Старт
 * Ждёт 1 минуту
 * Через следующие 0 - 60 секунд создаёт событие
*/

/* При появлении события
 * Если isOpenPanel => открывает панель
 * иначе            => вызывается activateEvent
*/

/* Панель
 * В панели выбирается вариант действий
*/

/* Успокоить вулкан (Стоимость: количество людей / 300)
 * Если isOpenPanel => activateEvent задаётся эта функция
 * Удаляет текущее событие
*/

/* Уменьшить потери (Стоимость: 5)
 * Если isOpenPanel => activateEvent задаётся эта функция
 * Погибает заданное количество процентов населения и запасов пищи от того процента который мог быть без вмешательства
 * Активируется миграцию
 * Включает таймер до следующего события
*/

/* Не вмешиваться
 * Если isOpenPanel => activateEvent задаётся эта функция
 * Выключает панель
*/

/* Удаление события
 * Удаляет событие
 * Активирует ожидание создания собития
 */

/* Если таймер доходит до конца
 * Погибает заданное количество процентов населения и пищи
 * Активирует миграцию
 * Удаляет событие
*/

public class VolcanoController : MonoBehaviour {
    [SerializeField] private EventPanel panel;
    [SerializeField] private IconMarker markerPrefab;
    [SerializeField] private Sprite markerSprite;
    [SerializeField] private Sprite panelSprite;
    [Header("Tickers")]
    [SerializeField, Min(0)] private int startTicksToActivate;
    [SerializeField, Min(0)] private int minTickToActivate;
    [SerializeField, Min(0)] private int maxTickToActivate;
    [Header("Percents")]
    [SerializeField, Range(0, 1)] private float fullPercentFood;
    [SerializeField, Range(0, 1)] private float fullPercentPopulation;
    [SerializeField, Range(0, 1)] private float partPercentFood;
    [SerializeField, Range(0, 1)] private float partPercentPopulation;

    private int ticker;
    private int activateIndex;
    private int ticksToActivateVolcano;
    private CivPiece piece;
    private IconMarker marker;
    private System.Random rand = new();
    private List<Action> autoActions = new();

    private void Awake() {
        autoActions.Add(CalmVolcano);
        autoActions.Add(ReduceLosses);
        autoActions.Add(ActivateVolcano);
        GameEvents.onTickLogic += StartCreateEvent;
    }

    private void StartCreateEvent() {
        ++ticker;
        if (ticker == startTicksToActivate) {
            ticker = 0;
            GameEvents.onTickLogic -= StartCreateEvent;
            CreateEvent();
        }
    }

    public void CreateEvent() {
        System.Random rand = new();
        var civilizations = Transmigratio.Instance.tmdb.humanity.civilizations;
        if (civilizations.Count == 0) return;
        piece = civilizations.ElementAt(rand.Next(0, civilizations.Count)).Value.pieces.ElementAt(rand.Next(0, civilizations.Count)).Value;
        ticksToActivateVolcano = rand.Next(minTickToActivate, maxTickToActivate);
        GameEvents.onTickLogic += WaitActivateVolcano;
        CreateMarker(Transmigratio.Instance.tmdb.map.wmsk.countries[piece.region.id].center);
        if (panel.IsShowAgain) OpenPanel();
    }

    public void RestartEvent() {
        ++ticker;
        if (ticker == ticksToActivateVolcano) {
            GameEvents.onTickLogic -= RestartEvent;
            ticker = 0;
            CreateEvent();
        }
    }

    public void CreateMarker(Vector2 position) {
        marker = Instantiate(markerPrefab);
        marker.Sprite = markerSprite;
        marker.OnClick += (int i) => OpenPanel();

        MarkerClickHandler handler = Transmigratio.Instance.tmdb.map.wmsk.AddMarker2DSprite(marker.gameObject, position, 0.03f, true, true);
        handler.allowDrag = false;

    }

    public void OpenPanel() {
        panel.Open();
        panel.onClick = ActivateDesidion;
        panel.Image = panelSprite;
        panel.Title = /*StringLoader.Load(*/"VolcanoTitle"/*)*/;
        panel.Description = /*StringLoader.Load(*/"VolcanoDescription"/*)*/;
        panel.Territory = /*string.Format(StringLoader.Load(*/ "VolcanoTerritory"/*) */;
        //                                 StringLoader.Load($"{piece.region.name}"),
        //                                 StringLoader.Load($"{piece.civilization.name}"));
        panel.AddDesidion(/*StringLoader.Load(*/"CalmVolcano"/*)*/, (int)(piece.population.value / 300f));
        panel.AddDesidion(/*StringLoader.Load(*/"ReduceLosses"/*)*/, 5);
        panel.AddDesidion(/*StringLoader.Load(*/"Nothing"/*)*/, 0);
    }

    public void ActivateDesidion(int index) {
        if (index == 0) CalmVolcano();
        if (index == 1) ReduceLosses();
        if (index == 2) Nothing();
    }

    private void CalmVolcano() {
        activateIndex = 0;
        Debug.Log("CalmVolcano");

        GameEvents.onTickLogic -= WaitActivateVolcano;
        ticker = 0;
        panel.Close();
        marker.Destroy();
        GameEvents.onTickLogic += RestartEvent;
    }

    private void ReduceLosses() {
        activateIndex = 1;
        piece.population.value -= (int)(piece.population.value * fullPercentFood * partPercentPopulation);
        piece.reserveFood -= piece.reserveFood * fullPercentFood * partPercentFood;

        Debug.Log("ReduceLosses");

        GameEvents.onTickLogic -= WaitActivateVolcano;
        ticker = 0;
        panel.Close();
        marker.Destroy();
        GameEvents.onTickLogic += RestartEvent;
    }

    private void Nothing() {
        activateIndex = 2;
        panel.Close();
    }

    private void ActivateVolcano() {
        piece.population.value -= (int)(piece.population.value * fullPercentFood);
        piece.reserveFood -= piece.reserveFood * fullPercentFood;

        Debug.Log("ActivateVolcano");

        GameEvents.onTickLogic -= WaitActivateVolcano;
        ticker = 0;
        panel.Close();
        marker.Destroy();
        GameEvents.onTickLogic += RestartEvent;
    }

    private void WaitActivateVolcano() {
        ++ticker;
        if (ticker == ticksToActivateVolcano) {
            ticker = 0;
            marker.Destroy();
            autoActions[activateIndex]?.Invoke();
            GameEvents.onTickLogic += RestartEvent;
        }
    }
}
