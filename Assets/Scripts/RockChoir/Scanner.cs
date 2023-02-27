using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#pragma warning disable 0649

namespace RockChoir
{
    public class Scanner : MonoBehaviour, IViewable
    {
        [SerializeField] private Text scannedTxt;
        [SerializeField] private GameObject mainCamera, deviceCamera;
        [SerializeField] private QRCodeDecodeController qrDecoder;
        [SerializeField] private AudioSource audioSource;

        public bool viewActive
        {
            get
            {
                return gameObject.activeSelf;
            }
        }

        public void SetView(bool viewActive)
        {
            if (viewActive)
            {
                TopMenuManager.managerInstance.visible = true;
                TopMenuManager.managerInstance.menuTitle = "SCAN";
            }

            gameObject.SetActive(viewActive);
            mainCamera.SetActive(!viewActive);
            deviceCamera.SetActive(viewActive);
        }

        void Start()
        {
            scannedTxt.text = "CONFIRMED " + SaveData.saveData.scans.Count.ToString();
            qrDecoder.onQRScanFinished += onScanFinished;
        }

        public void onScanFinished(string str)
        {
            if (SaveData.saveData.AddScan(str))
            {
                audioSource.Play();
                TopMenuManager.managerInstance.Scanned();
                scannedTxt.text = "CONFIRMED " + SaveData.saveData.scans.Count.ToString();
            }
            
            qrDecoder.Reset();
        }
    }
}