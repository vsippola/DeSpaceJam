using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerSelectLevelMenu : MonoBehaviour
{
    public RectTransform content;
    public GameObject levelSelectButton;

    public List<GameObject> buttons;

    private SingletonJsonLoadable<LevelsGameDataPairs> levelList;

    private void OnEnable()
    {
        if (content.childCount == 0) LoadButtons();
    }

    private void SetupLevelList()
    {
        levelList = SingletonJsonLoadable<LevelsGameDataPairs>.Instance;
    }

    private void LoadButtons()
    {
        if (content == null) return;
        if (levelSelectButton == null) return;
        if (levelList == null) SetupLevelList();

        List<RawLevel> levels = levelList.data.levels;

        foreach (RawLevel level in levels)
        {
            GameObject newBtn = Instantiate(levelSelectButton);

            ControllerButtonSelectLevel cont = newBtn.GetComponentInChildren<ControllerButtonSelectLevel>();
            cont.levelName = level.name;
            cont.levelpath = level.path;

            StartGameplayButtonMenu startGameply = newBtn.GetComponentInChildren<StartGameplayButtonMenu>();
            startGameply.path = level.path;

            newBtn.transform.SetParent(content, false);
            newBtn.SetActive(true);
        }

        content.sizeDelta = new Vector2(content.sizeDelta.x, 80 * (levels.Count+1));
    }
}
