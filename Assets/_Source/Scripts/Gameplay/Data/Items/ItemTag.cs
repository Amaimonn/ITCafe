namespace ITCafe.Data.Items
{
    public enum ItemTag
    {
        Undefined = 0,
        Muffin = 1,
        Croissant = 2,
        Onigiri = 3,
        Fries = 4,
        ChocolateDonut = 5,
        Bun = 7,
        // HotDogBun = 8,
        Patty = 9,
        Cheese = 10,
        Sausage = 11,
        Lettuce = 12,
        Rice = 13,
        Salmon = 14,
        Egg = 15,
        Tomato = 16,
        Dough = 17,
        SoupBase = 18,
        Noodles = 19,
        Meatloaf = 20,
        BasePizzaRaw = 21,
        HuntersPizzaRaw = 22,
        RamenRaw = 23,
        Cookie = 30,

        // Combinations
        SimpleCombination = 5000,
        BurgerCombination = 5001,
        HotDogCombination = 5002,

        // Menu Items
        HotDog = 10_000,
        Burger = 10_001,
        EggFried = 10_002, // also Processed
        Sushi = 10_003,
        Ramen = 10_004,
        MargheritaPizza = 10_005,
        HuntersPizza = 10_006,
        Salad = 10_007,
        TomatoSoup = 10_008,
        Coffee = 10_009,

        // Special
        Tray = 15_000,
        Plate = 15_001,
        SushiTable = 15_002,
        Bowl = 15_003,
        PlasticCup = 15_004,
        
        // Processed
        PattyFried = 20_000,
        SausageFried = 20_001,
        SausageSliced = 20_002,
        SausageFriedSliced = 20_003,
        LettuceSliced = 20_004,
        TomatoSliced = 20_005,
        SalmonSliced = 20_006,
        RiceBoiled = 20_007,
        MeatloafSliced = 20_008
    }
}