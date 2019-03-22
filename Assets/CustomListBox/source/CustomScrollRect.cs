using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    public bool AllowDragDrop { get; set; }


    public GameObject dragPlaceHolder;

    public GameObject demarcation;


    private bool bDragging = false;


    private float deltaX, deltaY;


    private float prevY;


    private const float autoScrollSensitivity = 0.001f;

    private const int autoScrollSpeedUpFactor = 10;

    private bool bScrollUp = false;


    private int startIndex;

    private int dropIndex;


    public delegate void ItemMoved(int oldIndex, int newIndex);

    public event ItemMoved OnItemMoved;


    private void AutoScroll(float scrollSensitivity)
    {
        if (content.rect.height > viewport.rect.height)
        {
            // Scroll up or down as required

            float scrollDelta;

            if (!bScrollUp)
            {
                scrollDelta = normalizedPosition.y + scrollSensitivity;
            }
            else
            {
                scrollDelta = normalizedPosition.y - scrollSensitivity;
            }

            if ((scrollDelta >= 0) && (scrollDelta <= 1))
            {
                SetNormalizedPosition(scrollDelta, 1);
            }
        }
    }


    void Update()
    {
        if (AllowDragDrop)
        {
            if (bDragging)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(viewport, dragPlaceHolder.transform.position))
                {
                    AutoScroll(autoScrollSensitivity);
                }
                else
                {
                    // We are out of the viewport

                    Vector3[] viewportWorld = new Vector3[4];

                    viewport.GetWorldCorners(viewportWorld);

                    // Below aren't we comparing world with screen coordinates ???, TODO : revisit
                    // I guess for screen space overlay canvas, GetWorldCorners returns corners in screen space

                    if (dragPlaceHolder.transform.position.y > viewportWorld[1].y)
                    {
                        // Speed up the scroll

                        if (!bScrollUp)
                        {
                            AutoScroll(autoScrollSensitivity * autoScrollSpeedUpFactor);
                        }
                    }

                    if (dragPlaceHolder.transform.position.y < viewportWorld[0].y)
                    {
                        // Speed up the scroll

                        if (bScrollUp)
                        {
                            AutoScroll(autoScrollSensitivity * autoScrollSpeedUpFactor);
                        }
                    }


                    demarcation.SetActive(false);
                }
            }
        }
    }


    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (AllowDragDrop)
        {
            bDragging = true;


            dragPlaceHolder.SetActive(true);


            deltaX = eventData.position.x - eventData.pointerCurrentRaycast.gameObject.transform.position.x;

            deltaY = eventData.position.y - eventData.pointerCurrentRaycast.gameObject.transform.position.y;


            dragPlaceHolder.transform.position = eventData.pointerCurrentRaycast.gameObject.transform.position;

            (dragPlaceHolder.transform as RectTransform).sizeDelta =
                (eventData.pointerCurrentRaycast.gameObject.transform as RectTransform).sizeDelta;


            dragPlaceHolder.GetComponent<Text>().text = eventData.pointerCurrentRaycast.gameObject.GetComponent<Text>().text;

            eventData.pointerCurrentRaycast.gameObject.GetComponent<Text>().text = "";

            startIndex = eventData.pointerCurrentRaycast.gameObject.transform.GetSiblingIndex();
        }
        else
        {
            base.OnBeginDrag(eventData);
        }
    }


    public override void OnDrag(PointerEventData eventData)
    {
        if (AllowDragDrop)
        {
            Vector2 newPos = eventData.position;

            newPos.x = newPos.x - deltaX;

            newPos.y = newPos.y - deltaY;


            if (newPos.y > prevY)
            {
                bScrollUp = false;
            }
            else
            {
                bScrollUp = true;
            }


            dragPlaceHolder.transform.position = newPos;

            prevY = newPos.y;


            // Calculate where the placeholder is relative to the list

            PointerEventData pointerData = new PointerEventData(EventSystem.current);

            pointerData.position = dragPlaceHolder.transform.position;

            List<RaycastResult> results = new List<RaycastResult>();

            EventSystem.current.RaycastAll(pointerData, results);

            GameObject current = null;

            foreach (RaycastResult rt in results)
            {
                if (rt.gameObject.transform.parent == content.transform)
                {
                    current = rt.gameObject;
                }
            }


            if (current != null)
            {
                // This can happen when we the content height is less than that of the viewport

                int currentIndex = current.transform.GetSiblingIndex();

                Transform prev = null;

                if (currentIndex > 0)
                    prev = content.GetChild(currentIndex - 1);

                Transform next = null;

                if (currentIndex < (content.childCount - 1))
                    next = content.GetChild(currentIndex + 1);


                // Set the position of the demarcation line

                Vector3[] dragPlaceHolderWorldRect = new Vector3[4];

                Vector3[] prevWorldRect = new Vector3[4];

                Vector3[] nextWorldRect = new Vector3[4];

                Vector3[] viewportWorldRect = new Vector3[4];

                (dragPlaceHolder.transform as RectTransform).GetWorldCorners(dragPlaceHolderWorldRect);

                (viewport.transform as RectTransform).GetWorldCorners(viewportWorldRect);

                if (prev != null)
                {
                    (prev.transform as RectTransform).GetWorldCorners(prevWorldRect);

                    if (GeomUtils.DoOverlap(dragPlaceHolderWorldRect[1], dragPlaceHolderWorldRect[3],
                        prevWorldRect[1], prevWorldRect[3]))
                    {
                        demarcation.SetActive(true);


                        Vector3 demarcationPos = demarcation.transform.position;

                        demarcation.transform.position = new Vector3(
                            demarcationPos.x,
                            prevWorldRect[0].y,
                            demarcationPos.z);


                        if (startIndex < currentIndex)
                        {
                            dropIndex = prev.transform.GetSiblingIndex();
                        }
                        else
                        {
                            dropIndex = currentIndex;
                        }


                        return;
                    }
                }
                else
                {
                    // We are at the top of the list

                    if (dragPlaceHolder.transform.position.y > current.transform.position.y)
                    {
                        demarcation.SetActive(true);


                        Vector3 demarcationPos = demarcation.transform.position;

                        demarcation.transform.position = new Vector3(
                            demarcationPos.x,
                            viewportWorldRect[1].y,
                            demarcationPos.z);


                        dropIndex = 0;


                        return;
                    }
                }

                if (next != null)
                {
                    (next.transform as RectTransform).GetWorldCorners(nextWorldRect);

                    if (GeomUtils.DoOverlap(dragPlaceHolderWorldRect[1], dragPlaceHolderWorldRect[3],
                        nextWorldRect[1], nextWorldRect[3]))
                    {
                        demarcation.SetActive(true);


                        Vector3 demarcationPos = demarcation.transform.position;

                        demarcation.transform.position = new Vector3(
                            demarcationPos.x,
                            nextWorldRect[1].y,
                            demarcationPos.z);


                        if (startIndex <= currentIndex)
                        {
                            dropIndex = currentIndex;
                        }
                        else
                        {
                            dropIndex = next.transform.GetSiblingIndex();
                        }


                        return;
                    }
                }
                else
                {
                    // We are at the bottom of the list

                    if (dragPlaceHolder.transform.position.y < current.transform.position.y)
                    {
                        demarcation.SetActive(true);


                        Vector3 demarcationPos = demarcation.transform.position;

                        demarcation.transform.position = new Vector3(
                            demarcationPos.x,
                            viewportWorldRect[0].y,
                            demarcationPos.z);


                        dropIndex = content.childCount - 1;


                        return;
                    }
                }


                demarcation.SetActive(false);
            }
        }
        else
        {
            base.OnDrag(eventData);
        }
    }


    public override void OnEndDrag(PointerEventData eventData)
    {
        if (AllowDragDrop)
        {
            bDragging = false;


            Vector2 newPos = eventData.position;

            newPos.x = newPos.x - deltaX;

            newPos.y = newPos.y - deltaY;

            if (RectTransformUtility.RectangleContainsScreenPoint(viewport, newPos))
            {
                dragPlaceHolder.transform.gameObject.SetActive(false);


                // Re order the list

                GameObject orig = content.GetChild(startIndex).gameObject;

                orig.GetComponent<Text>().text = dragPlaceHolder.GetComponent<Text>().text;

                orig.transform.SetSiblingIndex(dropIndex);


                // Notify subscribers of the change

                OnItemMoved(startIndex, dropIndex);
            }
            else
            {
                GameObject orig = content.GetChild(startIndex).gameObject;

                orig.GetComponent<Text>().text = dragPlaceHolder.GetComponent<Text>().text;


                dragPlaceHolder.transform.gameObject.SetActive(false);
            }


            demarcation.SetActive(false);
        }
        else
        {
            base.OnEndDrag(eventData);
        }
    }


    protected override void LateUpdate()
    {
        if (AllowDragDrop)
        {
            if (content.rect.height < viewport.rect.height)
            {
                return;
            }
        }


        base.LateUpdate();

        // Workaround to prevent the code from centering the content in case the content height is
        // less than that of the viewport

        // TODO : revisit for more clarity
    }
}