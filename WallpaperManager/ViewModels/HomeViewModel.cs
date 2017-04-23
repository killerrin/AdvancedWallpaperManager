﻿using GalaSoft.MvvmLight.Command;
using KillerrinStudiosToolkit;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KillerrinStudiosToolkit.Settings;
using WallpaperManager.Models;
using WallpaperManager.Models.Settings;
using Windows.UI.Xaml.Controls;
using System.IO;
using KillerrinStudiosToolkit.UserProfile;

namespace WallpaperManager.ViewModels
{
    public class HomeViewModel : WallpaperManagerViewModelBase
    {
        public static HomeViewModel Instance { get { return ServiceLocator.Current.GetInstance<HomeViewModel>(); } }

        public ActiveDesktopThemeSetting ActiveDesktopThemeSetting { get; } = new ActiveDesktopThemeSetting();
        public ActiveLockscreenThemeSetting ActiveLockscreenThemeSetting { get; } = new ActiveLockscreenThemeSetting();
        public ActiveDesktopThemeHistorySetting ActiveDesktopThemeHistorySetting { get; } = new ActiveDesktopThemeHistorySetting();
        public ActiveLockscreenThemeHistorySetting ActiveLockscreenThemeHistorySetting { get; } = new ActiveLockscreenThemeHistorySetting();

        WallpaperTheme m_activeWallpaperTheme = null;
        public WallpaperTheme ActiveWallpaperTheme
        {
            get { return m_activeWallpaperTheme; }
            set
            {
                m_activeWallpaperTheme = value;
                RaisePropertyChanged(nameof(ActiveWallpaperTheme));
            }
        }

        WallpaperTheme m_activeLockscreenTheme = null;
        public WallpaperTheme ActiveLockscreenTheme
        {
            get { return m_activeLockscreenTheme; }
            set
            {
                m_activeLockscreenTheme = value;
                RaisePropertyChanged(nameof(ActiveLockscreenTheme));
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public HomeViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
            }

            ResetViewModel();
        }

        public override void Loaded()
        {

        }

        public override void OnNavigatedTo()
        {
            try
            {
                if (ActiveDesktopThemeSetting.Value.HasValue)
                {
                    ActiveWallpaperTheme = ThemeRepository.Find(ActiveDesktopThemeSetting.Value.Value);
                    Debug.WriteLine($"Active Wallpaper Theme: {ActiveDesktopThemeSetting.Value} - {ActiveWallpaperTheme?.ID} - {ActiveWallpaperTheme?.Name}");
                }
            }
            catch (Exception) { ActiveDesktopThemeSetting.RevertToDefault(); }

            try
            { 
                if (ActiveLockscreenThemeSetting.Value.HasValue)
                {
                    ActiveLockscreenTheme = ThemeRepository.Find(ActiveLockscreenThemeSetting.Value.Value);
                    Debug.WriteLine($"Active Lockscreen Theme: {ActiveLockscreenThemeSetting.Value} - {ActiveLockscreenTheme?.ID} - {ActiveLockscreenTheme?.Name}");
                }
            }
            catch (Exception) { ActiveLockscreenThemeSetting.RevertToDefault(); }
        }

        public override void OnNavigatedFrom()
        {

        }

        public override void ResetViewModel()
        {

        }

        #region Next Commands
        public RelayCommand NextDesktopWallpaperCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    NextDesktopWallpaper();
                });
            }
        }
        public async void NextDesktopWallpaper()
        {
            if (ActiveWallpaperTheme == null) return;
            Debug.WriteLine($"{nameof(NextDesktopWallpaper)} - Begin");

            // Grab a Random Image
            string randomImagePath = ActiveWallpaperTheme.RandomImageFromCache;
            Debug.WriteLine($"{nameof(NextDesktopWallpaper)} - Random Image Selected: {randomImagePath}");

            // Get the Storage File
            StorageTask storageTask = new StorageTask();
            var file = await storageTask.GetFileFromPath(new Uri(randomImagePath));
            Debug.WriteLine($"{nameof(NextLockscreen)} - Converted Path to File: {file.Name}");

            // Set the Wallpaper
            WallpaperTools wallpaperTools = new WallpaperTools();
            if (await wallpaperTools.SetWallpaperImage(file))
            {
                Debug.WriteLine($"{nameof(NextDesktopWallpaper)} - Successfully Changed Image");

                // Add it to the history if successful
                ActiveDesktopThemeHistorySetting.Add(randomImagePath);
            }
            else { Debug.WriteLine($"{nameof(NextDesktopWallpaper)} - Failed"); }

            Debug.WriteLine($"{nameof(NextDesktopWallpaper)} - Completed");
        }

        public RelayCommand NextLockscreenCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    NextLockscreen();
                });
            }
        }

        public async void NextLockscreen()
        {
            if (ActiveLockscreenTheme == null) return;
            Debug.WriteLine($"{nameof(NextLockscreen)} - Begin");

            // Grab a Random Image
            string randomImagePath = ActiveLockscreenTheme.RandomImageFromCache;
            var randomImageName = Path.GetFileNameWithoutExtension(randomImagePath);
            Debug.WriteLine($"{nameof(NextLockscreen)} - Random Image Selected: {randomImagePath}");

            // Set the Lockscreen
            LockscreenTools lockscreenTools = new LockscreenTools();
            if (await lockscreenTools.SetLockscreenImageFromFileSystem(randomImagePath))
            {
                Debug.WriteLine($"{nameof(NextLockscreen)} - Successfully Changed Image");

                // Add it to the history if successful
                ActiveLockscreenThemeHistorySetting.Add(randomImagePath);
            }
            else { Debug.WriteLine($"{nameof(NextLockscreen)} - Failed"); }

            Debug.WriteLine($"{nameof(NextLockscreen)} - Completed");
        }
        #endregion

        #region Navigate Commands
        public RelayCommand NavigateActiveWallpaperThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (ActiveWallpaperTheme == null) return;
                    ThemeListViewModel.Instance.NavigateTheme(ActiveWallpaperTheme);
                });
            }
        }
        public RelayCommand NavigateActiveLockscreenThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    if (ActiveLockscreenTheme == null) return;
                    ThemeListViewModel.Instance.NavigateTheme(ActiveLockscreenTheme);
                });
            }
        }
        #endregion

        #region Deselect Theme Commands
        public RelayCommand DeselectActiveWallpaperThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DeselectActiveWallpaperThemeDialog();
                });
            }
        }
        public async void DeselectActiveWallpaperThemeDialog()
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Deselect Desktop Wallpaper Theme?",
                Content = "If you deselect this theme it will no longer automatically update your Desktop Wallpaper Background",
                PrimaryButtonText = "Deselect",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ActiveDesktopThemeSetting.RevertToDefault();
                ActiveDesktopThemeHistorySetting.RevertToDefault();
                ActiveWallpaperTheme = null;
            }
        }

        public RelayCommand DeselectActiveLockscreenThemeCommand
        {
            get
            {
                return new RelayCommand(() =>
                {
                    DeselectActiveLockscreenThemeDialog();
                });
            }
        }
        public async void DeselectActiveLockscreenThemeDialog()
        {
            ContentDialog deleteDialog = new ContentDialog
            {
                Title = "Deselect Lockscreen Theme?",
                Content = "If you deselect this theme it will no longer automatically update your Lockscreen Background",
                PrimaryButtonText = "Deselect",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await deleteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                ActiveLockscreenThemeSetting.RevertToDefault();
                ActiveLockscreenThemeHistorySetting.RevertToDefault();
                ActiveLockscreenTheme = null;
            }
        }
        #endregion
    }
}
