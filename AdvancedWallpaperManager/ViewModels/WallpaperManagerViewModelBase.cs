﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using AdvancedWallpaperManager.DAL.Repositories;
using KillerrinStudiosToolkit.Services;
using KillerrinStudiosToolkit.Store;

namespace AdvancedWallpaperManager.ViewModels
{
    public abstract class WallpaperManagerViewModelBase : ViewModelBase
    {
        #region Services
        public bool CanNavigate { get { return SimpleIoc.Default.IsRegistered<NavigationService>(); } }
        public NavigationService NavigationService
        {
            get
            {
                return SimpleIoc.Default.GetInstance<NavigationService>(); ;
            }
        }

        public bool HasProgressService { get { return SimpleIoc.Default.IsRegistered<ProgressService>(); } }
        public ProgressService ProgressService
        {
            get
            {
                return SimpleIoc.Default.GetInstance<ProgressService>(); ;
            }
        }

        public bool HasMediaService { get { return SimpleIoc.Default.IsRegistered<MediaService>(); } }
        public MediaService MediaService
        {
            get
            {
                return SimpleIoc.Default.GetInstance<MediaService>(); ;
            }
        }

        public bool HasSplitViewService { get { return SimpleIoc.Default.IsRegistered<SplitViewService>(); } }
        public SplitViewService SplitViewService
        {
            get
            {
                return SimpleIoc.Default.GetInstance<SplitViewService>(); ;
            }
        }

        public static string m_topNavBarText = "";
        public string TopNavBarText
        {
            get { return m_topNavBarText; }
            set
            {
                m_topNavBarText = value;
                RaisePropertyChanged(nameof(TopNavBarText));
            }
        }

        #endregion

        #region App Products
        private static AppProduct m_productAWMPro = new AppProduct("AWMPro", "9nzm4xdbvpk0", false);
        public AppProduct ProductAWMPro
        {
            get { return m_productAWMPro; }
            set
            {
                if (value == null) return;
                m_productAWMPro = value;
                RaisePropertyChanged(nameof(ProductAWMPro));
            }
        }
        #endregion

        #region Repositories
        public WallpaperThemeRepository ThemeRepository { get; set; }
        public WallpaperDirectoryRepository DirectoryRepository { get; set; }
        public FileAccessTokenRepository AccessTokenRepository { get; set; }
        public FileDiscoveryCacheRepository FileDiscoveryCacheRepository { get; set; }
        #endregion

        public WallpaperManagerViewModelBase()
        {
            // Setup the Repos
            var context = new WallpaperManagerContext();
            ThemeRepository = new WallpaperThemeRepository(context);
            DirectoryRepository = new WallpaperDirectoryRepository(context);
            AccessTokenRepository = new FileAccessTokenRepository(context);
            FileDiscoveryCacheRepository = new FileDiscoveryCacheRepository(context);

            // Setup the AppProducts
            try
            {
                InAppPurchaseManagerBase.AppProductsChanged += InAppPurchaseManager_AppProductsChanged;
                var product = InAppPurchaseManagerFactory.Create(true).GetAppProductByStoreID("9nzm4xdbvpk0");
                ProductAWMPro = product;
                Debug.WriteLine($"{nameof(WallpaperManagerViewModelBase)} | {ProductAWMPro}");
            }
            catch (Exception) { }
        }

        private void InAppPurchaseManager_AppProductsChanged(InAppPurchaseManagerBase sender, List<AppProduct> args)
        {
            try
            {
                Debug.WriteLine($"{nameof(InAppPurchaseManager_AppProductsChanged)}");
                foreach (var product in args)
                {
                    Debug.WriteLine($"New Product | {product}");
                }

                ProductAWMPro = sender.GetAppProductByStoreID("9nzm4xdbvpk0");
            }
            catch (Exception) { }
        }

        public abstract void Loaded();
        public abstract void OnNavigatedTo();
        public abstract void OnNavigatedFrom();
        public abstract void ResetViewModel();

        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) return;

            try
            {
                base.RaisePropertyChanged(propertyName);
            }
            catch (Exception) { }
        }
    }
}
