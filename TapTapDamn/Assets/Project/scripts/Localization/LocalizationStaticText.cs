using UnityEngine;
using UnityEngine.UI;

public class LocalizationStaticText : MonoBehaviour
{
    string key = "";

    Text thisText;

    void Start()
    {
        thisText = this.GetComponent<Text>();

        key = thisText.text;

        thisText.text = Localization.GetText(key);
        Events.instance.AddListener<EventSetLang>(UpdateLang);
    }

    void OnDestroy()
    {
        Events.instance.RemoveListener<EventSetLang>(UpdateLang);
    }


    void UpdateLang(EventSetLang res)
    {
        thisText.text = Localization.GetText(key);
    }

}
