using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

[Serializable]
[XmlType("data")]
public class GameMetaData {

    #region Instance:
    private static GameMetaData _instance;
    public static GameMetaData instance
    {
        get
        {
            if (_instance == null)
                _instance = new GameMetaData();

            return _instance;
        }

        set
        {
            _instance = value;
        }
    }

    public GameMetaData()
    {
        //AddBooster(BoosterItem.canistrey, Parse.instance.placements.startBoost.canistrey, false);
    }

    #endregion

    #region Variable's
    [XmlElement("modes")]
    public List<ModeClass> modes = new List<ModeClass>();
    #endregion
}
