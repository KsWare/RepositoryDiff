using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace KsWare.RepositoryDiff.Common
{
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public API")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API")]
    [SuppressMessage("ReSharper", "MethodNameNotMeaningful", Justification = "Get/Set pattern")]
    [SuppressMessage("ReSharper", "TooManyArguments", Justification = "by design")]
    public class NotifyPropertyChangedBase : INotifyPropertyChanged {

        private readonly Dictionary<string,object> _fields=new Dictionary<string, object>();

        /// <summary>
        /// Sets a backing field value and if it's changed raise a notification.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="oldValue">A reference to the field to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property for change notifications.</param>
        /// <returns></returns>
        protected virtual bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue)) {
                return false;
            }

            oldValue = newValue;

            NotifyPropertyChange(propertyName ?? string.Empty);

            return true;
        }

        protected bool Set<T>(Func<T> getter, Action<T> setter, T newValue, [CallerMemberName] string propertyName = null) {
            var oldValue = getter();

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue)) {
                return false;
            }

            setter(newValue);

            NotifyPropertyChange(propertyName ?? string.Empty);

            return true;
        }

        protected virtual T Get<T>([CallerMemberName] string propertyName = null)
        {
            T oldValue;
            if (!_fields.TryGetValue(propertyName, out var fieldValue))
            {
                oldValue = default(T);
                _fields.Add(propertyName,oldValue);
            }
            else
            {
                oldValue = (T) fieldValue;
            }

            return oldValue;
        }

        protected virtual bool Set<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            T oldValue;
            if (!_fields.TryGetValue(propertyName, out var fieldValue))
            {
                oldValue = default(T);
                _fields.Add(propertyName,oldValue);
            }
            else
            {
                oldValue = (T) fieldValue;
            }
            
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue)) {
                return false;
            }

            _fields[propertyName] = newValue;

            NotifyPropertyChange(propertyName ?? string.Empty);

            return true;
        }



        private void NotifyPropertyChange(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;

		
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}