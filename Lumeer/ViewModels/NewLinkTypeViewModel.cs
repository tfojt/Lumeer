using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace Lumeer.ViewModels
{
    public class NewLinkTypeViewModel : NotifyPropertyChanged
    {
        public delegate void CreatingDoneEventHandler(NewLinkType newLinkType);
        public event CreatingDoneEventHandler CreatingDone;

        public ICommand CreateCmd => new Command(Create);
        public ICommand CancelCmd => new Command(Cancel);

        public List<Table> Tables { get; set; }

        private Table _selectedTable;
        public Table SelectedTable
        {
            get => _selectedTable;
            set
            {
                SetValue(ref _selectedTable, value);
                Name = $"{_currentTable.Name} {_selectedTable.Name}";
                CheckCanCreate();
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                SetValue(ref _name, value);
                CheckCanCreate();
            }
        }

        private bool _canCreate = false;
        public bool CanCreate
        {
            get => _canCreate;
            set => SetValue(ref _canCreate, value);
        }

        private Table _currentTable;

        public NewLinkTypeViewModel(Table currentTable)
        {
            _currentTable = currentTable;

            var tablesToSelect = new List<Table>(Session.Instance.AllTables);
            tablesToSelect.Remove(currentTable);

            Tables = tablesToSelect;
        }

        private void CheckCanCreate()
        {
            bool nameValid = !string.IsNullOrEmpty(_name) && _name.Length > 2;
            bool tableSelected = _selectedTable != null;

            CanCreate = nameValid && tableSelected;
        }

        private void Create()
        {
            var newLinkType = new NewLinkType(Name, _currentTable.Id, SelectedTable.Id);
            CreatingDone?.Invoke(newLinkType);
        }

        private void Cancel()
        {
            CreatingDone?.Invoke(null);
        }
    }
}
