using ITCafe.Gameplay.Data;
using UnityEngine;

namespace ITCafe
{
    public static class Constants
    {
#region Outline config
        public const float OUTLINE_WIDTH = 7f;
        public static Color OUTLINE_COLOR = new(1f, 10.6283679f, 0.45f, 1);
#endregion

#region Registration Keys
        public const string CLIENT_SEATS = nameof(CLIENT_SEATS);
        public const string CLIENT_ORDER_PLACES = nameof(CLIENT_ORDER_PLACES);
#endregion

#region Scene signals
        public const string MAIN_MENU_EXIT_SIGNAL = nameof(MAIN_MENU_EXIT_SIGNAL);
        public const string GAMEPLAY_EXIT_SIGNAL = nameof(GAMEPLAY_EXIT_SIGNAL);
        public const string RESTART_GAMEPLAY_SIGNAL = nameof(RESTART_GAMEPLAY_SIGNAL);
#endregion

#region Item Keys
        [ItemKey] public const string BURGER = nameof(BURGER);
        [ItemKey] public const string ONIGIRI = nameof(ONIGIRI);
        [ItemKey] public const string HOT_DOG = nameof(HOT_DOG);
        [ItemKey] public const string DONUT = nameof(DONUT);
        [ItemKey] public const string FRIES = nameof(FRIES);
#endregion
    }
}