using System;
using DevKit.UI.MVVM;
using DevKit.UI.MVVM.Bases;
using DevKit.Utils;
using Inui.UI.MVVM.Settings;
using ITCafe.Data;
using R3;
using UnityEngine;
using VContainer;

namespace ITCafe.UI.MVVM
{
    public class PauseViewModel : ScreenViewModel
    {
        private readonly Subject<Unit> _exitToMenuSignal;
        private readonly Subject<Unit> _restartSignal;
        private readonly InputService _inputService;
        private readonly IViewBinder<SettingsViewModel> _settingsBinder;
        private readonly IViewBinder<ConfirmPopUpViewModel> _confirmBinder;

        private ConfirmationSetup _exitSetup;
        private ConfirmationSetup _restartSetup;
        private ConfirmPopUpViewModel _confirmViewModel;

        private IDisposable _currentPopupDisposable;

        public PauseViewModel([Key(Constants.GAMEPLAY_EXIT_SIGNAL)] Subject<Unit> exitToMenuSignal,
            [Key(Constants.RESTART_GAMEPLAY_SIGNAL)]
            Subject<Unit> restartSignal,
            InputService inputService,
            IViewBinder<SettingsViewModel> settingsBinder,
            IViewBinder<ConfirmPopUpViewModel> confirmBinder)
        {
            _exitToMenuSignal = exitToMenuSignal;
            _restartSignal = restartSignal;
            _inputService = inputService;
            _settingsBinder = settingsBinder;
            _confirmBinder = confirmBinder;
        }

        public override void Open()
        {
            _inputService.SetInputEnabled(false);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
            base.Open();
        }

        public override void CompleteClosing()
        {
            _inputService.SetInputEnabled(true);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
            base.CompleteClosing();
        }

        public void SetupExitPopUp(ConfirmationSetup exitSetup)
        {
            _exitSetup = exitSetup;
        }

        public void SetupRestartPopUp(ConfirmationSetup restartSetup)
        {
            _restartSetup = restartSetup;
        }

        public void OpenSettings()
        {
            _settingsBinder.Open();
        }

        public void ExitToMenu()
        {
            BindPopUp(_ => _exitToMenuSignal.OnNext(Unit.Default), _exitSetup);
        }

        public void Restart()
        {
            BindPopUp(_ => _restartSignal.OnNext(Unit.Default), _restartSetup);
        }

        private void BindPopUp(Action<Unit> onConfirmed, ConfirmationSetup setup)
        {
            if (_confirmViewModel != null)
                return;

            _confirmViewModel = _confirmBinder.Open();
            _confirmViewModel.Setup(setup);

            _currentPopupDisposable = _confirmViewModel.OnConfirmed
                .Take(1)
                .Subscribe(onConfirmed);

            Subs.SubscribeOnce(() =>
                {
                    Disposes.ClearDispose(ref _currentPopupDisposable);
                    _confirmViewModel = null;
                },
                x => _confirmViewModel.OnClosingCompleted += x,
                x => _confirmViewModel.OnClosingCompleted -= x);
        }

        public override void Dispose()
        {
            Disposes.ClearDispose(ref _currentPopupDisposable);

            base.Dispose();
        }
    }
}