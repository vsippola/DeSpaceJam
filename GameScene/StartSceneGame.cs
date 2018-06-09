using UnityEngine;

public class StartSceneGame : MonoBehaviour
{
    public string dataPath = "/StreamingAssets/";
    public Transform controllerParent;

    public ObjectGameSingletoneJsonLoadable levels;
    private ConfigureSingletonJsonLoadable<LevelsGameDataPairs> confLevels;

    public ObjectGameSingletoneJsonLoadable gameData;
    private ConfigureSingletonJsonLoadable<DataGameDataPairs> confGameData;

    public ObjectGameSingletoneJsonLoadable gameSettings;
    private ConfigureSingletonJsonLoadable<SettingsGameDataPairs> confGameSettings;

    public GameObject ddolParent;

    public GameObject nextObject;
    public string firstLevel = "";

    private void Awake()
    {
        Configure();
        Load();
    }

    private void Start()
    {
        ConfigureDDOLParent();
        ConfigureMusicObject();
        ConfigureTransitionObject();
        ConfigureExitApplication();

        EndStartup();
    }

    private void Configure()
    {
        ConfigureFileTools();
        ConfigureLevelList();
        ConfigureGameplayData();
        ConfigureGameSettings();
    }

    private void ConfigureFileTools()
    {
        string appPath = Application.dataPath;
        FileTools.ConfigureTools(appPath, dataPath);
    }

    private void ConfigureLevelList()
    {
        confLevels = new ConfigureSingletonJsonLoadable<LevelsGameDataPairs>(levels.path, levels.pathBkp);
        confLevels.Configure();
    }

    private void ConfigureGameplayData()
    {
        confGameData = new ConfigureSingletonJsonLoadable<DataGameDataPairs>(gameData.path, gameData.pathBkp);
        confGameData.Configure();
    }

    private void ConfigureGameSettings()
    {
        confGameSettings = new ConfigureSingletonJsonLoadable<SettingsGameDataPairs>(gameSettings.path, gameSettings.pathBkp);
        confGameSettings.Configure();
    }

    private void Load()
    {
        LoadLevelList();
        LoadGameplayData();
        LoadGameplaySettings();
    }

    private void LoadLevelList()
    {
        confLevels.Load();
    }

    private void LoadGameplayData()
    {
        confGameData.Load();
    }

    private void LoadGameplaySettings()
    {
        confGameSettings.Load();
    }

    private void ConfigureDDOLParent()
    {
        //Find the ddol object if it exists
        string ddolTag = "DDOLParent";
        if ((ddolParent == null) || (!ddolParent.CompareTag(ddolTag))) ddolParent = GameObject.FindGameObjectWithTag(ddolTag);

        if (ddolParent == null) ddolParent = new GameObject("DontDestroyOnLoad");

        //Configure the DDOL parent
        ddolParent.tag = ddolTag;
        DontDestroyOnLoad(ddolParent);
    }

    private void ConfigureMusicObject()
    {
        LoadObjectsByTag("GlobalMusic", true, ddolParent.transform);
    }

    private void ConfigureTransitionObject()
    {
        LoadObjectsByTag("TransitionData", true, ddolParent.transform);
    }

    private void ConfigureExitApplication()
    {
        LoadObjectsByTag("GameExitController", true, ddolParent.transform);
    }

    private void LoadObjectsByTag(string tag, bool active, Transform parent)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);

        if(obj == null)
        {
            obj = (GameObject)Instantiate(
            Resources.Load(tag),
            Vector3.zero,
            Quaternion.identity);

            obj.transform.parent = parent;
        }

        obj.SetActive(active);
    }

    private void EndStartup()
    {
        if(firstLevel != "")
        {
            GameObject obj = GameObject.FindGameObjectWithTag("TransitionData");
            obj.GetComponent<DataTransition>().nextLevelPath = firstLevel;
        }
        nextObject.SetActive(true);
        transform.gameObject.SetActive(false);
    }
}
