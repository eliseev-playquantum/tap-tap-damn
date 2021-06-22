using UnityEngine;

public class GameSettings
{
    #region Debug

    //public static bool InfinityHP
    //{
    //    get
    //    {
    //        if (!_InfinityHPInit)
    //        {
    //            _InfinityHPInit = true;
    //            _InfinityHP = PlayerPrefsX.GetBool("InfinityHP", false);
    //        }

    //        return _InfinityHP;
    //    }
    //    set
    //    {
    //        _InfinityHPInit = true;
    //        _InfinityHP = value;

    //        PlayerPrefsX.SetBool("InfinityHP", _InfinityHP);
    //        save();
    //    }
    //}

//   public static bool NoAds
//   {
//       get
//       {
//           return PlayerPrefsX.GetBool("NoAds", false);
//       }
//       set
//       {
//           PlayerPrefsX.SetBool("NoAds", value);
//           save();
//       }
//   }
//
   #endregion
//
//
//   public static string QualityLevel
//   {
//       get
//       {
//           return PlayerPrefs.GetString("QualityLevel", "");
//       }
//       set
//       {
//           PlayerPrefs.SetString("QualityLevel", value);
//           save();
//       }
//   }


    #region Settings
    /**
	 * Language
	 */
    public static string language
    {
        get
        {
            return PlayerPrefs.GetString("language", "");
        }
        set
        {
            PlayerPrefs.SetString("language", value);
            save();
        }
    }

    /**
     * Громкость звуков
     */
    const float soundVolumeDefult = 1f;
    public static float soundVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("soundVolume", soundVolumeDefult);
        }
        set
        {
            PlayerPrefs.SetFloat("soundVolume", value);
            save();
        }
    }

    public static float soundVolumeOld
    {
        get
        {
            return PlayerPrefs.GetFloat("soundVolumeOld", soundVolumeDefult);
        }
        set
        {
            PlayerPrefs.SetFloat("soundVolumeOld", value);
            save();
        }
    }

    /**
     * Громкость музыки
     */
    const float musicVolumeDefult = 1f;
    public static float musicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat("musicVolume", musicVolumeDefult);
        }
        set
        {
            PlayerPrefs.SetFloat("musicVolume", value);
            save();
        }
    }
    public static float musicVolumeOld
    {
        get
        {
            return PlayerPrefs.GetFloat("musicVolumeOld", soundVolumeDefult);
        }
        set
        {
            PlayerPrefs.SetFloat("musicVolumeOld", value);
            save();
        }
    }

    #endregion

    public static void save()
    {
        //PlayerPrefs.Save();
    }
}

