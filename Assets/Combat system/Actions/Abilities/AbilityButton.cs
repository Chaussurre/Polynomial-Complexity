using CombatSystem.Abilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CombatSystem.Selection
{
    public class AbilityButton : MonoBehaviour
    {
        [SerializeField] private Button Button;
        [SerializeField] private Image Image;
        [SerializeField] private TMP_Text Text;

        private AbilitySelectionManager manager;
        
        private int index;
        
        public void SetIndex(AbilitySelectionManager manager, int index)
        {
            this.manager = manager;
            this.index = index;
            Button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            manager.Select(index);
        }

        public void Initialize(Ability ability)
        {
            Image.sprite = ability.Icon;
            Text.text = ability.AbilityName;
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
