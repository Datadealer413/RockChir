using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockChoir
{
    public class BillingHistoryView : MonoBehaviour, IViewable
    {
        [SerializeField] private ServiceManager serviceManager;
        [SerializeField] private ResponsePanel responsePanel;
        [SerializeField] private GameObject billingHistory;
        [SerializeField] private GameObject rootTransactionsObj, transactionsTemplate;
        private bool transactionsReceived = false;

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
                TopMenuManager.managerInstance.menuTitle = "BILLING HISTORY";
                billingHistory.SetActive(true);
                GetBillingHistory();
            }
            else
            {
                billingHistory.SetActive(false);
            }
        }

        public void GetBillingHistory()
        {
            if (!transactionsReceived)
            {
                responsePanel.response = "LOADING";
                transactionsReceived = true;
                StartCoroutine(GetBillingHistoryProcess());
            }
        }

        private IEnumerator GetBillingHistoryProcess()
        {
            JSONObject response = null;
            yield return StartCoroutine(serviceManager.MakeRequest(RequestType.BillingHistory, value => response = value as JSONObject));

            if(!response.IsNull && response.HasField("Records") && response["Records"].Count > 0)
            {
                responsePanel.visible = false;
            }
            else
            {
                responsePanel.response = "NO TRANSACTIONS FOUND";
            }
        }
    }
}
