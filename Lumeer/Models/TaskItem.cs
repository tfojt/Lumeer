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

        public List<SelectionOptionItem> Selections { get; set; } = new List<SelectionOptionItem>();

        public TaskItem(Rest.Task task)
        {
            Task = task;

            var table = Session.Instance.TaskTables.Single(t => t.Id == task.CollectionId);

            const string EMPTY_TITLE = "Empty title";
            if (Task.Data.TryGetValue(table.DefaultAttributeId, out object firstData))
            {
                string title = firstData?.ToString();
                Title = !string.IsNullOrEmpty(title) ? title : EMPTY_TITLE;
            }
            else
            {
                Title = EMPTY_TITLE;
            }

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
                if (string.IsNullOrEmpty(selection))
                {
                    continue;
                }

                var selectionListId = (string)tableAttribute.Constraint.Config["selectionListId"];
                var selectionList = Session.Instance.SelectionLists.Single(sl => sl.Id == selectionListId);
                var selectionOption = selectionList.Options.Single(o => o.EffectiveValue == selection);

                var selectionOptionItem = new SelectionOptionItem(selectionOption);
                Selections.Add(selectionOptionItem);
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
