namespace ITCafe.Gameplay.Data
{
    public abstract class Model<T>
    {
        public T State { get; protected set; }

        public Model(T state)
        {
            State = state;
        }
    }
}