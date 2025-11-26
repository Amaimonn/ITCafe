namespace ITCafe.Environment
{
    public enum ItemPartTag
    {
        BurgerBun,
        HotDogBun,
        Patty, 
        Cheese,
        Sausage
    }
    
    public interface IItemPart
    {
        public bool CanBeUsedWith(int itemHash); // IItemPart mb? + public dict<ItemPartTag, int "amount"> { get; }
    }
}