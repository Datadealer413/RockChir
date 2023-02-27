using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 0649

namespace RockChoir
{
    public class PersonalInfoManager : MonoBehaviour, IViewable
    {
        [SerializeField] private ServiceManager serviceManager;
        [SerializeField] private GameObject menu, personalInfo, EmergencyInfo, templateEmergencyContact, rootObjEmergencyContact;
        [SerializeField] private InputField firstName, lastName, birthday, gender, email, addressL1, town, county, postCode, phoneNumber, voiceType, medicalConditions;
        private List<EmergencyContact> emergencyContacts = new List<EmergencyContact>();
        private bool hasEmergencyInfo = false;

        public bool viewActive
        {
            get
            {
                return menu.activeSelf;
            }
        }

        private void Awake()
        {
            ServiceManager.loginComplete += SetPersonalInfo;
        }

        private void SetPersonalInfo()
        {
            firstName.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["FirstName"].str) ? serviceManager.contactInfo["Records"][0]["FirstName"].str.ToUpper() : string.Empty;
            lastName.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["LastName"].str) ? serviceManager.contactInfo["Records"][0]["LastName"].str.ToUpper() : string.Empty;
            email.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["Email"].str) ? serviceManager.contactInfo["Records"][0]["Email"].str.ToUpper() : string.Empty;
            birthday.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["Birthdate"].str) ? serviceManager.contactInfo["Records"][0]["Birthdate"].str.ToUpper() : string.Empty;
            gender.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["Gender__c"].str) ? serviceManager.contactInfo["Records"][0]["Gender__c"].str.ToUpper() : string.Empty;
            addressL1.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["MailingStreet"].str) ? serviceManager.contactInfo["Records"][0]["MailingStreet"].str.ToUpper() : string.Empty;
            town.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["MailingCity"].str) ? serviceManager.contactInfo["Records"][0]["MailingCity"].str.ToUpper() : string.Empty;
            county.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["MailingState"].str) ? serviceManager.contactInfo["Records"][0]["MailingState"].str.ToUpper() : string.Empty;
            postCode.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["MailingPostalCode"].str) ? serviceManager.contactInfo["Records"][0]["MailingPostalCode"].str.ToUpper() : string.Empty;
            phoneNumber.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["Phone"].str) ? serviceManager.contactInfo["Records"][0]["Phone"].str.ToUpper() : string.Empty;
            voiceType.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["Voice_Type__c"].str) ? serviceManager.contactInfo["Records"][0]["Voice_Type__c"].str.ToUpper() : string.Empty;
            medicalConditions.text = !string.IsNullOrEmpty(serviceManager.contactInfo["Records"][0]["Medical_Conditions__c"].str) ? serviceManager.contactInfo["Records"][0]["Medical_Conditions__c"].str.ToUpper() : string.Empty;
        }
        
        public void SetView(bool viewActive)
        {
            if (viewActive)
            {
                TopMenuManager.managerInstance.visible = true;
                TopMenuManager.managerInstance.menuTitle = "Personal Info";
                menu.SetActive(true);
                personalInfo.SetActive(false);
                EmergencyInfo.SetActive(false);
            }
            else
            {
                menu.SetActive(false);
                personalInfo.SetActive(false);
                EmergencyInfo.SetActive(false);
            }
        }

        public void SavePersonalInfo()
        {
            if (true)
            {
                StartCoroutine(SavePersonalInfoProcess());
            }
        }

        private IEnumerator SavePersonalInfoProcess()
        {
            JSONObject response = null;
            yield return StartCoroutine(serviceManager.MakeRequest(RequestType.UpdatePersonalInfo, value => response = value as JSONObject, new string[] {
                firstName.text, lastName.text, email.text, county.text, addressL1.text, birthday.text, town.text, "", postCode.text,
                phoneNumber.text, serviceManager.contactInfo["Records"][0]["Id"].str, voiceType.text, gender.text} ));

#if DEBUG || DEVELOPMENT_BUILD
            Debug.Log(response);
#endif

        }

        public void GetEmergencyInfo()
        {
            TopMenuManager.managerInstance.menuTitle = "EMERGENCY INFO";

            if (!hasEmergencyInfo)
            {
                StartCoroutine(GetEmergencyProcess());
            }
        }
        
        private IEnumerator GetEmergencyProcess()
        {
            hasEmergencyInfo = true;
            JSONObject response = null;
            yield return StartCoroutine(serviceManager.MakeRequest(RequestType.EmergencyContact, value => response = value as JSONObject));

            if (!response.IsNull)
            {
                for(int i=0; i < response["Records"].Count; i++)
                {
                    GameObject obj = Instantiate(templateEmergencyContact, rootObjEmergencyContact.transform);
                    EmergencyContact newContact = obj.GetComponent<EmergencyContact>();
                    newContact.Setup(response[i]);
                    emergencyContacts.Add(newContact);
                }
            }
        }
    }
}
