namespace Scripts.Game.Event {
    public abstract class EntityEvent : Script.Event.Event {
        
        internal Entity entity;
        
        public EntityEvent(Entity entity) {
            this.entity = entity;
        }
    }
}