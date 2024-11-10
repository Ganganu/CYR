using System.Windows;

namespace CYR.Dialog
{
    public interface IDialogService
    {
        void ShowDialog(string message, Action<string> callback);
        void ShowDialog<TViewModel>(Action<string> callback, Dictionary<string, object>? viewModelParameters = null);
    }
    public class DialogService : IDialogService
    {
        static Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();
        public static void RegisterDialog<TView,TViewModel>()
        {
            _mappings.Add(typeof(TViewModel), typeof(TView));
        }

        public void ShowDialog<TViewModel>(Action<string> callback, Dictionary<string, object>? viewModelParameters = null)
        {
            var type = _mappings[typeof(TViewModel)];
            ShowDialogInternal(type, callback, typeof(TViewModel), viewModelParameters);
        }

        public void ShowDialog(string message, Action<string> callback)
        {
            var type = Type.GetType($"CYR.Dialog.{message}");
            ShowDialogInternal(type, callback, null);
        }

        private static void ShowDialogInternal(Type type, Action<string> callback, Type vmType, 
            Dictionary<string, object>? viewModelParameters = null)
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
            if(vmType != null)
            {
                var vm = Activator.CreateInstance(vmType);

                if (viewModelParameters != null)
                {
                    foreach (var param in viewModelParameters)
                    {
                        var property = vmType.GetProperty(param.Key);
                        if (property != null && property.CanWrite)
                        {
                            property.SetValue(vm, param.Value);
                        }
                    }
                }

                (content as FrameworkElement).DataContext = vm;
            }

            dialog.Content = content;
            dialog.ShowDialog();
        }
    }
}
