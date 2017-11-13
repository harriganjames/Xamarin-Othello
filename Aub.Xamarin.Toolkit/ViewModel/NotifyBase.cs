using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Aub.Xamarin.Toolkit.ViewModel
{
    public class NotifyBase : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public static string GetPropertyName<T>(Expression<Func<T,object>> propertyExpression)
        {
            string name = null;
            var lambda = (LambdaExpression)propertyExpression;
            MemberExpression memberExpression = null;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else if (lambda.Body is MemberExpression)
            {
                memberExpression = (MemberExpression)lambda.Body;
            }
            if (memberExpression != null)
            {
                name = memberExpression.Member.Name;
            }
            else
            {
                throw new Exception("Invalid property expression passed into GetPropertyName");
            }
            return name;
        }


        public static PropertyChangedEventArgs GetPropertyChangedEventArgs<T>(Expression<Func<T,object>> propertyExpression)
        {
            return new PropertyChangedEventArgs(GetPropertyName(propertyExpression));
        }


        protected bool SetValue(ref string field,string value, PropertyChangedEventArgs property)
        {
            bool set = false;
            if(field!=value)
            {
                field = value;
                NotifyPropertyChanged(property);
                set = true;
            }
            return set;
        }

        public virtual void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
          

        // The CallerMemberName attribute that is applied to the optional propertyName 
        // parameter causes the property name of the caller to be substituted as an argument. 
        public virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            //Debug.WriteLine("NotifyPropertyChanged({0})",(object)propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void NotifyPropertyChanged(Expression<Func<object>> property)
        {
            var lambda = (LambdaExpression)property;
            MemberExpression memberExpression = null;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            } 
            else if (lambda.Body is MemberExpression)
            {
                memberExpression = (MemberExpression)lambda.Body;
            }
            if (memberExpression != null)
            {
                this.NotifyPropertyChanged(memberExpression.Member.Name);
            }
            else
            {
                throw new Exception("Invalid property expression passed into NotifyPropertyChanged");
            }
        }

        public virtual void NotifyAllPropertiesChanged()
        {
            //Debug.WriteLine("NotifyAllPropertiesChanged");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
        }


        public void Dispose()
        {
            PropertyChanged = null;
        }
    }
}
