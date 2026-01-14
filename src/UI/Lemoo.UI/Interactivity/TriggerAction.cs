using System;
using System.Windows;

namespace Lemoo.UI.Interactivity
{
    /// <summary>
    /// 触发器操作基类。
    /// </summary>
    /// <typeparam name="T">关联的对象类型</typeparam>
    public abstract class TriggerAction<T> : TriggerAction where T : DependencyObject
    {
        private T? _associatedObject;

        /// <summary>
        /// 获取关联的对象。
        /// </summary>
        protected T? TypedAssociatedObject
        {
            get => _associatedObject;
            private set => _associatedObject = value;
        }

        /// <summary>
        /// 获取关联的对象。
        /// </summary>
        public override DependencyObject? AssociatedObject => _associatedObject;

        /// <summary>
        /// 将操作附加到关联对象。
        /// </summary>
        /// <param name="dependencyObject">要关联的对象</param>
        public override void Attach(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
                throw new ArgumentNullException(nameof(dependencyObject));

            if (dependencyObject != AssociatedObject)
            {
                if (AssociatedObject != null)
                    throw new InvalidOperationException("Cannot attach action to multiple objects.");

                if (dependencyObject is not T)
                    throw new ArgumentException($"Object must be of type {typeof(T)}", nameof(dependencyObject));

                _associatedObject = (T)dependencyObject;
                OnAttached();
            }
        }

        /// <summary>
        /// 从关联对象分离操作。
        /// </summary>
        public override void Detach()
        {
            OnDetaching();
            _associatedObject = null;
        }

        /// <summary>
        /// 在操作附加到关联对象后调用。
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        /// <summary>
        /// 在操作从关联对象分离前调用。
        /// </summary>
        protected virtual void OnDetaching()
        {
        }

        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="parameter">参数</param>
        protected abstract void Invoke(object parameter);

        /// <summary>
        /// 执行操作。
        /// </summary>
        /// <param name="parameter">参数</param>
        internal override void Execute(object parameter)
        {
            Invoke(parameter);
        }
    }

    /// <summary>
    /// 触发器操作基类。
    /// </summary>
    public abstract class TriggerAction : IAttachedObject
    {
        DependencyObject? IAttachedObject.AssociatedObject => AssociatedObject;

        /// <summary>
        /// 获取关联的对象。
        /// </summary>
        public abstract DependencyObject? AssociatedObject { get; }

        /// <summary>
        /// 将操作附加到关联对象。
        /// </summary>
        public abstract void Attach(DependencyObject dependencyObject);

        /// <summary>
        /// 从关联对象分离操作。
        /// </summary>
        public abstract void Detach();

        /// <summary>
        /// 执行操作。
        /// </summary>
        internal abstract void Execute(object parameter);

        void IAttachedObject.Attach(DependencyObject dependencyObject)
        {
            Attach(dependencyObject);
        }

        void IAttachedObject.Detach()
        {
            Detach();
        }
    }
}
