using Lumeer.CustomViews;
using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Linq;
using Xamarin.Forms;

namespace Lumeer.Models
{

    public class TaskTableAttributeWrapper
    {
        public Task Task { get; }
        public TableAttribute TableAttribute { get; }
        public AttributeType AttributeType { get; }
        public bool OriginalValueHasValue { get; private set; }
        public object OriginalValue { get; private set; }
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
                        if (OriginalValueHasValue)
                        {
                            if (OriginalValue is string dateString)  // Api sometimes sends date in string
                            {
                                if (string.IsNullOrEmpty(dateString))
                                {
                                    OriginalValueHasValue = false;
                                    OriginalValue = null;
                                }
                                else
                                {
                                    datePicker.NullableDate = DateTime.Parse(dateString);
                                    OriginalValue = datePicker.NullableDate;
                                }
                            }
                            else
                            {
                                datePicker.NullableDate = (DateTime)OriginalValue;  // TODOT handle format
                            }
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
            bool currentValueHasValue;

            switch (Cell)
            {
                case EntryCell entryCell:
                    {
                        string currentValue = entryCell.Text;
                        currentValueHasValue = !string.IsNullOrEmpty(currentValue);
                        newValue = currentValue;
                        break;
                    }
                case ViewCell viewCell:
                    {
                        if (AttributeType == AttributeType.DateTime)
                        {
                            var stackLayout = (StackLayout)viewCell.View;
                            var nullableDatePicker = (NullableDatePicker)stackLayout.Children.Single(ch => ch is NullableDatePicker);

                            DateTime? currentValue = nullableDatePicker.NullableDate;
                            currentValueHasValue = currentValue.HasValue;
                            newValue = currentValue;
                            break;
                        }

                        throw new NotImplementedException($"{nameof(ViewCell)} - {AttributeType}");
                    }
                default:
                    throw new NotImplementedException(Cell.GetType().ToString());
            }

            if (OriginalValueHasValue)
            {
                return !OriginalValue.Equals(newValue);
            }

            if (currentValueHasValue)
            {
                return !newValue.Equals(OriginalValue);
            }

            return false;
        }
    }
}
