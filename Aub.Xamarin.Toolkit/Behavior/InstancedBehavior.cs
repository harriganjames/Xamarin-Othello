using System;
using Xamarin.Forms;

namespace Aub.Xamarin.Toolkit.Behavior
{
    public abstract class InstancedBehavior<T> : Behavior<T> where T : BindableObject
    {
        protected T AssociatedObject { get; private set; }

        protected override void OnAttachedTo(T bindable)
        {
            base.OnAttachedTo(bindable);
            this.AssociatedObject = bindable;
            if (bindable == null)
                this.BindingContext = null;
            else
            {
                bindable.BindingContextChanged += associatedObject_BindingContextChanged;
                this.BindingContext = bindable.BindingContext;
            }
        }

        protected override void OnDetachingFrom(T bindable)
        {
            base.OnDetachingFrom(bindable);
            if (bindable != null)
                bindable.BindingContextChanged -= this.associatedObject_BindingContextChanged;
            this.AssociatedObject = null;
        }

        private void associatedObject_BindingContextChanged(object sender, EventArgs e)
        {
            if (AssociatedObject != null)
                this.BindingContext = AssociatedObject.BindingContext;
        }
    }
}
