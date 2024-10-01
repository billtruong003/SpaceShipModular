using TMPro;
using UnityEngine;

public class LinkHandler : MonoBehaviour
{

    public void OpenLink(string url)
    {
        Application.OpenURL(url);
    }

    public void OpenLink(TMP_Text text)
    {
        Application.OpenURL(text.text);
    }
}
