using UnityEngine;

public class LocalizationStatic3DText : MonoBehaviour
{
    string key = "";

    TextMesh thisText;

    void Start()
    {
        thisText = this.GetComponent<TextMesh>();

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
