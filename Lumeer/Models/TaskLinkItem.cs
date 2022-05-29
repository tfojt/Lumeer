using Lumeer.Fonts;
using Lumeer.Models.Rest;
using Lumeer.Services;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Task = System.Threading.Tasks.Task;

namespace Lumeer.Models
{
    public class TaskLinkItem : NotifyPropertyChanged
    {
        public string Name { get; set; }

        private bool _editing;
        public bool Editing
        {
            get => _editing;
            set => SetValue(ref _editing, value);
        }
        
        private bool _madeChanges;
        public bool MadeChanges
        {
            get => _madeChanges;
            set => SetValue(ref _madeChanges, value);
        }
        
        private bool _contentVisible;
        public bool ContentVisible
        {
            get => _contentVisible;
            set => SetValue(ref _contentVisible, value);
        }
        
        private bool _allSelected;
        public bool AllSelected
        {
            get => _allSelected;
            set
            {
                if (_allSelected == value)
                {
                    return;
                }

                ChangeAllSelected(value, true);
            }
        }

        public ICommand EditingCmd => new Command(ChangeEditing);
        public IAsyncCommand SaveCmd => new AsyncCommand(SaveChanges);

        public FontImageData CurrentTableFontImageData { get; set; }
        public FontImageData LinkedTableFontImageData { get; set; }

        public View LinkedTableAttributesView { get; set; }

        public DataTemplate LinkedTableDocumentDataTemplate { get; set; }
        public List<DocumentItem> OriginalLinkedTableDocuments { get; set; } = new List<DocumentItem>();
        public ObservableCollection<DocumentItem> DisplayedLinkedTableDocuments { get; set; } = new ObservableCollection<DocumentItem>();

        private List<DocumentItem> _beforeEditingDisplayedLinkedTableDocuments = new List<DocumentItem>();

        private readonly IAlertService _alertService;

        private Rest.Task _currentTask;
        private LinkType _currentLinkType;
        private List<Link> _relevantLinks;

        public TaskLinkItem(string name, Table currentTable, Table linkedTable, List<Document> allLinkedTableDocuments, List<Link> relevantLinks, Rest.Task currentTask, LinkType currentLinkType)
        {
            Name = name;
            _currentTask = currentTask;
            _currentLinkType = currentLinkType;
            _relevantLinks = relevantLinks;

            _alertService = DependencyService.Get<IAlertService>();

            CurrentTableFontImageData = new FontImageData(FontAwesomeAliases.PRO_REGULAR, FontAwesomeIcons.CircleQuestion, currentTable.Color);
            LinkedTableFontImageData = new FontImageData(FontAwesomeAliases.PRO_REGULAR, FontAwesomeIcons.CircleQuestion, linkedTable.Color);

            var linkedTableDocumentIds = new List<string>();
            foreach (var relevantLink in relevantLinks)
            {
                string linkedTableDocumentId = relevantLink.DocumentIds.Single(id => id != _currentTask.Id);
                linkedTableDocumentIds.Add(linkedTableDocumentId);
            }

            foreach (var document in allLinkedTableDocuments)
            {
                bool isSelected = linkedTableDocumentIds.Contains(document.Id);
                var documentItem = new DocumentItem(document, isSelected);
                OriginalLinkedTableDocuments.Add(documentItem);

                if (isSelected)
                {
                    DisplayedLinkedTableDocuments.Add(documentItem);
                }
            }

            LinkedTableAttributesView = GenerateLinkedTableAttributesView(linkedTable);

            LinkedTableDocumentDataTemplate = GenerateLinkedTableDocumentDataTemplate(linkedTable);
        }

        private void ChangeEditing()
        {
            bool newEditingValue = !Editing;
            if (newEditingValue)
            {
                _beforeEditingDisplayedLinkedTableDocuments.AddRange(DisplayedLinkedTableDocuments, true);
                DisplayedLinkedTableDocuments.AddRange(OriginalLinkedTableDocuments, true);

                Editing = true; // optimization for CheckBox

                if (!ContentVisible)
                {
                    ContentVisible = true;
                }
            }
            else
            {
                Editing = false; // optimization for CheckBox

                if (MadeChanges)    // revert selection changes
                {
                    foreach (var document in _beforeEditingDisplayedLinkedTableDocuments)
                    {
                        document.IsSelected = true;
                    }

                    var wasNotSelectedDocuments = OriginalLinkedTableDocuments.Except(_beforeEditingDisplayedLinkedTableDocuments);
                    foreach (var document in wasNotSelectedDocuments)
                    {
                        document.IsSelected = false;
                    }
                }

                DisplayedLinkedTableDocuments.AddRange(_beforeEditingDisplayedLinkedTableDocuments, true);
            }
        }

