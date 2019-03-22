using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomListBoxTest : MonoBehaviour {


    public Text listContents;


    private CustomListBoxView customListBox;

    private List<string> list;


    // Use this for initialization
    void Start () {

        GameObject obj = ((GameObject) Instantiate(Resources.Load("prefabs\\CustomListBox\\CustomListBox"), transform));

        customListBox = obj.GetComponent<CustomListBoxView>();

        customListBox.AllowDragDrop = true;

        list = new List<string>()
        {
            "Manufacturer",
            "Theo Win",
            "Theo Win Per Day",
            "Slot_StandID",
            "Actual Hold %",
            "Denom",
            "Actual Win",
            "FreePlay",
            "Leased Fees",
            "Theme",
            "Reels",
            "Game Type",
            "Location",
            "Bank",
            "Cabinet Type",
            "Occupancy",
            "Section",
            "Coin In",
            "Occupancy",
            "Net Theo Win",
        };

        customListBox.Set(list);


        Refresh();
    }


    public void Refresh()
    {
        listContents.text = "List : \n\n ";

        foreach (string s in list)
        {
            listContents.text = listContents.text + s + "\n ";
        }
    }
}