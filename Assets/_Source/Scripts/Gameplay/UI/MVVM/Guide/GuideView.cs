using System;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using ITCafe.Data;
using UnityEngine;
using UnityEngine.UIElements;
using R3;

namespace ITCafe.Gameplay.UI.MVVM
{
    public class GuideView : ScreenToolkitAttach<GuideViewModel>
    {
        [SerializeField] private string _pagesContainerName = "PagesContainer";
        [SerializeField] private string _okButtonName = "OkButton";
        [SerializeField] private string _closeButtonName = "CloseButton";
        [SerializeField] private string _nextButtonName = "NextButton";
        [SerializeField] private string _previousButtonName = "PreviousButton";
        [SerializeField] private string _paginationContainerName = "PaginationContainer";
        [SerializeField] private string _paginationDotClass = "guide__page-indicator";

        private VisualElement _pagesContainer;
        private Button _okButton;
        private Button _closeButton;
        private Button _nextButton;
        private Button _previousButton;
        private VisualElement _paginationContainer;
        private VisualElement[] _paginationDots;
        private VisualElement _activeDot;
        private VisualElement[] _pages;
        private VisualElement _activePage;

        protected override void OnInit()
        {
            _pagesContainer = Root.Q<VisualElement>(name: _pagesContainerName);
            _pagesContainer.Clear();
            _okButton = Root.Q<Button>(name: _okButtonName);
            _closeButton = Root.Q<Button>(name: _closeButtonName);
            _nextButton = Root.Q<Button>(name: _nextButtonName);
            _previousButton = Root.Q<Button>(name: _previousButtonName);
            _paginationContainer = Root.Q<VisualElement>(name: _paginationContainerName);
            _paginationContainer.Clear();
        }

        protected override void OnBind(GuideViewModel viewModel)
        {
            base.OnBind(viewModel);

            _nextButton.SubscribeCallback<ClickEvent>(OnNextClicked)
                .AddTo(_disposables);
            _previousButton.SubscribeCallback<ClickEvent>(OnPreviousClicked)
                .AddTo(_disposables);
            _okButton.SubscribeCallbackOnce<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);
            _closeButton.SubscribeCallbackOnce<ClickEvent>(OnCloseClicked)
                .AddTo(_disposables);

            viewModel.OnGuideChanged.Subscribe(OnGuideChanged)
                .AddTo(_disposables);
            viewModel.OnPagesCountChanged.Subscribe(OnPagesCountChanged)
                .AddTo(_disposables);
            viewModel.OnCurrentPageIndexChanged.Subscribe(OnCurrentPageIndexChanged)
                .AddTo(_disposables);
            viewModel.OnNextEnabledChanged.Subscribe(_nextButton.SetEnabled)
                .AddTo(_disposables);
            viewModel.OnPreviousEnabledChanged.Subscribe(_previousButton.SetEnabled)
                .AddTo(_disposables);
        }

        private void OnCloseClicked(ClickEvent _)
        {
            ViewModel.StartClosing();
        }

        private void OnNextClicked(ClickEvent _)
        {
            ViewModel.GoNextPage();
        }

        private void OnPreviousClicked(ClickEvent _)
        {
            ViewModel.GoPreviousPage();
        }

        private void OnGuideChanged(GuideSO guideSO)
        {
            _pagesContainer.Clear();

            var pages = guideSO.Pages;

            if (pages.Count == 0)
            {
                _pages = Array.Empty<VisualElement>();
                return;
            }

            _pages = new VisualElement[pages.Count];
            _activePage = CreatePage(pages[0]);
            _pages[0] = _activePage;

            for (var i = 1; i < pages.Count; i++)
            {
                var page = CreatePage(pages[i]);
                page.style.display = DisplayStyle.None;
                _pages[i] = page;
            }
        }

        private VisualElement CreatePage(VisualTreeAsset pageAsset)
        {
            var pageElement = pageAsset.CloneTree();
            _pagesContainer.Add(pageElement);

            return pageElement;
        }

        private void OnCurrentPageIndexChanged(int pageIndex)
        {
            UpdateCurrentDot(pageIndex);
            UpdateCurrentPage(pageIndex);
        }

        private void UpdateCurrentDot(int pageIndex)
        {
            if (pageIndex >= _paginationDots.Length || pageIndex < 0)
                return;

            _activeDot?.SetEnabled(false);
            _activeDot = _paginationDots[pageIndex];
            _activeDot.SetEnabled(true);
        }

        private void UpdateCurrentPage(int pageIndex)
        {
            if (pageIndex >= _pages.Length || pageIndex < 0)
                return;
            if (_activePage != null)
                _activePage.style.display = DisplayStyle.None;
            _activePage = _pages[pageIndex];
            _activePage.style.display = DisplayStyle.Flex;
        }

        private void OnPagesCountChanged(int count)
        {
            _paginationContainer.Clear();
            _paginationDots = new VisualElement[count];
            for (var i = 0; i < count; i++)
            {
                var dot = new VisualElement();
                dot.AddToClassList(_paginationDotClass);
                dot.SetEnabled(false);
                _paginationDots[i] = dot;
                _paginationContainer.Add(dot);
            }
        }
    }
}