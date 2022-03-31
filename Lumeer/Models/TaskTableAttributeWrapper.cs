using Lumeer.CustomViews;
using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using Xamarin.Forms;

namespace Lumeer.Models
{

    public class TaskTableAttributeWrapper
    {
        public Task Task { get; }
        public TableAttribute TableAttribute { get; }
        public AttributeType AttributeType { get; }
        public bool OriginalValueHasValue { get; }
        public object OriginalValue { get; }
        public object CurrentValue { get; }
        public Cell Cell { get; }

        public TaskTableAttributeWrapper(Task task, TableAttribute tableAttribute)
        {
            Task = task;
            TableAttribute = tableAttribute;
            OriginalValueHasValue = task.Data.TryGetValue(tableAttribute.Id, out object originalValue);
            OriginalValue = originalValue;
            AttributeType = (AttributeType)Enum.Parse(typeof(AttributeType), tableAttribute.Constraint.Type);
            Cell = GenerateCell();
        }

        private Cell GenerateCell()
        {
            Cell cell;
            string attributeName = TableAttribute.Name;

            switch (AttributeType)
            {
                case AttributeType.User:    // TODOT Handle user hints
                case AttributeType.Text:
                    {
                        string text = OriginalValueHasValue ? (string)OriginalValue : "";
                        cell = new EntryCell
                        {
                            Label = attributeName,
                            Text = text
                        };
                        break;
                    }
                case AttributeType.DateTime:
                    {
                        var stackLayout = new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Padding = new Thickness(13, 0)
                        };

                        var label = new Label
                        {
                            Text = attributeName,
                            VerticalOptions = LayoutOptions.Center
                        };
                        stackLayout.Children.Add(label);

                        var datePicker = new NullableDatePicker
                        {
                            HorizontalOptions = LayoutOptions.CenterAndExpand
                        };
                        if (OriginalValueHasValue && !(OriginalValue is string dateString && string.IsNullOrEmpty(dateString)))
                        {
                            datePicker.NullableDate = (DateTime)OriginalValue;  // TODOT handle format
                        }
                        stackLayout.Children.Add(datePicker);

                        // TODOT sometimes TimerPicker

                        cell = new ViewCell
                        {
                            View = stackLayout
                        };
                        break;
                    }
                default:
                    throw new NotImplementedException(nameof(AttributeType));
            }

            return cell;
        }

        public bool ValueChanged(out object newValue)
        {
            switch (Cell)
            {
                case EntryCell entryCell:
                    {
                        string currentValue = entryCell.Text;
                        bool currentValueHasValue = !string.IsNullOrEmpty(currentValue);

                        if (!OriginalValueHasValue && !currentValueHasValue)
                        {
                            newValue = null;
                            return false;
                        }
                        if (!OriginalValueHasValue && currentValueHasValue)
                        {
                            newValue = currentValue;
                            return true;
                        }
                        if (OriginalValueHasValue && !currentValueHasValue)
                        {
                            newValue = "";
                            return true;
                        }
                        if (OriginalValueHasValue && currentValueHasValue)
                        {
                            string originalValue = (string)OriginalValue;
                            
                            newValue = currentValue;
                            return originalValue != currentValue;
                        }

                        throw new NotImplementedException(nameof(EntryCell));
                    }
                default:
                    throw new NotImplementedException(Cell.GetType().ToString());
            }
        }
    }
}
