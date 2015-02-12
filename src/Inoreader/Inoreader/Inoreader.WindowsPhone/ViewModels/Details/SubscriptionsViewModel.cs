using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Inoreader.Api;
using Inoreader.Api.Models;
using Inoreader.Services;
using Microsoft.ApplicationInsights;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.Mvvm.Interfaces;

namespace Inoreader.ViewModels.Details
{
	public class SubscriptionsViewModel : BindableBase
	{
		#region Fields

		private static readonly Regex CategoryRegex = new Regex("^user/[0-9]*/label/", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		private readonly INavigationService _navigationService;
		private readonly ApiClient _apiClient;
		private readonly TelemetryClient _telemetryClient;
		private readonly AppSettingsService _settingsService;

		private bool _isBusy;
		private List<TreeItemBase> _treeItems;
		private List<TreeItemBase> _rootItems;
		private bool _isRoot = true;

		private ICommand _itemClickCommand;
		private ICommand _refreshCommand;

		#endregion

		#region Properties

		private string _subscriptionsHeader = Strings.Resources.SubscriptionsSectionHeader;

		public string SubscriptionsHeader
		{
			get { return _subscriptionsHeader; }
			set { SetProperty(ref _subscriptionsHeader, value); }
		}

		public List<TreeItemBase> TreeItems
		{
			get { return _treeItems; }
			set { SetProperty(ref _treeItems, value); }
		}

		public bool IsBusy
		{
			get { return _isBusy; }
			set { SetProperty(ref _isBusy, value); }
		}

		#endregion

		#region Commands

		public ICommand ItemClickCommand
		{
			get { return _itemClickCommand ?? (_itemClickCommand = new DelegateCommand<object>(OnItemClick)); }
		}

		public ICommand RefreshCommand
		{
			get { return _refreshCommand ?? (_refreshCommand = new DelegateCommand(OnRefresh)); }
		}

		#endregion


		public SubscriptionsViewModel(INavigationService navigationService, ApiClient apiClient, TelemetryClient telemetryClient, AppSettingsService settingsService)
		{
			if (navigationService == null) throw new ArgumentNullException("navigationService");
			if (apiClient == null) throw new ArgumentNullException("apiClient");
			if (telemetryClient == null) throw new ArgumentNullException("telemetryClient");
			if (settingsService == null) throw new ArgumentNullException("settingsService");

			_navigationService = navigationService;
			_apiClient = apiClient;
			_telemetryClient = telemetryClient;
			_settingsService = settingsService;
		}

		public async void LoadSubscriptions()
		{
			IsBusy = true;

			Exception error = null;
			try
			{
				var stopwatch = Stopwatch.StartNew();

				var tags = await _apiClient.GetTagsAsync();
				var subscriptions = await _apiClient.GetSubscriptionsAsync();
				var unreadCount = await _apiClient.GetUnreadCountAsync();

				stopwatch.Stop();
				_telemetryClient.TrackMetric(TemetryMetrics.GetSubscriptionsTotalResponseTime, stopwatch.Elapsed.TotalSeconds);
				
				var unreadCountDictionary = unreadCount.UnreadCounts.ToDictionary(uk => uk.Id, uk => uk.Count);

				var catsQuery = from tag in tags.Tags
								where CategoryRegex.IsMatch(tag.Id)
								select new CategoryItem
								{
									Id = tag.Id,
									SortId = tag.SortId,
									UnreadCount = GetUnreadCount(unreadCountDictionary, tag.Id)
								};

				var categories = catsQuery.ToList();

				foreach (var categoryItem in categories)
				{
					var subsQuery = from s in subscriptions.Subscriptions
									where s.Categories != null
										  && s.Categories.Any(c => String.Equals(c.Id, categoryItem.Id, StringComparison.OrdinalIgnoreCase))
									orderby s.Title// descending 
									select CreateSubscriptionItem(s, unreadCountDictionary);

					categoryItem.Subscriptions = new List<SubscriptionItem>(subsQuery);
					categoryItem.Title = (from s in subscriptions.Subscriptions
										  from c in s.Categories
										  where String.Equals(c.Id, categoryItem.Id, StringComparison.OrdinalIgnoreCase)
										  select c.Label).FirstOrDefault();
				}

				// hide empty groups
				categories.RemoveAll(c => c.Subscriptions.Count == 0);

				var singleItems = (from s in subscriptions.Subscriptions								  
								  where s.Categories == null || s.Categories.Length == 0
								  orderby s.Title
								  select CreateSubscriptionItem(s, unreadCountDictionary)).ToList();

				var allItems = new List<TreeItemBase>(categories.OrderBy(c => c.Title));
				allItems.AddRange(singleItems);

				if (_settingsService.HideEmptySubscriptions)
				{
					HideEmpty(allItems);					
				}

				_rootItems = allItems;

				TreeItems = _rootItems;
				_isRoot = true;
			}
			catch (Exception ex)
			{
				error = ex;
				_telemetryClient.TrackException(ex);
			}
			finally
			{
				IsBusy = false;
			}
			
			if (error != null)
			{
				MessageDialog msgbox = new MessageDialog(error.Message, Strings.Resources.ErrorDialogTitle);
				await msgbox.ShowAsync();
			}
		}

		private void HideEmpty(List<TreeItemBase> allItems)
		{
			allItems.RemoveAll(c => c.UnreadCount == 0);
			foreach (var cat in allItems.OfType<CategoryItem>())
			{
				cat.Subscriptions.RemoveAll(c => c.UnreadCount == 0);
			}
		}

		private static SubscriptionItem CreateSubscriptionItem(Subscription s, Dictionary<string, int> unreadCountDictionary)
		{
			return new SubscriptionItem
			{
				Id = s.Id,
				SortId = s.SortId,
				Url = s.Url,
				HtmlUrl = s.HtmlUrl,
				IconUrl = s.IconUrl,
				Title = s.Title,
				FirstItemMsec = s.FirstItemMsec,
				UnreadCount = GetUnreadCount(unreadCountDictionary, s.Id)
			};
		}

		private static int GetUnreadCount(Dictionary<string, int> unreadCounts, string id)
		{
			int count;
			return unreadCounts.TryGetValue(id, out count) ? count : 0;
		}

		private void OnItemClick(object args)
		{
			var clickEventArgs = (ItemClickEventArgs)args;

			var categoryItem = clickEventArgs.ClickedItem as CategoryItem;
			if (categoryItem != null)
			{
				SubscriptionsHeader = categoryItem.Title;
				TreeItems = new List<TreeItemBase>(categoryItem.Subscriptions);
				_isRoot = false;
			}
			else
			{
				var subscriptionItem = clickEventArgs.ClickedItem as SubscriptionItem;
				if (subscriptionItem != null)
					_navigationService.Navigate(PageTokens.Stream, subscriptionItem.Id);
			}
		}

		private void OnRefresh()
		{
			_telemetryClient.TrackEvent(TelemetryEvents.ManualRefreshSubscriptions);
			LoadSubscriptions();
		}

		public bool NavigateBack()
		{
			if (!_isRoot)
			{
				SubscriptionsHeader = Strings.Resources.SubscriptionsSectionHeader;
				TreeItems = _rootItems;
				_isRoot = true;
				return false;
			}

			return true;
		}
	}

	public abstract class TreeItemBase
	{
		public string Id { get; set; }
		public string SortId { get; set; }
		public string Title { get; set; }
		public int UnreadCount { get; set; }
	}

	public class SubscriptionItem : TreeItemBase
	{
		public string Url { get; set; }
		public string HtmlUrl { get; set; }
		public string IconUrl { get; set; }
		public long FirstItemMsec { get; set; }
	}

	public class CategoryItem : TreeItemBase
	{
		public List<SubscriptionItem> Subscriptions { get; set; }
	}
}