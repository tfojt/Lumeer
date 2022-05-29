using Lumeer.Models.Rest;
using Lumeer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lumeer.Models
{
    public class DocumentItem : NotifyPropertyChanged
    {
        public const string DocumentPropertyName = nameof(Document);
        public const string IsSelectedPropertyName = nameof(IsSelected);

        public Document Document { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetValue(ref _isSelected, value);
        }

        public DocumentItem(Document document, bool isSelected)
        {
            Document = document;
            IsSelected = isSelected;
        }
    }
}
