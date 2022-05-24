using Lumeer.Models.Rest;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models
{
    public class TasksFilterSettings
    {
        public TasksFilter TasksFilter { get; set; } = new TasksFilter();

        public bool IncludeSubItems { get; set; }
    }
}
