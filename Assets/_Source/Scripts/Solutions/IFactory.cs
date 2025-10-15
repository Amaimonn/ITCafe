namespace ITCafe.Solutions
{
    public interface IFactory<out T>
    {
        public T Create();
    }
}