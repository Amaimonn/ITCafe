using UnityEngine;

namespace ITCafe
{
    public static class Constants
    {
#region Outline config
        public const float OUTLINE_WIDTH = 7f;
        public static Color OUTLINE_COLOR = new (1f, 10.6283679f, 0.45f, 1);
#endregion

#region Registration Keys
        public const string CLIENT_SEATS = nameof(CLIENT_SEATS);
        public const string CLIENT_ORDER_PLACES = nameof(CLIENT_ORDER_PLACES);
#endregion
        public const string MAIN_MENU_EXIT_SIGNAL =  nameof(MAIN_MENU_EXIT_SIGNAL);
        public const string GAMEPLAY_EXIT_SIGNAL =  nameof(GAMEPLAY_EXIT_SIGNAL);
    }
}