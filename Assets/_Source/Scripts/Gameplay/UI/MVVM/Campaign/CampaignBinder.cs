using System;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using ITCafe.Data.Campaign;
using ITCafe.Infrastructure.Saves;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class CampaignBinder : SimpleAttachBinder<CampaignView, CampaignViewModel>
    {
        private readonly CampaignDataModelFactory _campaignDataModelFactory;
        private readonly ISaveStateProvider _saveStateProvider;

        public CampaignBinder(Func<CampaignViewModel> viewModelFactory,
            CampaignDataModelFactory campaignDataModelFactory,
            ISaveStateProvider saveStateProvider,
            IRootUIBinder rootUIBinder,
            CampaignView viewPrefab) :
            base(viewModelFactory, rootUIBinder, viewPrefab)
        {
            _campaignDataModelFactory = campaignDataModelFactory;
            _saveStateProvider = saveStateProvider;
        }

        protected override CampaignViewModel GetViewModel()
        {
            var viewModel = base.GetViewModel();
            
            var campaignModel = new CampaignModel(_saveStateProvider.SaveState.CampaignState);
            var campaignDataViewModel = _campaignDataModelFactory.Create();
            viewModel.Bind(campaignModel, campaignDataViewModel);

            return viewModel;
        }
        
        protected override void DisposeInstances()
        {
            _campaignDataModelFactory.Release();
            base.DisposeInstances();
        }
    }
}