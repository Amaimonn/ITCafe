using System;
using DevKit.Solutions;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using ITCafe.Data.Campaign;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class CampaignBinder : SimpleAttachBinder<CampaignView, CampaignViewModel>
    {
        private readonly CampaignDataModelFactory _campaignDataModelFactory;
        private readonly IFactory<CampaignModel> _campaignModelFactory;

        public CampaignBinder(Func<CampaignViewModel> viewModelFactory,
            CampaignDataModelFactory campaignDataModelFactory,
            IFactory<CampaignModel> campaignModelFactory,
            IRootUIBinder rootUIBinder,
            CampaignView viewPrefab) :
                base(viewModelFactory, rootUIBinder, viewPrefab)
        {
            _campaignDataModelFactory = campaignDataModelFactory;
            _campaignModelFactory = campaignModelFactory;
        }

        protected override CampaignViewModel GetViewModel()
        {
            var viewModel = base.GetViewModel();
            
            var campaignModel = _campaignModelFactory.Create();
            var campaignDataModel = _campaignDataModelFactory.Create();
            viewModel.Bind(campaignModel, campaignDataModel);

            return viewModel;
        }
        
        protected override void DisposeInstances()
        {
            _campaignDataModelFactory.Release();
            base.DisposeInstances();
        }
    }
}