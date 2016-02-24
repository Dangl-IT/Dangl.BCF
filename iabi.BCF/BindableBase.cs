using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace iabi.BCF
{
    /// <summary>
    /// Implements <see cref="INotifyPropertyChanged"/> and <see cref="IDisposable"/>.
    /// </summary>
    public abstract class BindableBase : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Protected parameterless constructor.
        /// </summary>
        protected BindableBase()
        {
        }

        /// <summary>
        /// <see cref="INotifyPropertyChanged"/> implementation.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Checks if a property already matches a desired value.  Sets the property and
        ///     notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="Storage">Reference to a property with both getter and setter.</param>
        /// <param name="Value">Desired value for the property.</param>
        /// <param name="PropertyName">
        ///     Name of the property used to notify listeners.  This
        ///     value is optional and can be provided automatically when invoked from compilers that
        ///     support CallerMemberName.
        /// </param>
        /// <returns>
        ///     True if the value was changed, false if the existing value matched the
        ///     desired value.
        /// </returns>
        protected bool SetProperty<T>(ref T Storage, T Value, [CallerMemberName] String PropertyName = null)
        {
            if (Equals(Storage, Value))
            {
                return false;
            }

            Storage = Value;
            OnPropertyChanged(PropertyName);
            return true;
        }

        /// <summary>
        /// Event to be raised for <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        /// <param name="PropertyName">Optional, when not given the <see cref="System.Runtime.CompilerServices.CallerMemberNameAttribute"/> is used to determine
        /// the calling function.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null)
            {
                Handler(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        /// <summary>
        /// Returns the name of a property as string.
        /// Must be called in the form of:
        /// GetPropertyName(() => this.Property);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="GivenProperty">The property for which to return the string.</param>
        /// <returns></returns>
        public string GetPropertyName<T>(Expression<Func<T>> GivenProperty)
        {
            return ((MemberExpression)GivenProperty.Body).Member.Name;
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/>. Will always call the <see cref="OnDispose"/> method that
        /// may be used in derived classes to implement behaviour upon being disposed, such as releasing event
        /// handler listeners.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// This method is called by the <see cref="Dispose"/> method upon disposing of
        /// this class via the <see cref="IDisposable"/> interface.
        /// </summary>
        protected virtual void OnDispose()
        {
        }
    }
}