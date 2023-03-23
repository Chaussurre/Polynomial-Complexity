using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Selection;
using UnityEngine;

namespace CombatSystem
{
    public class AbilitySelectionManager : MonoBehaviour
    {
        private SelectionAction CurrentActionSelection;
        private bool isEnabled = false;

        [SerializeField] private GameObject Buttons;
        
        private void Awake()
        {
            SelectionStackManager.OnLayerBecomeActive.AddListener(OnLayerActive);
            SelectionStackManager.ClearStack.AddListener(HideAction);
        }


        void OnLayerActive(SelectionLayer Layer)
        {
            if (Layer is SelectionAction Action)
                ShowAction(Action);
            else
                HideAction();
        }

        private void HideAction()
        {
            isEnabled = false;
            Buttons.SetActive(false);
        }

        void ShowAction(SelectionAction Action)
        {
            isEnabled = true;
            CurrentActionSelection = Action;
            Buttons.SetActive(true);
        }

        /// <summary>
        /// When active, will select the ability with corresponding index
        /// </summary>
        public void Select(int index)
        {
            if (!isEnabled) return;
            
            CurrentActionSelection.Select(index);
        }
    }
}
