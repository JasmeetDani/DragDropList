using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomListBoxView : MonoBehaviour
{
    public GameObject content;

    public GameObject customListBoxItemPrefab;


    public CustomScrollRect scrollRect;


    private float currentAnchor = 1;

    private float itemHeight = 0.1f;


    // Subject to change when support for a generic list is added, TODO : revisit
    private List<string> boundData;


    public bool AllowDragDrop { get; set; }


    void Start()
    {
        if (AllowDragDrop)
        {
            scrollRect.OnItemMoved += OnItemMoved;

            scrollRect.AllowDragDrop = AllowDragDrop;
        }
    }

    void OnDestroy()
    {
        if (AllowDragDrop)
        {
            // Is below really required ???, TODO : revisit

            scrollRect.OnItemMoved -= OnItemMoved;
        }
    }


    public void Set(List<string> list)
    {
        boundData = list;

        foreach(string s in boundData)
        {
            Add(s);
        }


        gameObject.SetActive(true);
    }

    
    private void Add(string item)
    {
        GameObject itemControl = (GameObject) UnityEngine.Object.Instantiate(customListBoxItemPrefab, content.transform);


        itemControl.GetComponent<Text>().text = item;


        AdjustContentSize();


        currentAnchor = currentAnchor - itemHeight;
    }


    private void AdjustContentSize()
    {
        (content.transform as RectTransform).anchorMin = new Vector2(0, currentAnchor - itemHeight);
    }


    public void OnItemMoved(int oldIndex, int newIndex)
    {
        string item = boundData[oldIndex];

        boundData.RemoveAt(oldIndex);

        boundData.Insert(newIndex, item);
    }
}