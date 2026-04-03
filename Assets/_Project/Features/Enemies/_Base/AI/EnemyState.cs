namespace InsideVentura.AI
{
    public abstract class EnemyState
    {
        protected EnemyBrain Brain;
        protected Enemy EnemyInstance;

        public void Init(EnemyBrain brain, Enemy enemy)
        {
            Brain = brain;
            EnemyInstance = enemy;
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
    }
}