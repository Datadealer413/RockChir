using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockChoir
{
    public class Transaction : MonoBehaviour
    {
        [SerializeField] private Text transactionNo, date, amount;
        private JSONObject info;

        public void SetTransaction(JSONObject _info)
        {
            info = _info;

            transactionNo.text = info[0][""].str;
        }
    }
}