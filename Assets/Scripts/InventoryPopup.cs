using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryPopup : MonoBehaviour
{
    [SerializeField] private Image[] itemIcons;
    [SerializeField] private Text[] itemLabels;
    [SerializeField] private Text curItemLabel;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button useButton;
    private string _curItem;

    public void Refresh() {
        List<string> itemList = Managers.Inventory.GetItemList();

        int len = itemIcons.Length;
        for (int i=0; i<len; i++) {
            //Check inventory list while looping through all UI images
            if (i < itemList.Count) {
                itemIcons[i].gameObject.SetActive(true);
                itemLabels[i].gameObject.SetActive(true);

                string item = itemList[i];

                //Set icon's sprites
                Sprite sprite = Resources.Load<Sprite>("Icons/" + item);
                itemIcons[i].sprite = sprite;
                itemIcons[i].SetNativeSize();

                //Set labels to show count and 'equipped' if so
                int count = Managers.Inventory.GetItemCount(item);
                string message = "x" + count;
                if (item == Managers.Inventory.equippedItem) {
                    message = "Equipped\n" + message;
                }
                itemLabels[i].text = message;

                //Clicking on icons
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(
                    //Lambda function to trigger differently for each item
                    (BaseEventData data) => {OnItem(item);}
                );
                EventTrigger trigger = itemIcons[i].GetComponent<EventTrigger>();
                //Clear listener to refresh and add this listener function to EventTrigger
                trigger.triggers.Clear();
                trigger.triggers.Add(entry);
            }
            else {
                //Hide image and label if no more item to display
                itemIcons[i].gameObject.SetActive(false);
                itemLabels[i].gameObject.SetActive(false);
            }
        }

        if (!itemList.Contains(_curItem)) {
            _curItem = null;
        }

        //Hide buttons if no item selected
        if (_curItem == null) {
            curItemLabel.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            useButton.gameObject.SetActive(false);
        }
        //Display currently selected item
        else {
            curItemLabel.gameObject.SetActive(true);
            equipButton.gameObject.SetActive(true);
            //'Use' button only for health item
            if (_curItem == "health") {
                useButton.gameObject.SetActive(true);
            }
            else {
                useButton.gameObject.SetActive(false);
            }
            curItemLabel.text = _curItem + ":";
        }
    }

    //Function called by mouse click listener
    public void OnItem(string item) {
        _curItem = item;
        Refresh();
    }

    public void OnEquip() {
        Managers.Inventory.EquipItem(_curItem);
        Refresh();
    }

    public void OnUse() {
        Managers.Inventory.ConsumeItem(_curItem);
        if (_curItem == "health") {
            Managers.Player.ChangeHealth(25);
        }
        Refresh();
    }
}
