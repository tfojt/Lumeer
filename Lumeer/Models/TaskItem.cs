using Lumeer.Fonts;
using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Lumeer.Models
{
    public class TaskItem
    {
        public Rest.Task Task { get; set; }

        public string Title { get; set; }

        public string TableFontFamily { get; set; }
        public string TableGlyph { get; set; }
        public Color TableColor { get; set; }

        public List<SelectionOption> Selections { get; set; } = new List<SelectionOption>();

        public TaskItem(Rest.Task task)
        {
            Task = task;

            const string EMPTY_TITLE = "Empty title";
            if (Task.Data.TryGetValue("a1", out object firstData))
            {
                string title = firstData?.ToString();
                Title = !string.IsNullOrEmpty(title) ? title : EMPTY_TITLE;
            }
            else
            {
                Title = EMPTY_TITLE;
            }

            var table = Session.Instance.TaskTables.Single(t => t.Id == task.CollectionId);

            TableFontFamily = FontAwesomeAliases.PRO_REGULAR;
            TableGlyph = FontAwesomeIcons.CircleQuestion;
            TableColor = Color.FromHex(table.Color);

            foreach (var tableAttribute in table.Attributes)
            {
                var attributeType = ParseAttributeType(tableAttribute);
                if (attributeType != AttributeType.Select)
                {
                    continue;
                }

                bool hasValue = task.Data.TryGetValue(tableAttribute.Id, out object value);
                if (!hasValue)
                {
                    continue;
                }

                string selection = value.ToString();

                if (!string.IsNullOrEmpty(selection))
                {
                    var selectionOption = new SelectionOption()
                    {
                        Value = selection
                    };

                    Selections.Add(selectionOption);
                }

                /*var selectionListId = (string)tableAttribute.Constraint.Config["selectionListId"];
                var selectionList = Session.Instance.SelectionLists.Single(sl => sl.Id == selectionListId);*/
            }

            var random = new Random();
            var count = random.Next(1, 10);
            for (int i = 0; i < count; i++)
            {
                var selectionOption = new SelectionOption()
                {
                    Value = $"Selection{i}"
                };

                Selections.Add(selectionOption);
            }
        }

        private AttributeType ParseAttributeType(TableAttribute tableAttribute)
        {
            var constraint = tableAttribute.Constraint;
            if (constraint == null)
            {
                return AttributeType.None;
            }

            return (AttributeType)Enum.Parse(typeof(AttributeType), constraint.Type);
        }
    }
}
