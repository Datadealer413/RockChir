using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockChoir
{
    public class PersonalInfoSubsection : MonoBehaviour, IViewable
    {
        [SerializeField] private PersonalInfoManager manager;

        public bool viewActive { get { return manager.viewActive; } }

        // Explicitly pass to parent to handle
        public void SetView(bool viewActive)
        {
            if (viewActive)
            {
                manager.SetView(viewActive);
            }
        }
    }
}
