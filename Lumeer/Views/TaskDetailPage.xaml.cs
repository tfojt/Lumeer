using Lumeer.CustomViews;
using Lumeer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Lumeer.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TaskDetailPage : ContentPage
    {
        public TaskDetailPage(Models.Rest.Task task, Models.Rest.Table table)
        {
            BindingContext = new TaskDetailViewModel(task, table);

            InitializeComponent();

            foreach (var attribute in table.Attributes)
            {
                var cell = GenerateCell(attribute, task);
                this.tableSection.Add(cell);
            }
        }

        private Cell GenerateCell(Models.Rest.TableAttribute tableAttribute, Models.Rest.Task task)
        {
            Cell cell;
            string attributeName = tableAttribute.Name;
            bool attributeDataHasValue = task.Data.TryGetValue(tableAttribute.Id, out object attributeData);

            string type = tableAttribute.Constraint.Type;
            var attributeType = (AttributeType)Enum.Parse(typeof(AttributeType), type);
            switch (attributeType)
            {
                case AttributeType.User:    // TODOT Handle user hints
                case AttributeType.Text:
                    {
                        string text = attributeDataHasValue ? (string)attributeData : "";
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
                        if (attributeDataHasValue && !(attributeData is string))
                        {
                            datePicker.NullableDate = (DateTime)attributeData;  // TODOT handle format
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
                    throw new NotImplementedException(type);
            }

            return cell;
        }

        private enum AttributeType
        {
            Text,
            User,
            DateTime
        }
    }
}