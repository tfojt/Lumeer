using Lumeer.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Lumeer.TemplateSelectors
{
    public class TaskActivityDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ActivityDataTemplate { get; set; }
        public DataTemplate CreationActivityDataTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var activityItem = (TaskActivityItem)item;

            return activityItem.IsCreationActivity ? CreationActivityDataTemplate : ActivityDataTemplate;
        }
    }
}
