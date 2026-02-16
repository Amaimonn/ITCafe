namespace ITCafe.Environment
{
    public interface ICompositeComponent
    {
        public bool TryGetCachedComponent<T>(out T component);
    }
}