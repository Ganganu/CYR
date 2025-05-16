using System.Linq.Expressions;
using System.Windows;

namespace CYR.Dialog
{
    public interface IDialogService
    {
        public void ShowDialog<TViewModel>(Action<string> callback, Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters = null);
    }
    public class DialogService : IDialogService
    {
        static Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        public static void RegisterDialog<TView,TViewModel>()
        {
            _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public void ShowDialog<TViewModel>(Action<string> callback, Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters = null)
        {
            var type = _mappings[typeof(TViewModel)];
            ShowDialogInternal(type, callback, typeof(TViewModel), viewModelParameters);
        }

        private static void ShowDialogInternal<TViewModel>(Type type, Action<string> callback, Type? vmType, Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters)
        {
            var dialog = new DialogWindow();
            EventHandler closeEventHandler = null;
            closeEventHandler = (sender, e) =>
            {
                callback(dialog.DialogResult.ToString());
                dialog.Closed -= closeEventHandler;
            };
            dialog.Closed += closeEventHandler;

            var content = Activator.CreateInstance(type);
            if (vmType != null)
            {
                var vm = Activator.CreateInstance(vmType);

                if (viewModelParameters != null)
                {
                    foreach (var param in viewModelParameters)
                    {
                        if (param.Key.Body is MemberExpression memberExpression)
                        {
                            var propertyName = memberExpression.Member.Name;
                            var property = vmType.GetProperty(propertyName);
                            if (property != null && property.CanWrite)
                            {
                                property.SetValue(vm, param.Value);
                            }
                        }
                        else if (param.Key.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression unaryMemberExpression)
                        {
                            var propertyName = unaryMemberExpression.Member.Name;
                            var property = vmType.GetProperty(propertyName);
                            if (property != null && property.CanWrite)
                            {
                                property.SetValue(vm, param.Value);
                            }
                        }
                    }
                }

                (content as FrameworkElement).DataContext = vm;
            }

            dialog.Content = content;
            dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dialog.ShowDialog();
        }

    }
}
