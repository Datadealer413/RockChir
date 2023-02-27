using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class DownloadableSong : DownloadedObject
    {
        [SerializeField] private Text songTitle;
        [SerializeField] private Image background, openCloseButton;
        [SerializeField] private RectTransform rect;
        [SerializeField] private Dropdown dropdown;
        [SerializeField] private RectTransform downloadBtn;
        
        public void SetSong(JSONObject json, Color bgColor, Color txtColor)
        {
            rect.sizeDelta = new Vector2(Screen.width, Screen.height * heightInPercent);
            background.color = bgColor;
            songTitle.color = txtColor;
            openCloseButton.color = txtColor;

            rawData = json;
            songTitle.text = json["Name"].str;

            SetSongOptions();
        }

        private void SetSongOptions()
        {
            if (rawData.HasField("option") && rawData["option"].Count > 0)
            {
                for(int i=0; i < rawData["option"].Count; i++)
                {
                    dropdown.options.Add(new Dropdown.OptionData(rawData["option"][i]["optionName"].str.ToUpper()));
                }
            }
            else
            {
                dropdown.gameObject.SetActive(false);
                downloadBtn.anchorMax = new Vector2(downloadBtn.anchorMax.x, 1);
                downloadBtn.anchorMin = new Vector2(downloadBtn.anchorMin.x, 0);
            }
        }

        public void Download()
        {
            if(rawData.HasField("option") && rawData["option"].Count > 0)
            {
                if(dropdown.value != 0)
                {
                    
                    transform.GetComponentInParent<SongViewManager>().DownloadSong(rawData["option"][dropdown.value - 1]["optionId"].str, rawData["FileName"].str);
                }
                else
                {
                    Debug.Log("None selected");
                }
            }
        }
    }
}