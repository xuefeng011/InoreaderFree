﻿using System;
using System.Collections.Generic;
using System.Windows.Input;
using Windows.UI.Xaml.Navigation;
using Inoreader.Api;
using Inoreader.ViewModels.Details;
using Microsoft.ApplicationInsights;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;
using Microsoft.Practices.Unity;

namespace Inoreader.ViewModels.Pages
{
	public class SubscriptionsPageViewModel : ViewModel, INavigateBackwards
	{
		private readonly INavigationService _navigationService;
		private readonly ApiClient _apiClient;

		private ICommand _settingsPageCommand;
		private ICommand _aboutPageCommand;
		private ICommand _signOutCommand;

		public SubscriptionsViewModel Subscriptions { get; private set; }

		public ICommand SettingsPageCommand
		{
			get { return _settingsPageCommand ?? (_settingsPageCommand = new DelegateCommand(OnSettingsPage)); }
		}

		public ICommand AboutPageCommand
		{
			get { return _aboutPageCommand ?? (_aboutPageCommand = new DelegateCommand(OnAboutPage)); }
		}

		public ICommand SignOutCommand
		{
			get { return _signOutCommand ?? (_signOutCommand = new DelegateCommand(OnSignOut)); }
		}

		public SubscriptionsPageViewModel(IUnityContainer container, INavigationService navigationService, ApiClient apiClient)
		{
			if (container == null) throw new ArgumentNullException("container");
			if (navigationService == null) throw new ArgumentNullException("navigationService");
			if (apiClient == null) throw new ArgumentNullException("apiClient");

			_navigationService = navigationService;
			_apiClient = apiClient;

			Subscriptions = container.Resolve<SubscriptionsViewModel>();
		}

		public override void OnNavigatedTo(object navigationParameter, NavigationMode navigationMode, Dictionary<string, object> viewModelState)
		{
			base.OnNavigatedTo(navigationParameter, navigationMode, viewModelState);

			Subscriptions.LoadSubscriptions();
		}

		public bool NavigateBack()
		{
			return Subscriptions.NavigateBack();
		}

		private void OnSettingsPage()
		{
			_navigationService.Navigate(PageTokens.Settings, null);
		}

		private void OnAboutPage()
		{
			_navigationService.Navigate(PageTokens.About, null);
		}

		private void OnSignOut()
		{
			_apiClient.ClearSession();
			_navigationService.Navigate(PageTokens.SignIn, null);			
		}
	}
}