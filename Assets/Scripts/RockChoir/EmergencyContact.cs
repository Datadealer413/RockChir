using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class EmergencyContact : MonoBehaviour
    {
        public InputField firstName;
        public InputField lastName;
        public InputField description;
        public InputField phoneNumber;
        public JSONObject originalData;
        [SerializeField, Range(0, 1)] private float heightInPercent;
        [SerializeField] private RectTransform rect;

        public void Setup(JSONObject info)
        {
            originalData = info;
            firstName.text = !string.IsNullOrEmpty(info[0]["First_Name__c"].str) ? info[0]["First_Name__c"].str.ToUpper() : string.Empty;
            lastName.text = !string.IsNullOrEmpty(info[0]["Last_Name__c"].str) ? info[0]["Last_Name__c"].str.ToUpper() : string.Empty;
            phoneNumber.text = !string.IsNullOrEmpty(info[0]["Phone__c"].str) ? info[0]["Phone__c"].str.ToUpper() : string.Empty;
            description.text = !string.IsNullOrEmpty(info[0]["Description__c"].str) ? info[0]["Description__c"].str.ToUpper() : string.Empty;

            rect.sizeDelta = new Vector2(transform.parent.GetComponent<RectTransform>().rect.width, Screen.height * heightInPercent);
        }
    }
}