using Lumeer.Models.Rest;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Lumeer.TemplateSelectors
{
    public class TaskDataTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<string, DataTemplate> _tableDataTemplates;
        
        public TaskDataTemplateSelector(Dictionary<string, DataTemplate> tableDataTemplates)
        {
            _tableDataTemplates = tableDataTemplates;
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var task = (Task)item;

            return _tableDataTemplates[task.CollectionId];
        }
    }
}
