using System.Linq.Expressions;
using System.Windows;

namespace CYR.Dialog;

public interface IDialogService
{
    public void ShowDialog<TViewModel>(Action<string> callback, Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters = null);
    public void ShowDialog<TViewModel>(Action<TViewModel> callback, Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters = null);
}

public class DialogService : IDialogService
{
    static Dictionary<Type, (Type ViewType, Func<object[]>? ConstructorParamsFactory)> _mappings =
        new Dictionary<Type, (Type, Func<object[]>?)>();

    public static void RegisterDialog<TView, TViewModel>()
        where TView : new()
        where TViewModel : new()
    {
        _mappings[typeof(TViewModel)] = (typeof(TView), null);
    }

    public static void RegisterDialog<TView, TViewModel>(Func<object[]> constructorParamsFactory)
        where TView : new()
    {
        _mappings[typeof(TViewModel)] = (typeof(TView), constructorParamsFactory);
    }

    public void ShowDialog<TViewModel>(Action<string> callback, Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters = null)
    {
        if (!_mappings.TryGetValue(typeof(TViewModel), out var mapping))
        {
            throw new InvalidOperationException($"No view registered for view model type {typeof(TViewModel).Name}");
        }

        ShowDialogInternal(mapping.ViewType, callback, typeof(TViewModel), mapping.ConstructorParamsFactory, viewModelParameters);
    }    

    private static void ShowDialogInternal<TViewModel>(
        Type viewType,
        Action<string> callback,
        Type? vmType,
        Func<object[]>? constructorParamsFactory,
        Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters)
    {
        var dialog = new DialogWindow();
        EventHandler closeEventHandler = null;
        closeEventHandler = (sender, e) =>
        {
            callback(dialog.DialogResult.ToString());
            dialog.Closed -= closeEventHandler;
        };
        dialog.Closed += closeEventHandler;

        var content = Activator.CreateInstance(viewType);

        if (vmType != null)
        {
            object vm;

            if (constructorParamsFactory != null)
            {
                var constructorParams = constructorParamsFactory();
                vm = Activator.CreateInstance(vmType, constructorParams);
            }
            else
            {
                vm = Activator.CreateInstance(vmType);
            }

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
    public void ShowDialog<TViewModel>(Action<TViewModel> callback, Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters = null)
    {
        if (!_mappings.TryGetValue(typeof(TViewModel), out var mapping))
        {
            throw new InvalidOperationException($"No view registered for view model type {typeof(TViewModel).Name}");
        }

        ShowDialogInternal(mapping.ViewType, callback, typeof(TViewModel), mapping.ConstructorParamsFactory, viewModelParameters);
    }
    private static void ShowDialogInternal<TViewModel>(
    Type viewType,
    Action<TViewModel> callback,
    Type? vmType,
    Func<object[]>? constructorParamsFactory,
    Dictionary<Expression<Func<TViewModel, object>>, object>? viewModelParameters)
    {
        var dialog = new DialogWindow();
        object? vm = null;

        if (vmType != null)
        {
            vm = constructorParamsFactory != null
                ? Activator.CreateInstance(vmType, constructorParamsFactory())
                : Activator.CreateInstance(vmType);

            if (viewModelParameters != null && vm != null)
            {
                foreach (var param in viewModelParameters)
                {
                    var memberExpr = param.Key.Body as MemberExpression ??
                                    (param.Key.Body as UnaryExpression)?.Operand as MemberExpression;

                    if (memberExpr != null)
                    {
                        var property = vmType.GetProperty(memberExpr.Member.Name);
                        if (property != null && property.CanWrite)
                        {
                            property.SetValue(vm, param.Value);
                        }
                    }
                }
            }
        }
        EventHandler? closeEventHandler = null;
        closeEventHandler = (sender, e) =>
        {
            dialog.Closed -= closeEventHandler;
            if (vm is TViewModel typedVm)
            {
                callback(typedVm);
            }
        };
        dialog.Closed += closeEventHandler;

        var content = Activator.CreateInstance(viewType);
        if (content is FrameworkElement element)
        {
            element.DataContext = vm;
        }

        dialog.Content = content;
        dialog.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        dialog.ShowDialog();
    }
}