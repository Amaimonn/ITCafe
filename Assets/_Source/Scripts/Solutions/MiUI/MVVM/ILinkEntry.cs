namespace MiUI.MVVM
{
    /// <summary>
    /// Шаблон, имитирующий ссылку для открытия окна.
    /// </summary>
    public interface ILinkEntry
    {
        public void Open();
        public void Close();
    }
}