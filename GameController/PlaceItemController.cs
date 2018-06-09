using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceItemController : MonoBehaviour
{ 

    public char key = GameBoardCubeDictionary.MOVE_CUBE;
    public GameObjectPooler pool;

    public GameLogicController logicController;
    public MomentDisplay momDisplay;
    public HudController hudControl;
    public RemoveItemController removeItemController;

    Ray ray;
    RaycastHit hit;

    private char oldKey;
    GameObject tempObj;
    Transform displayTrans;

	void Start ()
    {
        //Get an object for display
        tempObj = pool.GetObject(key);
        tempObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        displayTrans = tempObj.transform;
    }

	void Update ()
    {
        CheckDisplayUpdate();
        CheckPlacement();
    }

    private void OnEnable()
    {
        UpdateObject();
    }

    private void UpdateObject()
    {
        if (tempObj != null)
        {
            tempObj.layer = LayerMask.NameToLayer("Default");
            pool.ReturnObject(oldKey, tempObj);
        }

        oldKey = key;

        tempObj = pool.GetObject(key);
        tempObj.layer = LayerMask.NameToLayer("Ignore Raycast");
        displayTrans = tempObj.transform;
    }

    private void CheckPlacement()
    {
        //Check if the user is trying to place the object/cancel out.
        if(Input.GetButtonDown("Fire1"))
        {
            //If the object is active, place it
            if(tempObj.activeSelf)
            {
                PlaceObject();
            }
            else
            {
                DisableController();
            }

            removeItemController.enabled = true;
        }
    }

    private void CheckDisplayUpdate()
    {
        //See if the mouse is over any point
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Vector2Int i = momDisplay.displayCenter;
            Transform centerDisplay = momDisplay.displays[i.x][i.y].transform;

            Transform t = hit.collider.transform;

            //Only place the new object in the centre display
            if (!t.IsChildOf(centerDisplay)) return;

            //Check that the square in the game space is free
            LevelMoment moment = momDisplay.GetFocusedMoment();

            Vector3Int j = new Vector3Int(Mathf.RoundToInt(t.localPosition.x),
                Mathf.RoundToInt(t.localPosition.y + 1),
                Mathf.RoundToInt(t.localPosition.z));

            if (!moment.IsFree(j)) return;


            displayTrans.position = t.position + Vector3.up;
            displayTrans.eulerAngles = t.eulerAngles;
            
            DisplayData data = centerDisplay.GetComponentInChildren<DisplayData>();

            displayTrans.parent = data.centeringTransform;
            tempObj.SetActive(true);

        }
        else
        {
            tempObj.SetActive(false);
        }
    }

    private void PlaceObject()
    {
        Vector3 objPos = tempObj.transform.localPosition;
        Vector3Int pos = new Vector3Int(Mathf.RoundToInt(objPos.x),
            Mathf.RoundToInt(objPos.y),
            Mathf.RoundToInt(objPos.z));

        LevelMoment moment = momDisplay.GetFocusedMoment();
        bool changed = moment.Change(pos, key);

        if (changed)
        {
            LevelData.Instance.ChangeFuture(moment);
            logicController.updating = true;
            momDisplay.UpdateTimeline(moment);
            hudControl.RedrawHud(moment);            
        }

        DisableController();
    }

    public void SetKey(char key)
    {
        this.key = key;
        UpdateObject();
    }

    public void DisableController()
    {
        key = '\0';
        tempObj.SetActive(false);
        this.enabled = false;
    }


}
