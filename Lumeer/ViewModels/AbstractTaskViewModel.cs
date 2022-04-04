using Lumeer.Models;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Task = Lumeer.Models.Rest.Task;

namespace Lumeer.ViewModels
{
    public abstract class AbstractTaskViewModel : BaseViewModel
    {
        public delegate void TaskEventHandler(Task task);

        /*public event TaskTableAttributeWrappersEventHandler TaskTableAttributeWrappersCreated;
        public delegate void TaskTableAttributeWrappersEventHandler(List<TaskTableAttributeWrapper> taskTableAttributeWrappers);*/

        public Task Task { get; set; }

        private Table _table;
        public Table Table
        {
            get => _table;
            set
            {
                if (_table == value)
                {
                    return;
                }

                _table = value;
                bool tableSelected = _table != null;
                if (tableSelected)
                {
                    CreateNewTaskTableAttributeWrappers();
                }

                TableSelected = tableSelected;
            }
        }

        private bool _tableSelected;
        public bool TableSelected
        {
            get => _tableSelected;
            set => SetValue(ref _tableSelected, value);
        }

        protected readonly IAlertService AlertService;
        protected readonly INavigationService NavigationService;

        public List<TaskTableAttributeWrapper> TaskTableAttributeWrappers { get; } = new List<TaskTableAttributeWrapper>();

        private TableSection _tableSection;

        public AbstractTaskViewModel(Task task, Table table, TableSection tableSection)
        {
            AlertService = DependencyService.Get<IAlertService>();
            NavigationService = DependencyService.Get<INavigationService>();

            _tableSection = tableSection;

            Task = task;
            Table = table;
        }

        private void CreateNewTaskTableAttributeWrappers()
        {
            // TODOT cache <Table, TaskTableAttributeWrapper> ?

            TaskTableAttributeWrappers.Clear();
            _tableSection.Clear();

            foreach (TableAttribute tableAttribute in Table.Attributes)
            {
                var taskTableAttributeWrapper = new TaskTableAttributeWrapper(Task, tableAttribute);
                TaskTableAttributeWrappers.Add(taskTableAttributeWrapper);
                _tableSection.Add(taskTableAttributeWrapper.Cell);
            }

            //TaskTableAttributeWrappersCreated?.Invoke(TaskTableAttributeWrappers);
        }

        protected async Task<HttpResponseMessage> SendTaskRequest(HttpMethod method, object payload)
        {
            string commonPartUri = $"organizations/{Session.Instance.OrganizationId}/projects/{Session.Instance.ProjectId}/collections/{Table.Id}/documents";
            if (method.Method == "PATCH")
            {
                commonPartUri += $"/{Task.Id}/data";
            }

            return await ApiClient.Instance.SendRequest(method, commonPartUri, payload);
        }

        protected bool TaskAttributesChanged(out Dictionary<string, object> changedAttributes)
        {
            changedAttributes = new Dictionary<string, object>();

            foreach (var wrapper in TaskTableAttributeWrappers)
            {
                if (wrapper.ValueChanged(out object newValue))
                {
                    changedAttributes.Add(wrapper.TableAttribute.Id, newValue);
                }
            }

            return changedAttributes.Count > 0;
        }

        protected void UpdateTaskDataAttributes(Dictionary<string, object> changedAttributes)
        {
            foreach (KeyValuePair<string, object> changedAttribute in changedAttributes)
            {
                if (Task.Data.ContainsKey(changedAttribute.Key))
                {
                    Task.Data[changedAttribute.Key] = changedAttribute.Value;
                }
                else
                {
                    Task.Data.Add(changedAttribute.Key, changedAttribute.Value);
                }
            }
        }
    }
}
