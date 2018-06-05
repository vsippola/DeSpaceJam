using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentDisplay : MonoBehaviour
{
    [Range(1, 10)]
    public uint padding = 4;
    [Range(1, 25)]
    public int displaysAcross = 7;
    [Range(1, 25)]
    public int displaysWide = 7;

    public GameObjectPooler pool;
    public Transform parent;
    public GameObject displayTemplate;

    public Vector3 spacing;
    public List<List<GameObject>> displays;

    public Vector2Int centerIndex = Vector2Int.zero;

    private LevelData levelData;
    public Vector2Int displayCenter;

    private void Awake()
    {
        displays = new List<List<GameObject>>();
    }

    void Start()
    {
        levelData = LevelData.Instance;

        float largest = Mathf.Max(levelData.startingMoment.x, levelData.startingMoment.z);
        spacing = new Vector3((padding + largest), 0f, (padding + largest));

        CreateDisplayObjects();
        SetInitialDisplay();
    }

    public void MoveCenter(Vector2Int change)
    {
        centerIndex += change;
        
        if (change.y == 0)
        {
            if(change.x == 1)
            {
                int i = centerIndex.x + displayCenter.x;
                RebuildColumn(displaysWide - 1, i);
            }
            else
            {
                int i = centerIndex.x - displayCenter.x;
                RebuildColumn(0, i);
            }
        }
        else
        {
            if (change.y == 1)
            {
                int i = centerIndex.y + displayCenter.y;
                RebuildRow(displaysAcross - 1, i);
            }
            else
            {
                int i = centerIndex.y - displayCenter.y;
                RebuildRow(0, i);
            }
        }
    }

    private void RebuildRow(int rowIndex, int momentIndex)
    {
        for (int j = 0; j < displaysWide; j++)
        {
            GameObject display = displays[rowIndex][j];
            DisplayData data = display.GetComponentInChildren<DisplayData>();

            //unbuild current display
            data.builder.UnbuildDisplay();

            int k = centerIndex.x - displayCenter.x + j;

            data.builder.SetMoment(levelData.GetMoment(momentIndex, k));
            data.builder.BuildDisplay();
        }
    }

    private void RebuildColumn(int columnIndex, int momentIndex)
    {       
        for (int j = 0; j < displaysAcross; j++)
        {
            GameObject display = displays[j][columnIndex];
            DisplayData data = display.GetComponentInChildren<DisplayData>();

            //unbuild current display
            data.builder.UnbuildDisplay();

            if ((momentIndex < 0) || (levelData.moments[0].Count <= momentIndex)) continue;

            int k = centerIndex.y - displayCenter.y + j;

            if ((k < 0) || (levelData.moments.Count <= k)) continue;            

            data.builder.SetMoment(levelData.GetMoment(k, momentIndex));
            data.builder.BuildDisplay();
        }
    }

    private void SetInitialDisplay()
    {
        List<List<LevelMoment>> moments = levelData.moments;

        for (int j = displayCenter.x; j < displaysAcross; j++)
        {
            for (int i = displayCenter.y; i < displaysWide; i++)
            {
                Vector2Int momentIndex = new Vector2Int
                {
                    x = i - displayCenter.y,
                    y = j - displayCenter.x
                };

                GameObject display = displays[j][i];
                DisplayData data = display.GetComponentInChildren<DisplayData>();

                if (momentIndex.x >= moments[0].Count) continue;
                if (momentIndex.y >= moments.Count) continue;

                data.builder.SetMoment(moments[momentIndex.y][momentIndex.x]);
                //Build initial display
                data.builder.BuildDisplay();
            }
        }
    }

    public void OnMomentBuildEvent(Vector2Int i)
    {      
        Vector2Int j = new Vector2Int (i.y, i.x);

        //Center new index
        j -= centerIndex;

        //adjust to display
        j += displayCenter;

       

        if ((0 < j.x) && (j.x < displaysWide) && (0 < j.y) && (j.y < displaysAcross))
        {
            //Debug.Log(j);

            GameObject display = displays[j.y][j.x];
            DisplayData data = display.GetComponentInChildren<DisplayData>();

            data.builder.SetMoment(levelData.moments[i.x][i.y]);
            data.builder.BuildDisplay();
            
        }       
    }

    private void CreateDisplayObjects()
    {
        //Find display center index
        displayCenter = new Vector2Int((displaysAcross - 1) / 2, (displaysWide - 1) / 2);

        for (int j = 0; j < displaysAcross; j++)
        {
            List<GameObject> row = new List<GameObject>();

            for (int i = 0; i < displaysWide; i++)
            {
                //Make new display node, and position it correctly
                GameObject newParent = new GameObject("Display (" + j + ", " + i + ")");
                newParent.transform.parent = parent;
                TranslateDisplay(i, j, newParent.transform);

                //Make new display view
                GameObject newDisplay = Instantiate(displayTemplate, newParent.transform);
                DisplayData newData = newDisplay.GetComponent<DisplayData>();

                //Set initial targets/position for new display object                
                newData.SetTranslation();
                CentreDisplay(newData);
                newData.SetRotation(-45f);

                //record keeping
                newData.coord = new Vector2Int(j, i);

                //initialize builder.
                newData.builder = new MomentDisplayBuilder(pool, newData.centeringTransform);

                //If parent is on the bounds of the display, it is hidden
                if ((i < 2) || (j < 2) || (i > (displaysAcross - 3)) || (j > (displaysAcross - 3)))
                {
                    newParent.SetActive(false);
                }

                //add parent to array
                row.Add(newParent);                
            }

            displays.Add(row);
        }
    }

    private void TranslateDisplay(int i, int j, Transform display)
    {
        Vector3 center = new Vector3(displayCenter.x, 0f, displayCenter.y);

        float x = (i - center.x) * spacing.x;
        float z = (j - center.z) * spacing.z;

        Vector3 offset = new Vector3(x, 0f, z);

        display.transform.localPosition = offset;

        //data.SetTranslation(data.translationTransform.position - offset);
    }

    private void CentreDisplay(DisplayData data)
    {
        LevelMoment moment = levelData.startingMoment;
        Transform container = data.centeringTransform;

        container.localPosition += new Vector3(.5f, .5f, .5f);
        container.localPosition -= new Vector3(moment.x / 2f, moment.y / 2f, moment.z / 2f);
    }

    public LevelMoment GetFocusedMoment()
    {
        Vector2Int i = centerIndex;
        return levelData.moments[i.y][i.x];
    }

    public void UpdateTimeline(LevelMoment moment)
    {
        //Get display coordinates.
        Vector2Int momentIndex = moment.GetIndex();

        Vector2Int i = MomentIToVectorI(momentIndex);

        //If the row changed is in the current display
        int phase = moment.phase;
        while (true)
        {
            if (!InDisplayColumn(i)) return;

            RebuildRow(i.x, phase);
            i.x++;
            phase++;
        }
        

    }

    private bool InDisplayBounds(Vector2Int i)
    {
        return InDisplayColumn(i) && InDisplayRow(i);
        
    }

    private bool InDisplayColumn(Vector2Int i)
    {
        return (0 <= i.x) && (i.x < displaysAcross);
    }

    private bool InDisplayRow(Vector2Int i)
    {
        return (0 <= i.y) && (i.y < displaysWide);
    }

    private Vector2Int MomentIToVectorI(Vector2Int i)
    {
        Vector2Int j = new Vector2Int(i.y, i.x);

        //Center new index
        j -= centerIndex;

        //adjust to display
        j += displayCenter;

        return j;
    }
}
