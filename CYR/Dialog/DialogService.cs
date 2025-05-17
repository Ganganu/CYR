using System;
using System.Collections.Generic;
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
    }
}