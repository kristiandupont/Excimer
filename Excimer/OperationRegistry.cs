using System;
using System.Collections.Generic;
using System.Linq;

namespace Excimer
{
    public interface IOperationRegistry
    {
        void RegisterCommand(string name, Delegate commandFunction);
        object InvokeCommand(string commandName, System.Collections.Generic.Dictionary<string, object> parameters);
        object InvokeCommand(string commandName, System.Collections.Generic.Dictionary<string, string> parameters);
    }

    public class OperationRegistry : IOperationRegistry
    {
        private readonly Dictionary<string, Delegate> _registeredCommands = new Dictionary<string, Delegate>();

        public void RegisterCommand(string name, Delegate commandFunction)
        {
            _registeredCommands[name] = commandFunction;
        }

        /// <summary>
        /// Invoke a command with a dictionary of typed parameters.
        /// </summary>
        public object InvokeCommand(string commandName, Dictionary<string, object> parameters)
        {
            if (!_registeredCommands.ContainsKey(commandName))
                throw new ArgumentException(string.Format("Unknown API command '{0}'", commandName));

            var command = _registeredCommands[commandName];
            var cmdParams = command.Method.GetParameters();
            var args = new List<object>();

            foreach (var p in cmdParams)
            {
                if (!parameters.ContainsKey(p.Name))
                    throw new ArgumentException(string.Format("Missing parameter '{0}'", p.Name), p.Name);

                args.Add(parameters[p.Name]);
            }

            var returnVal = command.DynamicInvoke(args.ToArray());
            return returnVal;
        }

        public object InvokeCommand(string commandName, Dictionary<string, string> parameters)
        {
            if (!_registeredCommands.ContainsKey(commandName))
                throw new ArgumentException("Unknown API command '" + commandName + "'");

            var typedParameters = new Dictionary<string, object>();

            var command = _registeredCommands[commandName];
            var cmdParams = command.Method.GetParameters();

            // If the command is registered with only one parameter, dictionary<string, object>,
            // that parameter will contain all the parameter values.
            if (cmdParams.Count() == 1 && cmdParams.First().ParameterType == typeof(Dictionary<string, string>))
            {
                return command.DynamicInvoke(parameters);
            }
            
            foreach (var p in cmdParams)
            {
                if (!parameters.ContainsKey(p.Name))
                    throw new ArgumentException("Missing parameter '" + p.Name + "'", p.Name);

                var v = parameters[p.Name];

                try
                {
                    typedParameters[p.Name] = QueryParse.Parse(v, p.ParameterType);
                }
                catch (ArgumentException)
                {
                    throw new ArgumentException("Parameter '" + p.Name + "' has unsupported type '" + p.ParameterType.Name + "'", p.Name);
                }
            }

            return InvokeCommand(commandName, typedParameters);
        }
    }
}
