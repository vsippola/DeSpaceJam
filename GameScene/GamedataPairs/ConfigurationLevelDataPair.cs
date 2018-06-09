using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
class ConfigurationLevelDataPair : GameDataPairs
{
    public LevelMoment startingMoment;
    public string levelAsString;
    public string nextLevelPath;
}

