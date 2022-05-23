using Lumeer.Models.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lumeer.Models
{
    public class TaskActivityItem
    {
        public TaskActivity TaskActivity { get; set; }

        public string Type { get; set; }
        public string ChangeDateTxt { get; set; }
        public string NewStateTxt { get; set; }
        public string OldStateTxt { get; set; }

        public bool IsCreationActivity { get; set; }

        public TaskActivityItem(TaskActivity taskActivity, List<TableAttribute> tableAttributes)
        {
            TaskActivity = taskActivity;

            Type = taskActivity.Type ?? "Updated";

            ChangeDateTxt = GetFormattedChangeDate(taskActivity.ChangeDate);

            NewStateTxt = GetFormattedState(taskActivity.NewState, tableAttributes);
            OldStateTxt = GetFormattedState(taskActivity.OldState, tableAttributes);
        }
        
        public TaskActivityItem(Task task)
        {
            IsCreationActivity = true;

            Type = "Created";

            ChangeDateTxt = GetFormattedChangeDate(task.CreationDate);
        }

        private string GetFormattedChangeDate(long dateMs)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(dateMs).ToString("MMMM d, yyyy, h:mm:ss tt");
        }

        private string GetFormattedState(Dictionary<string, object> state, List<TableAttribute> tableAttributes)
        {
            var sb = new StringBuilder();

            foreach (KeyValuePair<string, object> kvp in state)
            {
                var tableAttribute = tableAttributes.Single(tA => tA.Id == kvp.Key);
                var formattedState = $"{tableAttribute.Name}: {kvp.Value}, ";
                sb.Append(formattedState);
            }

            // Remove ', ' from last state
            sb.Remove(sb.Length - 2, 2);

            return sb.ToString();
        }
    }
}
