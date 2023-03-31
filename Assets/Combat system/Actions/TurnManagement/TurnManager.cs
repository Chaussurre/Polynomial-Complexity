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

        public static UnityEvent<CombatEntity> StartEntityTurn = new();
        public static UnityEvent<Vector2Int> NextTurnStep = new();
        public static UnityEvent OnEndTurn = new();

        #endregion

        [SerializeField] private bool AllowUnselect;
        
        [SerializeField] private List<TurnStep> Steps = new();

        private TurnStepSelectionLayer turnStepSelectionLayer;
        private CombatEntity CurrentEntity;
        private int currentStep;
        
        private void Awake()
        {
            StartEntityTurn.AddListener(StartTurn);
            NextTurnStep.AddListener(NextStep);
            SelectionStackManager.OnCancel.AddListener(OnCancelTurnStep);

            turnStepSelectionLayer = new();
        }

        private void StartTurn(CombatEntity entity)
        {
            currentStep = -1;
            CurrentEntity = entity;
            NextStep(BattleMap.GetEntityPos(entity));
        }

        private void NextStep(Vector2Int position)
        {
            if (!CurrentEntity) return;

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
                    CurrentEntity.ResetActionManager(step.StepId);
                if(CurrentEntity.DoAction(position, step.StepId))
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
                StartTurn(CurrentEntity);
                return;
            }
            
            CurrentEntity = null;
            SelectionStackManager.ClearStack.Invoke();
        }
        
        private void EndTurn()
        {
            CurrentEntity.EndTurn();
            CurrentEntity = null;
            SelectionStackManager.ClearStack.Invoke();
            OnEndTurn.Invoke();
        }
    }
}