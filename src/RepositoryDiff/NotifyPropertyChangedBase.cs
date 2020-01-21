using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace KsWare.RepositoryDiff
{
    [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global", Justification = "Public API")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API")]
    public class NotifyPropertyChangedBase : INotifyPropertyChanged {

        /// <summary>
        /// Sets a backing field value and if it's changed raise a notification.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="oldValue">A reference to the field to update.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property for change notifications.</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "MethodNameNotMeaningful", Justification = "Set pattern")]
        public virtual bool Set<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null) {
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue)) {
                return false;
            }

            oldValue = newValue;

            NotifyPropertyChange(propertyName ?? string.Empty);

            return true;
        }

        public bool Set<T>(Func<T> getter, Action<T> setter, T newValue, [CallerMemberName] string propertyName = null) {
            var oldValue = getter();

            if (EqualityComparer<T>.Default.Equals(oldValue, newValue)) {
                return false;
            }

            setter(newValue);

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