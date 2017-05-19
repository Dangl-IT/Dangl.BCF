using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace iabi.BCF
{
    /// <summary>
    ///     Implements <see cref="INotifyPropertyChanged" /> and <see cref="IDisposable" />.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        ///     Implementation of <see cref="IDisposable" />. Will always call the <see cref="OnDispose" /> method that
        ///     may be used in derived classes to implement behaviour upon being disposed, such as releasing event
        ///     handler listeners.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        ///     <see cref="INotifyPropertyChanged" /> implementation.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        ///     Event to be raised for <see cref="INotifyPropertyChanged" />.
        /// </summary>
        /// <param name="propertyName">
        ///     Optional, when not given the <see cref="System.Runtime.CompilerServices.CallerMemberNameAttribute" /> is used to
        ///     determine
        ///     the calling function.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Returns the name of a property as string.
        ///     Must be called in the form of:
        ///     GetPropertyName(() => this.Property);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property">The property for which to return the string.</param>
        /// <returns></returns>
        public string GetPropertyName<T>(Expression<Func<T>> property)
        {
            return ((MemberExpression) property.Body).Member.Name;
        }

        /// <summary>
        ///     This method is called by the <see cref="Dispose" /> method upon disposing of
        ///     this class via the <see cref="IDisposable" /> interface.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}