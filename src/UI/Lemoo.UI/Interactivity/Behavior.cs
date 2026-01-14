using System.Windows;

namespace Lemoo.UI.Interactivity
{
    /// <summary>
    /// 为扩展行为提供基类。
    /// </summary>
    /// <typeparam name="T">关联的对象类型</typeparam>
    public abstract class Behavior<T> : IBehavior where T : DependencyObject
    {
        private T? _associatedObject;

        /// <summary>
        /// 获取关联的对象。
        /// </summary>
        public T? AssociatedObject
        {
            get => _associatedObject;
            private set => _associatedObject = value;
        }

        /// <summary>
        /// 获取关联的对象。
        /// </summary>
        DependencyObject? IBehavior.AssociatedObject => AssociatedObject;

        /// <summary>
        /// 将行为附加到关联对象。
        /// </summary>
        /// <param name="dependencyObject">要关联的对象</param>
        public void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
                throw new System.ArgumentNullException(nameof(dependencyObject));

            if (dependencyObject != AssociatedObject)
            {
                if (AssociatedObject != null)
                    throw new System.InvalidOperationException("Cannot attach behavior to multiple objects.");

                if (dependencyObject is not T)
                    throw new System.ArgumentException($"Object must be of type {typeof(T)}", nameof(dependencyObject));

                AssociatedObject = (T)dependencyObject;
                OnAttached();
            }
        }

        /// <summary>
        /// 从关联对象分离行为。
        /// </summary>
        public void Detach()
        {
            OnDetaching();
            AssociatedObject = null;
        }

        /// <summary>
        /// 在行为附加到关联对象后调用。
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// 在行为从关联对象分离前调用。
        /// </summary>
        protected virtual void OnDetaching()
        {
        }
    }

    /// <summary>
    /// 行为接口。
    /// </summary>
    public interface IBehavior
    {
        /// <summary>
        /// 获取关联的对象。
        /// </summary>
        DependencyObject? AssociatedObject { get; }

        /// <summary>
        /// 将行为附加到关联对象。
        /// </summary>
        void Attach(DependencyObject dependencyObject);

        /// <summary>
        /// 从关联对象分离行为。
        /// </summary>
        void Detach();
    }
}
