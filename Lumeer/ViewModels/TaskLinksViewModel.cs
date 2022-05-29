using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using Lumeer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.ViewModels
{
    public class TaskLinksViewModel
    {
        public ObservableCollection<TaskLinkItem> Links { get; set; } = new ObservableCollection<TaskLinkItem>();

        public IAsyncCommand CreateNewLinkTypeCmd => new AsyncCommand(CreateNewLinkType);

        private readonly IAlertService _alertService;
        private readonly INavigationService _navigationService;

        private Models.Rest.Task _task;
        private Table _currentTable;

        private Dictionary<string, (Table Table, List<Document> TableDocuments)> tableDataCache = new Dictionary<string, (Table, List<Document>)>();

        public TaskLinksViewModel(Models.Rest.Task task, Table table)
        {
            _task = task;
            _currentTable = table;

            _alertService = DependencyService.Get<IAlertService>();
            _navigationService = DependencyService.Get<INavigationService>();

            Task.Run(LoadLinks);
        }

        private async Task LoadLinks()
        {
            try
            {
                var relevantLinkTypes = Session.Instance.LinkTypes.Where(lt => lt.CollectionIds.Contains(_task.CollectionId));

                foreach (var relevantLinkType in relevantLinkTypes)
                {
                    await GenerateTaskLinkItem(relevantLinkType);
                }
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while loading links", "Ok", ex);
            }
        }

        private async Task GenerateTaskLinkItem(LinkType linkType)
        {
            string linkedTableId = linkType.CollectionIds.Single(id => id != _task.CollectionId);
            var linkedTableData = await GetTableData(linkedTableId);

            var links = await ApiClient.Instance.GetLinks(_task.CollectionId, linkType.Id);
            var relevantLinks = links.Where(l => l.DocumentIds.Contains(_task.Id)).ToList();

            var taskLinkItem = new TaskLinkItem(linkType.Name, _currentTable, linkedTableData.Table, linkedTableData.TableDocuments, relevantLinks, _task, linkType);
            Links.Add(taskLinkItem);
        }

        private async Task<(Table Table, List<Document> TableDocuments)> GetTableData(string tableId)
        {
            if (!tableDataCache.TryGetValue(tableId, out var tableData))
            {
                var table = Session.Instance.AllTables.Single(t => t.Id == tableId);
                var tableDocuments = await ApiClient.Instance.GetDocuments(tableId);
                tableData = (table, tableDocuments);

                tableDataCache.Add(tableId, tableData);
            }

            return tableData;
        }

        private async Task CreateNewLinkType()
        {
            NewLinkType newLinkType = await _navigationService.ShowPopupAsync(new NewLinkTypePopup(_currentTable));
            if (newLinkType == null)
            {
                return;
            }

            try
            {
                var createdLinkType = await ApiClient.Instance.CreateLinkType(newLinkType);
                Session.Instance.LinkTypes.Add(createdLinkType);
                await GenerateTaskLinkItem(createdLinkType);
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while creating new link", "Ok", ex);
            }
        }
    }
}
