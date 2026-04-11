using DevKit.UI.MVVM.Bases;
using ITCafe.Shared;
using UnityEngine;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class CreditsView : AttachableToolkitWindow<CreditsViewModel>
    {
        [Header("SFX")]
        [SerializeField] private SfxData _closeClickSfx;
        
        [Inject] private readonly AudioPlayer _audioPlayer;
        
        protected override void OnClosing()
        {
            PlayCloseSfx();
            base.OnClosing();
        }
        
        private void PlayCloseSfx()
        {
            if (_closeClickSfx.IsValid)
                _audioPlayer.GetSfxBuilder().Play(_closeClickSfx);
        }
    }
}