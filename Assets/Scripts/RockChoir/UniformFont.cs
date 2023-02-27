using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class UniformFont : MonoBehaviour
    {
        private void Awake()
        {
            Text[] texts = GetComponentsInChildren<Text>();

            for(int i=0; i < texts.Length; i++)
            {
                texts[i].resizeTextForBestFit = false;
                texts[i].fontSize = FontManager.uniformSmallFontSize;
            }
        }
    }
}