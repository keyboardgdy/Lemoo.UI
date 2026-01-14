using System.Windows;

namespace Lemoo.UI.Interactivity
{
    /// <summary>
    /// 可附加对象接口。
    /// </summary>
    public interface IAttachedObject
    {
        /// <summary>
        /// 获取关联的对象。
        /// </summary>
        DependencyObject? AssociatedObject { get; }

        /// <summary>
        /// 附加到对象。
        /// </summary>
        void Attach(DependencyObject dependencyObject);

        /// <summary>
        /// 从对象分离。
        /// </summary>
        void Detach();
    }
}
