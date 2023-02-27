using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class DownloadedObject : MonoBehaviour
    {
        public JSONObject rawData { get; protected set; }
        [SerializeField, Range(0,1)] protected float heightInPercent;

        private void Awake()
        {
            Text[] textObjs = GetComponentsInChildren<Text>();

            for(int i=0; i < textObjs.Length; i++)
            {
                if(textObjs[i].tag == "RegularText")
                {
                    textObjs[i].resizeTextForBestFit = false;
                    textObjs[i].fontSize = FontManager.uniformSmallFontSize;
                }
                else if(textObjs[i].tag == "MenuText")
                {
                    textObjs[i].resizeTextForBestFit = false;
                    textObjs[i].fontSize = FontManager.uniformHeader3FontSize;
                }
                else
                {
                    // Do nothing bruv
                }
            }
        }
    }
}