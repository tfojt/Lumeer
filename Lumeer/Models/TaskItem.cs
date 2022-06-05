using Lumeer.Fonts;
using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using ConstraintType = Lumeer.Models.Rest.Enums.ConstraintType;

namespace Lumeer.Models
{
    public class TaskItem
    {
        public Rest.Task Task { get; set; }

        public string Title { get; set; }

        public string TableFontFamily { get; set; }
        public string TableGlyph { get; set; }
        public Color TableColor { get; set; }

        public FontImageData TableFontImageData { get; set; }

        public List<SelectionOptionItem> Selections { get; set; } = new List<SelectionOptionItem>();

        public GravatarImageSource GravatarImageSource { get; set; }

        private const string EMPTY_TITLE = "Empty title";

        public TaskItem(Rest.Task task)
        {
            Task = task;

            var table = Session.Instance.TaskTables.Single(t => t.Id == task.CollectionId);

            Title = GetTaskTitle(table);

            TableFontImageData = new FontImageData(table.Icon, table.Color);

            GenerateSelections(table);

            if (TryGetAssigneeEmail(table, out string assigneeEmail))
            {
                GravatarImageSource = new GravatarImageSource
                {
                    Email = assigneeEmail,
                    Size = 20,
                };
            }
        }

        private string GetTaskTitle(Table table)
        {
            if (!Task.Data.TryGetValue(table.DefaultAttributeId, out object defaultAttribute) || defaultAttribute == null)
            {
                return EMPTY_TITLE;
            }

            return defaultAttribute.ToString();
        }

        private void GenerateSelections(Table table)
        {
            foreach (var tableAttribute in table.Attributes)
            {
                if (tableAttribute.ConstraintType != ConstraintType.Select)
                {
                    continue;
                }

                bool hasValue = Task.Data.TryGetValue(tableAttribute.Id, out object value);
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

        private bool TryGetAssigneeEmail(Table table, out string assigneeEmail)
        {
            var assigneeAttributeId = (string)table.PurposeMetaData["assigneeAttributeId"];
            if (assigneeAttributeId == null || !Task.Data.TryGetValue(assigneeAttributeId, out object value))
            {
                assigneeEmail = null;
                return false;
            }

            assigneeEmail = (string)value;
            return true;
        }
    }
}
