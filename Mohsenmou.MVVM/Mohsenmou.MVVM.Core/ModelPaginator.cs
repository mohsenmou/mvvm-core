using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Mohsenmou.MVVM.Core
{
    public abstract class ModelPaginator<T> : DocumentPaginator, INotifyPropertyChanged
    {
        private double _fontSize;
        private ObservableCollection<T> _items;
        private double _margin;
        private int _pageCount;
        private Size _pageSize;
        private int _rowsPerPage;
        private Typeface _typeface;
        public ModelPaginator(ObservableCollection<T> items, Typeface typeface, double fontSize, double margin, Size pageSize)
        {
            _items = items;
            _typeface = typeface;
            _fontSize = fontSize;
            _margin = margin;
            _pageSize = pageSize;
            PaginateData();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public double FontSize
        {
            get { return _fontSize; }
            set
            {
                if (_fontSize != value)
                {
                    _fontSize = value;
                    OnPropertyChanged();
                }
            }
        }
        public override bool IsPageCountValid => true;
        public ObservableCollection<T> Items
        {
            get { return _items; }
            set
            {
                if (_items != value)
                {
                    _items = value;
                    OnPropertyChanged();
                }
            }
        }
        public double Margin
        {
            get { return _margin; }
            set
            {
                if (_margin != value)
                {
                    _margin = value;
                    OnPropertyChanged();
                }
            }
        }
        public override int PageCount => _pageCount;
        public override Size PageSize
        {
            get { return _pageSize; }
            set
            {
                if (_pageSize != value)
                {
                    _pageSize = value;
                    OnPropertyChanged();
                    PaginateData();
                }
            }
        }
        public int RowsPerPage => _rowsPerPage;
        public override IDocumentPaginatorSource Source => null;
        public Typeface Typeface
        {
            get { return _typeface; }
            set
            {
                if (_typeface != value)
                {
                    _typeface = value;
                    OnPropertyChanged();
                }
            }
        }
        public FormattedText GetFormattedText(string text) => GetFormattedText(text, _typeface);
        public FormattedText GetFormattedText(string text, Typeface typeface) =>
            new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, typeface, _fontSize, Brushes.Black, 1.25);
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void PaginateData()
        {
            FormattedText text = GetFormattedText("A");
            _rowsPerPage = (int)((_pageSize.Height - _margin * 2) / text.Height);
            _rowsPerPage -= 1;
            _pageCount = (int)Math.Ceiling((double)_items.Count / _rowsPerPage);
        }
    }
}