        private async Task SaveChanges()
        {
            Editing = false;

            var removedLinkIds = new List<string>();
            var createdLinkedTableDocuments = new List<NewLink>();

            for (int i = DisplayedLinkedTableDocuments.Count - 1; i > -1; i--)
            {
                var documentItem = DisplayedLinkedTableDocuments[i];
                if (documentItem.IsSelected)
                {
                    if (!_beforeEditingDisplayedLinkedTableDocuments.Contains(documentItem))    // created
                    {
                        var newLink = new NewLink(_currentTask.Id, documentItem.Document.Id, _currentLinkType.Id);
                        createdLinkedTableDocuments.Add(newLink);
                    }
                }
                else
                {
                    if (_beforeEditingDisplayedLinkedTableDocuments.Contains(documentItem))    // removed
                    {
                        var link = _relevantLinks.Single(l => l.DocumentIds.Contains(documentItem.Document.Id));
                        removedLinkIds.Add(link.Id);
                        _relevantLinks.Remove(link);
                    }

                    DisplayedLinkedTableDocuments.Remove(documentItem);
                }
            }

            try
            {
                List<Link> partiallyCreatedLinks = await ApiClient.Instance.EditDocumentLinks(_currentLinkType.Id, _currentTask.Id, removedLinkIds, createdLinkedTableDocuments);
                if (partiallyCreatedLinks.Count > 0)
                {
                    var partiallyCreatedLinkIds = partiallyCreatedLinks.Select(l => l.Id).ToArray();
                    var fullyCreatedLinks = await ApiClient.Instance.GetActualLinks(partiallyCreatedLinkIds);
                    _relevantLinks.AddRange(fullyCreatedLinks);
                }
            }
            catch (Exception ex)
            {
                await _alertService.DisplayAlert("Error", "Sorry, there was an error while editing selected links", "Ok", ex);
            }
        }

        private void ChangeAllSelected(bool newValue, bool influenceOthers)
        {
            SetValue(ref _allSelected, newValue, nameof(AllSelected));

            if (influenceOthers)
            {
                foreach (var document in DisplayedLinkedTableDocuments)
                {
                    document.IsSelected = _allSelected;
                }
            }
        }

        private View GenerateLinkedTableAttributesView(Table linkedTable)
        {
            var grid = GenerateGrid();

            var checkBox = GenerateCheckBox();
            checkBox.SetBinding(VisualElement.IsVisibleProperty, nameof(Editing));
            checkBox.SetBinding(CheckBox.IsCheckedProperty, nameof(AllSelected));

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.Children.Add(checkBox, 0, 0);

            foreach (var tableAttribute in linkedTable.Attributes)
            {
                var stackLayout = new StackLayout
                {
                    BackgroundColor = LinkedTableFontImageData.Color
                };

                var label = GenerateLabel();
                label.Text = tableAttribute.Name;

                stackLayout.Children.Add(label);

                grid.Children.AddHorizontal(stackLayout);
            }

            return grid;
        }

        private DataTemplate GenerateLinkedTableDocumentDataTemplate(Table linkedTable)
        {
            return new DataTemplate(() =>
            {
                var grid = GenerateGrid();

                var checkBox = GenerateCheckBox();
                checkBox.SetBinding(VisualElement.IsVisibleProperty, new Binding(nameof(Editing), source: this));
                checkBox.SetBinding(CheckBox.IsCheckedProperty, DocumentItem.IsSelectedPropertyName);
                checkBox.CheckedChanged += CheckBox_CheckedChanged;

                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                grid.Children.Add(checkBox, 0, 0);

                for (int i = 1; i < linkedTable.Attributes.Count + 1; i++)
                {
                    var label = GenerateLabel();

                    label.SetBinding(Label.TextProperty, $"{DocumentItem.DocumentPropertyName}.Data[a{i}]");

                    grid.Children.AddHorizontal(label);
                }

                return new ViewCell
                {
                    View = grid
                };
            });
        }

        private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (AllSelected)
            {
                if (!e.Value)
                {
                    ChangeAllSelected(false, false);
                }
            }
            else
            {
                if (OriginalLinkedTableDocuments.Count > 0 &&
                    OriginalLinkedTableDocuments.All(d => d.IsSelected))
                {
                    ChangeAllSelected(true, false);
                }
            }

            MadeChanges = _beforeEditingDisplayedLinkedTableDocuments.Any(d => !d.IsSelected) ||
                _beforeEditingDisplayedLinkedTableDocuments.Count != OriginalLinkedTableDocuments.Count(d => d.IsSelected);
        }

        private Grid GenerateGrid()
        {
            return new Grid
            {
                ColumnSpacing = 1
            };
        }

        private Label GenerateLabel()
        {
            return new Label
            {
                Margin = new Thickness(5),
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Black
            };
        }

        private CheckBox GenerateCheckBox()
        {
            return new CheckBox
            {
                IsVisible = false,
            };
        }
    }
}
