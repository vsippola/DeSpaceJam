using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveItemController : MonoBehaviour
{       
    public GameObjectPooler pool;

    public GameLogicController logicController;
    public MomentDisplay momDisplay;
    public HudController hudControl;
    public GameObject highlightObject;

    char key;
    Ray ray;
    RaycastHit hit;

    Vector3Int overI;

    float startUpDelay = 0f;

    GameObject highlight;
    Transform displayTrans;

    public GameLogicController logicCont;

    private void Start()
    {
        ResetOverI();
        highlight = Instantiate(highlightObject);
        displayTrans = highlight.transform;

        logicCont.GameWinEvent += OnWin;
    }

    void Update()
    {

        startUpDelay -= Time.deltaTime;
        if (startUpDelay > 0) return;
        

        CheckDisplayUpdate();
        CheckPlacement();
    }

    private void OnWin()
    {
        logicCont.GameWinEvent -= OnWin;
        enabled = false;
    }

    private void OnEnable()
    {
        startUpDelay = .5f;
    }

    private void CheckPlacement()
    {
        //Check if the user is trying to place the object/cancel out.
        if (Input.GetButtonDown("Fire1"))
        {
            //If the object is active, place it
            if (overI.x >= 0)
            {
                RemoveObject();
            }
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
                Mathf.RoundToInt(t.localPosition.y),
                Mathf.RoundToInt(t.localPosition.z));


            key = moment.level[j.x, j.y, j.z];

            if (IsRemoveable(key))
            {
                overI = j;

                Highlight(t);
            }
            else
            {
                UnHighlight();
            }
        }
        else
        {
            UnHighlight();
        }
    }

    private void Highlight(Transform t)
    {
        if (overI.x < 0) return;
        Vector2Int i = momDisplay.displayCenter;
        Transform centerDisplay = momDisplay.displays[i.x][i.y].transform;

        displayTrans.position = t.position;
        displayTrans.eulerAngles = t.eulerAngles;

        DisplayData data = centerDisplay.GetComponentInChildren<DisplayData>();

        displayTrans.parent = data.centeringTransform;
        highlight.SetActive(true);

    }

    private void UnHighlight()
    {
        highlight.SetActive(false);
        ResetOverI();
    }

    private bool IsRemoveable(char key)
    {
        if (key == GameBoardCubeDictionary.MOVE_CUBE) return true;
        if (key == GameBoardCubeDictionary.WORMHOLE_IN) return true;
        return false;
    }


    private void DisableController()
    {
        UnHighlight();
        this.enabled = false;
    }

    private void RemoveObject()
    {
        if(overI.x < 0) return;

        LevelMoment moment = momDisplay.GetFocusedMoment();
        bool changed = moment.Remove(overI, key);

        if (!changed) return;

        LevelData.Instance.ChangeFuture(moment);
        
        momDisplay.UpdateTimeline(moment);
        hudControl.RedrawHud(moment);
        logicController.updating = true;
    }

    public void Disable()
    {
        UnHighlight();
        this.enabled = false;
    }

    private void ResetOverI()
    {
        overI = new Vector3Int(-1, 0, 0);
    }
}
