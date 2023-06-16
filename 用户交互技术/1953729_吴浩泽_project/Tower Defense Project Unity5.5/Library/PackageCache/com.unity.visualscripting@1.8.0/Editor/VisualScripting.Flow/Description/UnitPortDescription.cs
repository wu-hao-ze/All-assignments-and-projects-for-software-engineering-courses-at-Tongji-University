using System;

namespace Unity.VisualScripting
{
    public sealed class UnitPortDescription : IDescription
    {
        private string _label;

        private bool _isLabelVisible = true;

        internal IUnitPort portType;

        public EditorTexture icon
        {
            get
            {
                if (_icon == null || !_icon.IsValid())
                {
                    _icon = GetIcon(portType);
                }

                return _icon;
            }
            set => _icon = value;
        }

        private EditorTexture _icon;

        public string fallbackLabel { get; set; }

        public string label
        {
            get => _label ?? fallbackLabel;
            set => _label = value;
        }

        public bool showLabel
        {
            get => !BoltFlow.Configuration.hidePortLabels || _isLabelVisible;
            set => _isLabelVisible = value;
        }

        string IDescription.title => label;

        public string summary { get; set; }

        public Func<Metadata, Metadata> getMetadata { get; set; }

        public void CopyFrom(UnitPortDescription other)
        {
            _label = other._label;
            _isLabelVisible = other._isLabelVisible;
            summary = other.summary;
            portType = other.portType ?? portType;
            getMetadata = other.getMetadata ?? getMetadata;
        }

        private static EditorTexture GetIcon(IUnitPort portType)
        {
            if (portType is IUnitControlPort)
            {
                return typeof(Flow).Icon();
            }
            else if (portType is IUnitValuePort)
            {
                return Icons.Type(((IUnitValuePort)portType).type);
            }
            else if (portType is IUnitInvalidPort)
            {
                return BoltCore.Resources.icons.errorState;
            }
            else
            {
                // throw new NotSupportedException();
                return null;
            }
        }
    }
}
