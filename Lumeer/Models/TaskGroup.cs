using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Lumeer.Models
{
    public class TaskGroup : ObservableCollection<TaskItem>
    {
        public const string ALL_GROUP_NAME = "All";
        public const string OTHERS_GROUP_NAME = "Others";

        public string Name { get; set; }

        public TaskGroup(string name)
        {
            Name = name;
        }

        public TaskGroup(string name, IEnumerable<TaskItem> tasks) : this(name)
        {
            Items.AddRange(tasks);
        }
    }
}
