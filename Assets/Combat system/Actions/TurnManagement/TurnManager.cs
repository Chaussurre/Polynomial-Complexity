using System;
using System.Collections.Generic;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;
using UnityEngine.Events;

namespace CombatSystem.Entities
{
    [Serializable]
    public struct TurnStep
    {
        public string StepId;
        public bool required;
        public bool ResetOnStep;
    }

    public class TurnStepSelectionLayer : SelectionLayer
    {
        public void Cancel() { }
    }
    
    public class TurnManager : MonoBehaviour
    {
        #region Events

        public static UnityEvent<TurnAgent> StartAgentTurn = new();
        public static UnityEvent NextTurnStep = new();
        public static UnityEvent OnEndTurn = new();

        public static UnityEvent<TurnAgent> AddAgent = new();
        public static UnityEvent<TurnAgent> RemoveAgent = new();
        

        #endregion

        [SerializeField] private bool AllowUnselect;
        
        [SerializeField] private List<TurnStep> Steps = new();

        private TurnStepSelectionLayer turnStepSelectionLayer;
        private TurnAgent CurrentTurnAgent;
        private int currentStep;
        
        private void Awake()
        {
            StartAgentTurn.AddListener(StartTurn);
            NextTurnStep.AddListener(NextStep);
            SelectionStackManager.OnCancel.AddListener(OnCancelTurnStep);

            turnStepSelectionLayer = new();
        }

        private void StartTurn(TurnAgent turnAgent)
        {
            currentStep = -1;
            CurrentTurnAgent = turnAgent;
            NextStep();
        }

        private void NextStep()
        {
            if (!CurrentTurnAgent) return;

            SelectionStackManager.AddLayer.Invoke(turnStepSelectionLayer);

            currentStep++;
            for (;; currentStep++)
            {
                if (currentStep == Steps.Count)
                {
                    EndTurn();
                    return;
                }

                var step = Steps[currentStep];

                if (step.ResetOnStep)
                    CurrentTurnAgent.ResetActionManager(step.StepId);
                if(CurrentTurnAgent.DoAction(step.StepId))
                    break;
            }
        }

        private void OnCancelTurnStep(SelectionLayer layer)
        {
            if (layer is not TurnStepSelectionLayer)
                return;

            if (currentStep > 0)
            {
                currentStep--;
                MapSelectionManager.OnCancel.Invoke();
                return;
            }

            if (!AllowUnselect)
            {
                StartTurn(CurrentTurnAgent);
                return;
            }
            
            CurrentTurnAgent = null;
            SelectionStackManager.ClearStack.Invoke();
        }
        
        private void EndTurn()
        {
            CurrentTurnAgent.EndTurn();
            CurrentTurnAgent = null;
            SelectionStackManager.ClearStack.Invoke();
            OnEndTurn.Invoke();
        }
    }
}