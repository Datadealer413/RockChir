using UnityEngine;
using UnityEngine.UI;

public class ButtonSwitch : MonoBehaviour {
    
    public Image songLibraryBtn, downloadsLibraryBtn;
    public Text songLibraryTxt, downloadsLibraryTxt;
    public Color activeColor, inactiveColor;

	public void ChangeButtonView(bool showSongLibrary)
    {
        if (showSongLibrary)
        {
            songLibraryBtn.color = activeColor;
            songLibraryTxt.color = Color.black;
            downloadsLibraryBtn.color = inactiveColor;
            downloadsLibraryTxt.color = Color.white;
        }
        else
        {
            songLibraryBtn.color = inactiveColor;
            songLibraryTxt.color = Color.white;
            downloadsLibraryBtn.color = activeColor;
            downloadsLibraryTxt.color = Color.black;
        }
    }
}
