using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Daniels.Common
{
    public static class ActionsManager
    {
        private class ActionStub
        {
            public ActionFunction Function;
            public string Help;
            public Dictionary<string, string> Parameters;
            public ActionStub(ActionFunction function, string help, Dictionary<string, string> parameters)
            {
                Function = function;
                Help = help;
                Parameters = parameters;
            }
        }
        private static Dictionary<string, ActionStub> _actions = new Dictionary<string, ActionStub>();

        public delegate void ActionFunction(string actionParameters);

        public static bool RegisterAction(ActionFunction actionFunction, string actionName, string help)
        {
            return RegisterAction(actionFunction, actionName, help, new Dictionary<string, string>());
        }

        public static bool RegisterAction(ActionFunction actionFunction, string actionName, string help, string paramName, string paramValue)
        {
            if (String.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");
            if (String.IsNullOrEmpty(paramValue))
                throw new ArgumentNullException("paramValue");
            return RegisterAction(actionFunction, actionName, help, new Dictionary<string, string>() { { paramName, paramValue } });
        }

        public static bool RegisterAction(ActionFunction actionFunction, string actionName, string help, Dictionary<string, string> parameters)
        {
            if(_actions.ContainsKey(actionName))
                return false;
            if(actionFunction == null)
                return false;

            Dictionary<string, string> actionParameters;

            if (parameters == null)
                actionParameters = new Dictionary<string, string>();
            else
                actionParameters = parameters;
            _actions.Add(actionName, new ActionStub(actionFunction, help, parameters));
            //_actionsReadOnly = new ReadOnlyDictionary<string, ActionFunction>(_actions);
            return true;
        }

        public static string[] GetActions()
        {
            return _actions.Keys.ToArray();
        }

        public static string[] GetActionsParameters(string actionName)
        {
            if (!_actions.ContainsKey(actionName))
                throw new ArgumentOutOfRangeException("actionName", "Action not found");

            return _actions[actionName].Parameters.Keys.ToArray();
        }

        public static void InvokeAction(string actionName)
        {
            InvokeAction(actionName, null);
        }

        public static void InvokeAction(string actionName, string actionParameters)
        {
            if (!_actions.ContainsKey(actionName))
                throw new ArgumentOutOfRangeException("actionName", "Action not found");

            if (String.IsNullOrEmpty(actionParameters))
            {
                _actions[actionName].Function(String.Empty);
            }
            else
            {
                if (_actions[actionName].Parameters.ContainsKey(actionParameters))
                    _actions[actionName].Function(_actions[actionName].Parameters[actionParameters]);
                else
                    throw new ArgumentOutOfRangeException("actionParameters", "No specified parameters found for given action");
            }
        }

        /*
        private static ReadOnlyDictionary<string, ActionFunction> _actionsReadOnly = new ReadOnlyDictionary<string, ActionFunction>(_actions);
        public static ReadOnlyDictionary<string, ActionFunction> Actions
        {
            get { return _actionsReadOnly; }
        }
        */

    }
}