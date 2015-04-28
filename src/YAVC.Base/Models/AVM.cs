using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using YAVC.Base.Util;

namespace YAVC.Base.Models {
	public abstract class AVM : ANotifiable {

        private static object typeLock = new object();
        private static readonly Dictionary<Type, List<string>> TypeProperties = new Dictionary<Type, List<string>>();
        private Type MyType;
        
        public IController TheController { get; protected set; }
		private readonly Dictionary<string, object> PropertyValues = new Dictionary<string, object>();

		protected AVM(IController c) {
			TheController = c;
            IntializePublicProperties(this);
		}

        private static void IntializePublicProperties(AVM me)
        {
            me.MyType = me.GetType();

            lock (typeLock)
            {
                if (TypeProperties.ContainsKey(me.MyType)) return;

                var publicProperties = me.MyType.GetRuntimeProperties();
                var publicPropertyNames = publicProperties.Where(p => p.CanRead).Select(p => p.Name);

                TypeProperties[me.MyType] = new List<string>(publicPropertyNames);
            }
        }

        public static string GetPropertyName<T>(Expression<Func<T>> propReference)
        {
            return ((MemberExpression)propReference.Body).Member.Name;
        }

		protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            if (PropertyValues.ContainsKey(propertyName))
                return (T)PropertyValues[propertyName];

            return default(T);
        }

        protected void Notify<T>(Expression<Func<T>> propertyReference)
        {
            var propname = GetPropertyName(propertyReference);

            NotifyChanged(propname);
        }

        protected void NotifyAll()
        {
            UI.Invoke(() =>
            {
                foreach (var property in TypeProperties[MyType])
                {
                    NotifyChanged(property);
                }
            });
        }

        /// <summary>
        /// Sets the value of the property specified by propertyName.<para>
        /// If the value is different (via object.Equals(value, oldvalue)) then
        /// Notify(propertyName) will be called to let listeners know the property has changed.</para>
        /// </summary>
        /// <param name="value">The new value of the property you're trying to set.</param>
        /// <param name="propertyName">The name of the property you want to change. If calling from
        /// a property Setter, you can omit this value to change the property you're currently setting.</param>
        /// <returns>True if the existing value was changed, false otherwise.</returns>
        protected bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            var shouldNotify = !PropertyValues.ContainsKey(propertyName) || !object.Equals(value, PropertyValues[propertyName]);

            PropertyValues[propertyName] = value;

            if (shouldNotify)
            {
                NotifyChanged(propertyName);
                return true; //- Value has changed
            }
            return false;
        }
        
    }
}
