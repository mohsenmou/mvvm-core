﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Mohsenmou.MVVM.Core
{
    public class ChangeTrackingCollection<T> : ObservableCollection<T>, IValidatableTrackingObject where T : class, IValidatableTrackingObject
    {
        private readonly ObservableCollection<T> _addedItems;
        private readonly ObservableCollection<T> _modifiedItems;
        private readonly ObservableCollection<T> _removedItems;
        private IList<T> _originalCollection;
        public ChangeTrackingCollection(IEnumerable<T> items) : base(items)
        {
            _originalCollection = this.ToList();

            AttachItemPropertyChangedHandler(_originalCollection);

            _addedItems = new ObservableCollection<T>();
            _removedItems = new ObservableCollection<T>();
            _modifiedItems = new ObservableCollection<T>();

            AddedItems = new ReadOnlyObservableCollection<T>(_addedItems);
            RemovedItems = new ReadOnlyObservableCollection<T>(_removedItems);
            ModifiedItems = new ReadOnlyObservableCollection<T>(_modifiedItems);
        }
        public ReadOnlyObservableCollection<T> AddedItems { get; private set; }
        public bool IsChanged => AddedItems.Count > 0 || RemovedItems.Count > 0 || ModifiedItems.Count > 0;
        public bool IsValid => this.All(t => t.IsValid);
        public ReadOnlyObservableCollection<T> ModifiedItems { get; private set; }
        public ReadOnlyObservableCollection<T> RemovedItems { get; private set; }
        public void AcceptChanges()
        {
            _addedItems.Clear();
            _removedItems.Clear();
            _modifiedItems.Clear();
            foreach (var item in this)
            {
                item.AcceptChanges();
            }
            _originalCollection = this.ToList();
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }
        public void RejectChanges()
        {
            foreach (var addItem in _addedItems.ToList())
            {
                Remove(addItem);
            }
            foreach (var removedItem in _removedItems.ToList())
            {
                Add(removedItem);
            }
            foreach (var modifiedItem in _modifiedItems.ToList())
            {
                modifiedItem.RejectChanges();
            }
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
        }
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var added = this.Where(current => _originalCollection.All(orig => orig != current));
            var removed = _originalCollection.Where(orig => this.All(current => current != orig));
            var modified = this.Except(added).Except(removed).Where(item => item.IsChanged).ToList();

            AttachItemPropertyChangedHandler(added);
            DetachItemPropertyChangedHandler(removed);

            UpdateObservableColection(_addedItems, added);
            UpdateObservableColection(_removedItems, removed);
            UpdateObservableColection(_modifiedItems, modified);

            base.OnCollectionChanged(e);
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
        }
        private void AttachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged += ItemPropertyChanged;
            }
        }
        private void DetachItemPropertyChangedHandler(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                item.PropertyChanged -= ItemPropertyChanged;
            }
        }
        private void ItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsValid))
            {
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsValid)));
            }
            else
            {
                var item = (T)sender;
                if (_addedItems.Contains(item))
                {
                    return;
                }
                if (item.IsChanged)
                {
                    if (!_modifiedItems.Contains(item))
                    {
                        _modifiedItems.Add(item);
                    }
                }
                else
                {
                    if (_modifiedItems.Contains(item))
                    {
                        _modifiedItems.Remove(item);
                    }
                }
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsChanged)));
            }
        }
        private void UpdateObservableColection(ObservableCollection<T> collection, IEnumerable<T> items)
        {
            collection.Clear();
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
