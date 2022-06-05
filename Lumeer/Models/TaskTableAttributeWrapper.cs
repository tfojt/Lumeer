using Lumeer.Models.Rest;
using Lumeer.Utils;
using Lumeer.Views;
using System;
using System.Collections;
using System.Linq;
using Xamarin.Forms;
using ConstraintType = Lumeer.Models.Rest.Enums.ConstraintType;

namespace Lumeer.Models
{
    public class TaskTableAttributeWrapper
    {
        private Task _task;
        private TableAttribute _tableAttribute;
        private bool _originalValueHasValue;
        private object _originalValue;
        public Cell Cell { get; private set; }

        public TaskTableAttributeWrapper(Task task, TableAttribute tableAttribute)
        {
            _task = task;
            _tableAttribute = tableAttribute;
            _originalValueHasValue = task.Data.TryGetValue(tableAttribute.Id, out object originalValue);
            _originalValue = originalValue;
            
            GenerateCell();
        }

        private void GenerateCell()
        {
            string attributeName = _tableAttribute.Name;

            switch (_tableAttribute.ConstraintType)
            {
                case ConstraintType.None:
                case ConstraintType.Text:
                    {
                        Cell = GenerateEntryCell(attributeName);
                        break;
                    }
                case ConstraintType.User:
                    {
                        var stackLayout = GenerateStackLayout();

                        var label = GenerateLabel(attributeName);
                        stackLayout.Children.Add(label);

                        var usersEmails = Session.Instance.Users.Select(u => u.Email).ToList();

                        var picker = GeneratePicker(usersEmails);

                        if (_originalValueHasValue)
                        {
                            var userEmail = (string)_originalValue;
                            string correspondingEmail = usersEmails.Single(e => e == userEmail);
                            picker.SelectedItem = correspondingEmail;
                        }

                        stackLayout.Children.Add(picker);

                        Cell = new ViewCell
                        {
                            View = stackLayout
                        };
                        break;
                    }
                case ConstraintType.DateTime:
                    {
                        var stackLayout = GenerateStackLayout();

                        var label = GenerateLabel(attributeName);
                        stackLayout.Children.Add(label);

                        var restFormat = (string)_tableAttribute.Constraint.Config["format"];
                        string format = RestParsers.ParseDateFormat(restFormat);

                        if (_originalValueHasValue)
                        {
                            if (_originalValue is string dateString)  // Api sometimes sends date in string
                            {
                                if (string.IsNullOrEmpty(dateString))
                                {
                                    _originalValueHasValue = false;
                                    _originalValue = null;
                                }
                                else
                                {
                                    var dateTime = DateTime.Parse(dateString);
                                    _originalValue = dateTime;
                                }
                            }
                        }

                        var datePicker = new NullableDatePicker((DateTime?)_originalValue, format)
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                        };

                        stackLayout.Children.Add(datePicker);

                        Cell = new ViewCell
                        {
                            View = stackLayout
                        };

                        break;
                    }
                case ConstraintType.Number:
                    {
                        Cell = GenerateEntryCell(attributeName, Keyboard.Numeric);

                        if (_originalValueHasValue)
                        {
                            _originalValue = _originalValue.ToString();   // api accepts this only as string...
                        }

                        break;
                    }
                case ConstraintType.Select:
                    {
                        var stackLayout = GenerateStackLayout();

                        var label = GenerateLabel(attributeName);
                        stackLayout.Children.Add(label);

                        var selectionListId = (string)_tableAttribute.Constraint.Config["selectionListId"];
                        var selectionList = Session.Instance.SelectionLists.Single(sl => sl.Id == selectionListId);

                        var picker = GeneratePicker(selectionList.Options);

                        if (_originalValueHasValue)
                        {
                            SelectionOption selectionOption = selectionList.Options.Single(so => so.GetValue() == (string)_originalValue);
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
                    throw new NotImplementedException($"{nameof(GenerateCell)} - {_tableAttribute.ConstraintType}");
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
            string text = _originalValueHasValue ? _originalValue.ToString() : "";

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

        private Picker GeneratePicker(IList itemsSource)
        {
            var picker = new Picker
            {
                ItemsSource = itemsSource,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            return picker;
        }

        public bool ValueChanged(out string attributeId, out object newValue)
        {
            attributeId = _tableAttribute.Id;
            bool currentValueHasValue;

            switch (Cell)
            {
                case EntryCell entryCell:
                    {
                        string currentValue = entryCell.Text;

                        switch (_tableAttribute.ConstraintType)
                        {
                            case ConstraintType.None:
                            case ConstraintType.Text:
                            case ConstraintType.Number:
                                {
                                    currentValueHasValue = !string.IsNullOrEmpty(currentValue);
                                    newValue = currentValue;
                                    break;
                                }
                            default:
                                throw new NotImplementedException($"{nameof(ValueChanged)} - {nameof(EntryCell)} - {_tableAttribute.ConstraintType}");
                        }

                        break;
                    }
                case ViewCell viewCell:
                    {
                        switch (_tableAttribute.ConstraintType)
                        {
                            case ConstraintType.DateTime:
                                {
                                    var stackLayout = (StackLayout)viewCell.View;
                                    var nullableDatePicker = (NullableDatePicker)stackLayout.Children.Single(ch => ch is NullableDatePicker);

                                    DateTime? currentValue = nullableDatePicker.NullableDate;
                                    currentValueHasValue = currentValue.HasValue;
                                    newValue = currentValue;
                                    break;
                                }
                            case ConstraintType.User:
                                {
                                    var stackLayout = (StackLayout)viewCell.View;
                                    var picker = (Picker)stackLayout.Children.Single(ch => ch is Picker);

                                    var currentValue = (string)picker.SelectedItem;
                                    currentValueHasValue = currentValue != null;
                                    newValue = currentValue;
                                    break;
                                }
                            case ConstraintType.Select:
                                {
                                    var stackLayout = (StackLayout)viewCell.View;
                                    var picker = (Picker)stackLayout.Children.Single(ch => ch is Picker);

                                    var currentValue = (SelectionOption)picker.SelectedItem;
                                    currentValueHasValue = currentValue != null;
                                    newValue = currentValue?.Value;
                                    break;
                                }
                            default:
                                throw new NotImplementedException($"{nameof(ValueChanged)} - {nameof(ViewCell)} - {_tableAttribute.ConstraintType}");
                        }

                        break;
                    }
                default:
                    throw new NotImplementedException($"{nameof(ValueChanged)} - {Cell.GetType()}");
            }

            if (_originalValueHasValue)
            {
                return !_originalValue.Equals(newValue);
            }

            if (currentValueHasValue)
            {
                return !newValue.Equals(_originalValue);
            }

            return false;
        }
    }
}
