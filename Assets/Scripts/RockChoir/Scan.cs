using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class Scan : MonoBehaviour
    {
        [SerializeField] private RectTransform rect;
        [SerializeField] private float heightInPercent;
        [SerializeField] private Text textObj;
        [SerializeField] private Image bg;

        // Called from animation
        public void DestroyMe()
        {
            Destroy(gameObject);
        }
        
        public void SetupScanObject(string _memberId, Color bgColor, Color fontColor)
        {
            rect.sizeDelta = new Vector2(Screen.width, Screen.height * heightInPercent);

            gameObject.name = _memberId;

            textObj.text = _memberId;
            textObj.color = fontColor;
            bg.color = bgColor;
        }
    }
}
