using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    //Это по-хорошему надо было делать через доступ к дочерним объектам, но понял я это уже когда скрипт был готов, поэтому пока оставлю так, чтобы без лишней надобности не тратить время.
    [SerializeField] GameObject doorLeft;
    [SerializeField] GameObject doorRight;

    bool closed = true;
    bool isClosing;
    bool isOpening;

    float closedL;
    float openedL;
    float closedR;
    float openedR;

    private void Start()
    {
        closedL = doorLeft.transform.localPosition.z;
        openedL = closedL - 1.8f;
        closedR = doorRight.transform.localPosition.z;
        openedR = closedR + 1.8f;
    }

    private void Update()
    {
        if (isOpening)
        {
            if (closedL - doorLeft.transform.localPosition.z > openedL)
            {
                doorLeft.transform.Translate(transform.forward * Time.deltaTime, Space.World);
            }

            if (closedR + doorRight.transform.localPosition.z < openedR)
                doorRight.transform.Translate(-transform.forward * Time.deltaTime, Space.World);

            if (closedL - doorLeft.transform.localPosition.z <= openedL && closedR - doorRight.transform.localPosition.z >= openedR)
            {
                isOpening = false;
                closed = false;
            }
        }

        if (isClosing)
        {
            if (closedL - doorLeft.transform.localPosition.z < closedL)
            { 
                doorLeft.transform.Translate(-transform.forward * Time.deltaTime, Space.World);
            }

            if (closedR + doorRight.transform.localPosition.z < closedR)
                doorRight.transform.Translate(transform.forward * Time.deltaTime, Space.World);

            if (closedL - doorLeft.transform.localPosition.z >= closedL && closedR + doorRight.transform.localPosition.z <= closedR)
            {
                isClosing = false;
                closed = true;
            }
        }

    }

    public void Activate()
    {
        if (!isOpening && !isClosing)
        {
            if (closed)
                isOpening = true;
            else
                isClosing = true;
            closed = !closed;
        }
    }
}
