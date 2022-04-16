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
        public Cell Cell { get; private set; }

        public TaskTableAttributeWrapper(Task task, TableAttribute tableAttribute)
        {
            Task = task;
            TableAttribute = tableAttribute;
            OriginalValueHasValue = task.Data.TryGetValue(tableAttribute.Id, out object originalValue);
            OriginalValue = originalValue;
            AttributeType = ParseAttributeType(tableAttribute);
            
            GenerateCell();
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

        private void GenerateCell()
        {
            string attributeName = TableAttribute.Name;

            switch (AttributeType)
            {
                case AttributeType.None:
                case AttributeType.User:    // TODOT Handle user hints
                case AttributeType.Text:
                    {
                        Cell = GenerateEntryCell(attributeName);
                        break;
                    }
                case AttributeType.DateTime:
                    {
                        var stackLayout = GenerateStackLayout();

                        var label = GenerateLabel(attributeName);

                        stackLayout.Children.Add(label);

                        string dateFormat = (string)TableAttribute.Constraint.Config["format"];

                        var datePicker = new NullableDatePicker
                        {
                            Format = dateFormat,
                            HorizontalOptions = LayoutOptions.FillAndExpand
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
                                    var dateTime = DateTime.Parse(dateString);
                                    datePicker.NullableDate = dateTime;
                                    OriginalValue = dateTime;
                                }
                            }
                            else
                            {
                                datePicker.NullableDate = (DateTime)OriginalValue;  // TODOT handle format
                            }
                        }
                        stackLayout.Children.Add(datePicker);

                        // TODOT sometimes TimerPicker

                        Cell = new ViewCell
                        {
                            View = stackLayout
                        };

                        break;
                    }
                case AttributeType.Number:
                    {
                        // TODOT check string or int
                        Cell = GenerateEntryCell(attributeName, Keyboard.Numeric);

                        if (OriginalValueHasValue)
                        {
                            OriginalValue = OriginalValue.ToString();   // api accepts this only as string...
                        }

                        break;
                    }
                case AttributeType.Select:
                    {
                        var stackLayout = GenerateStackLayout();

                        var label = GenerateLabel(attributeName);
                        stackLayout.Children.Add(label);

                        var selectionListId = (string)TableAttribute.Constraint.Config["selectionListId"];
                        var selectionList = Session.Instance.SelectionLists.Single(sl => sl.Id == selectionListId);

                        var picker = new Picker
                        {
                            ItemsSource = selectionList.Options,
                            HorizontalOptions = LayoutOptions.FillAndExpand
                        };

                        if (OriginalValueHasValue)
                        {
                            SelectionOption selectionOption = selectionList.Options.Single(so => so.GetValue() == (string)OriginalValue);
                            picker.SelectedItem = selectionOption;
                        }

                        stackLayout.Children.Add(picker);

                        Cell = new ViewCell
                        {
                            View = stackLayout
                        };

                        break;
                    }
                default:
                    throw new NotImplementedException($"{nameof(GenerateCell)} - {AttributeType}");
            }
        }

        private StackLayout GenerateStackLayout(StackOrientation orientation = StackOrientation.Horizontal)
        {
            var stackLayout = new StackLayout
            {
                Orientation = orientation,
                Padding = new Thickness(13, 0)
            };

            return stackLayout;
        }

        private Label GenerateLabel(string text)
        {
            var label = new Label
            {
                Text = text,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Start
            };

            return label;
        }

        private EntryCell GenerateEntryCell(string label, Keyboard keyboard = null)
        {
            string text = OriginalValueHasValue ? OriginalValue.ToString() : "";

            var entryCell = new EntryCell
            {
                Label = label,
                Text = text,
            };

            if (keyboard != null)
            {
                entryCell.Keyboard = keyboard;
            }

            return entryCell;
        }

        public bool ValueChanged(out object newValue)
        {
            bool currentValueHasValue;

            switch (Cell)
            {
                case EntryCell entryCell:
                    {
                        string currentValue = entryCell.Text;

                        switch (AttributeType)
                        {
                            case AttributeType.None:
                            case AttributeType.Text:
                            case AttributeType.User:
                            case AttributeType.Number:
                                {
                                    currentValueHasValue = !string.IsNullOrEmpty(currentValue);
                                    newValue = currentValue;
                                    break;
                                }
                            /*case AttributeType.Number:
                                {
                                    currentValueHasValue = long.TryParse(currentValue, out long currentParsedValue);
                                    newValue = currentParsedValue;
                                    break;
                                }*/
                            default:
                                throw new NotImplementedException($"{nameof(ValueChanged)} - {nameof(EntryCell)} - {AttributeType}");
                        }

                        break;
                    }
                case ViewCell viewCell:
                    {
                        switch (AttributeType)
                        {
                            case AttributeType.DateTime:
                                {
                                    var stackLayout = (StackLayout)viewCell.View;
                                    var nullableDatePicker = (NullableDatePicker)stackLayout.Children.Single(ch => ch is NullableDatePicker);

                                    DateTime? currentValue = nullableDatePicker.NullableDate;
                                    currentValueHasValue = currentValue.HasValue;
                                    newValue = currentValue;
                                    break;
                                }
                            case AttributeType.Select:
                                {
                                    var stackLayout = (StackLayout)viewCell.View;
                                    var picker = (Picker)stackLayout.Children.Single(ch => ch is Picker);

                                    var currentValue = (SelectionOption)picker.SelectedItem;
                                    currentValueHasValue = currentValue != null;
                                    newValue = currentValue?.Value;
                                    break;
                                }
                            default:
                                throw new NotImplementedException($"{nameof(ValueChanged)} - {nameof(ViewCell)} - {AttributeType}");
                        }

                        break;
                    }
                default:
                    throw new NotImplementedException($"{nameof(ValueChanged)} - {Cell.GetType()}");
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
