using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Excimer.KnockoutMapping
{
    public class ChangeEntry
    {
        public string PropertyName { get; set; }
        public object NewValue { get; set; }
    }

    public interface IKnockoutMapper
    {
        void RegisterServerSideViewModel<T>();
        KnockoutMapper.ViewModelWrapper ConstructViewModel(Type type);
        JavascriptString RenderKoMapping(Type type);
        Dictionary<string, object> GetInstance(string token);
        List<ChangeEntry> Subscribe(string viewModelToken);
        object GetViewModelObject(string refToken);
    }

    public class KnockoutMapper : IKnockoutMapper
    {
        public class ViewModelWrapper
        {
            public string RefToken { get; set; }
            public object ViewModel { get; set; }
        }

        private List<Type> _registeredTypes = new List<Type>();
        private IOperationRegistry _operationRegistry;
        private IMonitorCollection _monitorCollection;

        private Dictionary<string, ViewModelWrapper> _viewModelInstances = new Dictionary<string, ViewModelWrapper>();

        public List<ChangeEntry> PendingChanges = new List<ChangeEntry>();

        public KnockoutMapper(IOperationRegistry operationRegistry, IMonitorCollection monitorCollection)
        {
            _operationRegistry = operationRegistry;
            _monitorCollection = monitorCollection;

            _operationRegistry.RegisterCommand("subscribeToViewModel", new Func<string, List<ChangeEntry>>(Subscribe));
        }

        public void RegisterServerSideViewModel<T>()
        {
            var type = typeof(T);
            if (_registeredTypes.Contains(type))
                throw new Exception("Type '" + type.Name + "' is already registered");

            _registeredTypes.Add(type);

            _operationRegistry.RegisterCommand(type.Name + "_front", new Func<JavascriptString>(() => RenderKoMapping(type)));
            _operationRegistry.RegisterCommand(type.Name + "_constructor", new Func<ViewModelWrapper>(() => ConstructViewModel(type)));
        }

        private int _tokenCounter;

        public ViewModelWrapper ConstructViewModel(Type type)
        {
            var instance = Activator.CreateInstance(type);
            var token = "token_" + _tokenCounter++;

            var viewModelWrapper = new ViewModelWrapper { ViewModel = instance, RefToken = token };
            _viewModelInstances[token] = viewModelWrapper;

            foreach (var field in type.ObservableFields())
            {
                var localField = field; // Make sure closure doesn't change variable.
                var fieldValue = (IObservable)localField.GetValue(instance);
                fieldValue.Subscribe(newValue => AddPendingChange(localField.Name, newValue, token));
            }
            
            return viewModelWrapper;
        }

        private void AddPendingChange(string propertyName, object newValue, string viewModelToken)
        {
            PendingChanges.Add(new ChangeEntry { PropertyName = propertyName, NewValue = newValue });
            _monitorCollection.Pulse(viewModelToken);
        }

        public JavascriptString RenderKoMapping(Type type)
        {
            if(!_registeredTypes.Contains(type))
                throw new Exception("Type " + type.Name + " was not registered as a view model.");

            var initializers = new StringBuilder();
            var updaters = new StringBuilder();
            
            foreach (var field in type.ObservableFields())
            {
                initializers.Append("self." + field.Name.ToLowerCaseFirstLetter() + " = ko.observable('');");
                updaters.Append("self." + field.Name.ToLowerCaseFirstLetter() + "(result.viewModel." + field.Name.ToLowerCaseFirstLetter() + ");");
            }

            var b = new StringBuilder();
            b.AppendLine("function " + type.Name + "Mapper() { ");
            b.AppendLine("var self = this;");
            b.AppendLine(initializers.ToString());

            b.AppendLine("function subscribe() {");
            b.AppendLine("$.get('/api/subscribeToViewModel', { viewModelToken: self.refToken }, function(result) {");
            b.AppendLine("for(var i = 0; i < result.length; i++) { self[result[i].propertyName](result[i].newValue); };");
            b.AppendLine("subscribe();");
            b.AppendLine("});");
            b.AppendLine("}");

            b.AppendLine("$.get('/api/" + type.Name + "_constructor', {}, function(result) {");
            b.AppendLine("self.refToken = result.refToken;");
            b.AppendLine(updaters.ToString());
            b.AppendLine("subscribe();");
            b.AppendLine("});");

            b.AppendLine(" };");

            b.AppendLine("$(function() {");

            b.AppendLine("var viewModel = new " + type.Name + "Mapper();");
            b.AppendLine("ko.applyBindings(viewModel);");

            b.AppendLine("});");

            return new JavascriptString(b.ToString());
        }

        public Dictionary<string, object> GetInstance(string token)
        {
            if (!_viewModelInstances.ContainsKey(token))
                return null;

            var instance = _viewModelInstances[token].ViewModel;
            var result = new Dictionary<string, object>();

            foreach (var field in instance.GetType().ObservableFields())
            {
                var fieldValue = (IObservable)field.GetValue(instance);
                result[field.Name.ToLowerCaseFirstLetter()] = fieldValue.GetJson();
            }

            return result;
        }

        public List<ChangeEntry> Subscribe(string viewModelToken)
        {
            _monitorCollection.Wait(viewModelToken, 50 * 1000);
            Env.Log("Long Poll responding");

            var result = new List<ChangeEntry>();
            result.AddRange(PendingChanges);
            PendingChanges.Clear();
            return result;
        }

        public object GetViewModelObject(string refToken)
        {
            return _viewModelInstances[refToken].ViewModel;
        }
    }
}
