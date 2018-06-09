using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MomentDisplayController : MonoBehaviour
{
    public delegate void MomentDisplayChange(LevelMoment newMoment);
    public event MomentDisplayChange MomentDisplayChangeEvent;

    [Range(1f, 10f)]
    public float rotationSpeed = 2f;
    [Range(1f, 25f)]
    public float rotationSmoothing = 7f;
    [Range(1f, 10f)]
    public float translationSmoothing = 2f;

    [Range(0.001f, 1f)]
    public float deadzone = .001f;

    public MomentDisplay displayObjects;
    public ParticleEffectController particles;

    private bool checkInput;

    [Range(0.1f, 1f)]
    public float arrowKeyDelay = .5f;
    private Vector2 arrowKeyTimer = Vector2.zero;
    public bool scrolling = false;

    private bool done = false;
    private bool targeting = false;

    public GameLogicController logicCont;
    public Button targetBtn;

    private float targetDelay = .5f;
    private float targetTimer = .5f;

    private void Start()
    {
        logicCont.GameWinEvent += OnWin;
        targetBtn.onClick.AddListener(() => StartTargeting());
    }

    private void Update()
    {
        ProcessDisplayChanges();
        if (targeting)
        {
            TargetWinner();
            return;
        }       
        if (checkInput) CheckMouseInput();
        
        CheckKeyboardInput();        
    }

    private void TargetWinner()
    {
        if (!targeting || !done) return;

        
        targetTimer -= Time.deltaTime;
      
        if (targetTimer > 0) return;
        targetTimer = targetDelay;

        Vector2Int target = logicCont.winIndex;
        Vector2Int curIndex = displayObjects.centerIndex;

        if (target.y != curIndex.y)
        {
            if(target.y > curIndex.y)
            {
                Shift(true, false);
            }
            else
            {
                Shift(true, true);
            }
            return;
        }
        else if (target.x != curIndex.x)
        {            
            if (target.x > curIndex.x)
            {
                Shift(false, false);
            }
            else
            {
                Shift(false, true);
            }
            return;
        }

        targeting = false;
    }

    private void StartTargeting()
    {
        if (targeting) return;
        if (!done) return;

        targeting = true;
    }

    private void OnWin()
    {
        
        logicCont.GameWinEvent -= OnWin;
        done = true;
    }

    private void CheckMouseInput()
    {
        //Check if the mouse has been moved horizontally
        float rotationChange = Input.GetAxisRaw("Mouse X");
        if (rotationChange == 0) return;

        //If the wheel is moved move the target
        float degress = rotationChange * rotationSpeed;
        foreach (List<GameObject> row in displayObjects.displays)
        {
            foreach (GameObject display in row)
            {
                DisplayData data = display.GetComponentInChildren<DisplayData>();
                data.ChangeRotation(degress);
            }
        }
    }

    private void CheckKeyboardInput()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        arrowKeyTimer -= Vector2.one * Time.deltaTime;

        bool changed = false;

        if (input.x != 0)            
        {
            changed = scrolling;

            if (arrowKeyTimer.x <= 0)
            {
                changed = true;

                if(input.x > 0)
                {
                    Shift(false, false);
                }
                else
                {
                    Shift(false, true);
                }
                
                arrowKeyTimer.x = arrowKeyDelay;
                if (scrolling) arrowKeyTimer.x /= 3;
            }
        }
        else if (Input.GetKeyDown(KeyCode.PageUp))
        {
            changed = true;
            for (int i = 0; i < 10; i++)
            {
                Shift(false, false);
            }
        }
        else if(Input.GetKeyDown(KeyCode.PageDown))
        {
            changed = true;
            for (int i = 0; i < 10; i++)
            {
                Shift(false, true);
            }
        }

        if (input.y != 0)
        {
            changed = scrolling;

            if (arrowKeyTimer.y <= 0)
            {
                
                changed = true;

                if (input.y > 0)
                {
                    Shift(true, false);
                }
                else
                {
                    Shift(true, true);
                }
                
                arrowKeyTimer.y = arrowKeyDelay;
                if (scrolling) arrowKeyTimer.y /= 3;
            }
        }

        scrolling = changed;
    }

    //Default is Shift Horizontally right (move area left)
    //SwapXY = shift down
    //SwapLR = shift area left
    //Both = shift up
    private void Shift(bool swapXY, bool swapLR)
    {
        List<List<LevelMoment>> moments = LevelData.Instance.moments;

        Vector2Int bounds = new Vector2Int
        {
            x = 0,
            y = ((swapXY) ? moments.Count : moments[0].Count) - 1
        };

        //Chane which center value is checked base on X or Y shift
        int center = (swapXY)? displayObjects.centerIndex.y: displayObjects.centerIndex.x;

        //If shifting left and at the start, do not shift
        if (swapLR && (center == bounds.x)) return;
        //If shifting right and at the start, do not shift
        if (!swapLR && (center == bounds.y)) return;

        //Signale particles to animate
        particles.BumpSpeed();
        List<List<GameObject>> grid = displayObjects.displays;

        Vector2Int centerChange = Vector2Int.zero;

        //4 = -1 (mod 5) (same for things not 5)
        Vector2Int previous = Vector2Int.zero;
        Vector2Int m = new Vector2Int(grid[0].Count, grid.Count);
        if (swapXY)
        {
            if (swapLR)
            {
                previous.x = 1;
                centerChange.y = -1;
            }
            //previous = (-1,0);
            else
            {
                previous.x = m.y - 1;
                centerChange.y = 1;
            }
        }
        else
        {                    
            //previous = (1,0);
            if (swapLR)
            {
                previous.y = 1;               
                centerChange.x = -1;
            }       
            //previous = (-1,0);
            else
            {
                previous.y = m.y - 1;
                centerChange.x = 1;
            }
        }

        //Shift parents to move display views
        //Update display objects
        for (int i = 0; i < grid.Count; i++)
        {
            List<GameObject> row = grid[i];

            for (int j = 0; j < row.Count; j++)
            {
                //Calculate index of next parent object
                Vector2Int k = new Vector2Int
                {
                    x = (i + previous.x) % m.x,
                    y = (j + previous.y) % m.y
                };

                Transform newParent = grid[k.x][k.y].transform;
                DisplayData data = grid[i][j].GetComponentInChildren<DisplayData>();

                data.translationTransform.parent = newParent;
                data.SetTranslation();
            }
        }

        //Update the index of the moment in center display
        Vector2Int oldIndex = displayObjects.centerIndex;
        displayObjects.MoveCenter(centerChange);

        //If focus has changed call event (probaly in wrong place)
        if(oldIndex != displayObjects.centerIndex)
        {
            Vector2Int i = displayObjects.centerIndex;
            LevelMoment newFocus = LevelData.Instance.moments[i.y][i.x];

            if (MomentDisplayChangeEvent != null) MomentDisplayChangeEvent(newFocus);
        }
        
    }


    public void SetInputCheck(bool value)
    {
        checkInput = value;
    }

    private void ProcessDisplayChanges()
    {
        foreach (List<GameObject> row in displayObjects.displays)
        {
            foreach (GameObject display in row)
            {
                DisplayData data = display.GetComponentInChildren<DisplayData>();

                CheckTranslation(data);
                CheckRotation(data);
            }
        }
    }

    private void CheckTranslation(DisplayData data)
    {
        if (!data.translating) return;               

        Transform transform = data.translationTransform;

        if(Vector3.Distance(transform.localPosition, data.targetTranslation) < deadzone)
        {
            data.translating = false;
            return;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, data.targetTranslation, Time.deltaTime * translationSmoothing);

    }

    private void CheckRotation(DisplayData data)
    {
        if (!data.rotating) return;

        Transform transform = data.rotationTransform;

        float currentRotation = transform.localEulerAngles.y;

        if (Mathf.Abs(currentRotation - data.targetRotation) < deadzone)
        {
            data.rotating = false;
            return;
        }

        Quaternion newRotation = Quaternion.Euler(transform.localEulerAngles.x, data.targetRotation, transform.localEulerAngles.z);
        float rotationSpeed = Time.deltaTime * rotationSmoothing;
        rotationSpeed *= ( (SingletonJsonLoadable<SettingsGameDataPairs>.Instance.data.rotateSpeed + 50) / 100);
        transform.localRotation = Quaternion.Lerp(transform.rotation, newRotation, rotationSpeed);
    }
}
