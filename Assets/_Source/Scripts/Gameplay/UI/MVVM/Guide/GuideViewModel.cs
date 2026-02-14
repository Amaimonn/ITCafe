using DevKit.UI.MVVM.Bases;
using ITCafe.Data;
using R3;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class GuideViewModel : ScreenViewModel
    {
        public Observable<IGuideData> OnGuideChanged => _currentGuide;
        public Observable<int> OnCurrentPageIndexChanged => _currentPageIndex;
        public Observable<int> OnPagesCountChanged => _pagesCount;
        public Observable<bool> OnNextEnabledChanged => _isNextEnabled;
        public Observable<bool> OnPreviousEnabledChanged => _isPreviousEnabled;

        private readonly ReactiveProperty<IGuideData> _currentGuide;
        private readonly ReactiveProperty<int> _currentPageIndex = new(0);
        private readonly ReactiveProperty<int> _pagesCount;
        private readonly ReactiveProperty<bool> _isNextEnabled;
        private readonly ReactiveProperty<bool> _isPreviousEnabled = new(false);

        public GuideViewModel(IGuideData guideData)
        {
            _currentGuide = new ReactiveProperty<IGuideData>(guideData);
            _pagesCount = new ReactiveProperty<int>(guideData.Pages.Count);
            _isNextEnabled = new ReactiveProperty<bool>(_pagesCount.Value > 0);
        }

        public void GoNextPage()
        {
            if (_currentPageIndex.Value < _pagesCount.Value - 1)
                _currentPageIndex.Value++;

            UpdatePaginationButtons();
        }

        public void GoPreviousPage()
        {
            if (_currentPageIndex.Value > 0)
                _currentPageIndex.Value--;

            UpdatePaginationButtons();
        }

        private void UpdatePaginationButtons()
        {
            _isNextEnabled.Value = _currentPageIndex.Value < _pagesCount.Value - 1;
            _isPreviousEnabled.Value = _currentPageIndex.Value > 0;
        }
    }
}
