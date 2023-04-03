using System.Collections.Generic;
using CombatSystem.Abilities;
using UnityEngine;

namespace CombatSystem.Selection
{
    public class AbilitySelectionManager : MonoBehaviour
    {
        private SelectionAction CurrentActionSelection;
        private bool isEnabled = false;

        [SerializeField] private Transform ButtonsRoot;
        [SerializeField] private Transform VisibleButtonRoot;
        [SerializeField] private AbilityButton AbilityButtonPrefab;
        [SerializeField] private int Capacity = 20;

        private List<AbilityButton> Buttons = new();
        
        private void Awake()
        {
            SelectionStackManager.OnLayerBecomeActive.AddListener(OnLayerActive);
            SelectionStackManager.ClearStack.AddListener(HideAction);
            HideAction();
            
            for(int i = 0; i < Capacity; i++)
                createButton();
        }


        private void OnLayerActive(SelectionLayer Layer)
        {
            if (Layer is SelectionAction Action)
                ShowAction(Action);
            else
                HideAction();
        }

        private void HideAction()
        {
            isEnabled = false;
            ButtonsRoot.gameObject.SetActive(false);
        }

        private void createButton()
        {
            var button = Instantiate(AbilityButtonPrefab, ButtonsRoot);
            button.SetIndex(this, Buttons.Count);
            Buttons.Add(button);
        }

        private void ShowAction(SelectionAction Action)
        {
            isEnabled = true;
            CurrentActionSelection = Action;
            ButtonsRoot.gameObject.SetActive(true);

            for(int i = Buttons.Count; i < Action.Abilities.Count; i++)
                createButton();
            
            int index = 0;
            for (; index < Action.Abilities.Count; index++)
                ShowButton(Buttons[index], Action.Abilities[index]);

            for (; index < Buttons.Count; index++) 
                HideButton(Buttons[index]);
        }

        private void ShowButton(AbilityButton button, Ability ability)
        {
            button.transform.SetParent(VisibleButtonRoot);
            button.Initialize(ability);
        }

        private void HideButton(AbilityButton button)
        {
            button.transform.SetParent(ButtonsRoot);
            button.Hide();
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
