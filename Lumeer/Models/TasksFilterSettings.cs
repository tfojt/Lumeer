using Lumeer.Models.Rest;

namespace Lumeer.Models
{
    public class TasksFilterSettings
    {
        public TasksFilter TasksFilter { get; set; } = new TasksFilter();

        public bool IncludeSubItems { get; set; }
    }
}
