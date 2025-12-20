using R3;

namespace ITCafe.Gameplay.Data
{
    /// <summary>
    /// Модель настроек приложения.
    /// </summary>
    public class SettingsModel : Model<SettingsState>
    {
        public readonly ReactiveProperty<int> Sensitivity; 
        public readonly ReactiveProperty<bool> VSync;
        public readonly ReactiveProperty<int> FPS;

        public SettingsModel(SettingsState state) : base(state)
        {
            Sensitivity = new ReactiveProperty<int>(state.Sensitivity);
            VSync = new ReactiveProperty<bool>(state.VSync);
            FPS = new ReactiveProperty<int>(state.FPS);
        }

        /// <summary>
        /// Заносит текущие значения из модели в состояние (для сохранения).
        /// </summary>
        public void ApplyToState()
        {
            State.Sensitivity =  Sensitivity.Value;
            State.VSync = VSync.Value;
            State.FPS = FPS.Value;
        }
    }
}
