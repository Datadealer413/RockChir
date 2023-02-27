using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class FontManager : MonoBehaviour
    {
        public static int uniformSmallFontSize { get; private set; }
        public static int uniformHeader3FontSize { get; private set; }
        public static int uniformHeader2FontSize { get; private set; }

        [SerializeField] private Text templateSmallText, templateMenuText;

        [SerializeField] private List<GameObject> smallFonts = new List<GameObject>();
        [SerializeField] private List<GameObject> menuFonts = new List<GameObject>();

        public void SetFontUniforms()
        {
            smallFonts.AddRange(GameObject.FindGameObjectsWithTag("RegularText"));
            menuFonts.AddRange(GameObject.FindGameObjectsWithTag("MenuText"));

            uniformSmallFontSize = templateSmallText.cachedTextGenerator.fontSizeUsedForBestFit;
            uniformHeader3FontSize = templateMenuText.cachedTextGenerator.fontSizeUsedForBestFit;
            uniformHeader2FontSize = templateMenuText.cachedTextGenerator.fontSizeUsedForBestFit;
            
            for (int i = 0; i < smallFonts.Count; i++)
            {
                if(smallFonts[i] != null)
                {
                    Text txt = smallFonts[i].GetComponent<Text>();
                    txt.resizeTextForBestFit = false;
                    txt.fontSize = uniformSmallFontSize;
                }
            }

            for (int i = 0; i < menuFonts.Count; i++)
            {
                if (menuFonts[i] != null)
                {
                    Text txt = menuFonts[i].GetComponent<Text>();
                    txt.resizeTextForBestFit = false;
                    txt.fontSize = uniformHeader2FontSize;
                }
            }
        }
    }
}