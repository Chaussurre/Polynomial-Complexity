using CombatSystem.Entities;

namespace CombatSystem.Actions
{
    public class MapActionInstantiateView : MapActionView
    {
        private CombatEntityView View;
        
        private bool hidden;

        public MapActionInstantiateView(CombatEntity entity)
        {
            View = entity.View;
            hidden = true;
            View.gameObject.SetActive(false);
        }
        
        public override bool IsOver => !hidden; 
        public override void Update(float deltaTime)
        {
            View.gameObject.SetActive(true);
            hidden = false;
        }
    }
}