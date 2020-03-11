using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Daniels.Common
{
    public class ActionInfo
    {
        public readonly uint Id;
        public readonly string Name;
        public readonly string Help;
        public readonly string Parameters;
        public readonly bool ReadOnly;
        public ActionInfo(uint id, string name, string help, bool readOnly, string parameters)
        {
            Id = id;
            Name = name;
            Help = help;
            ReadOnly = readOnly;
            Parameters = parameters;
        }
    }

    public class ActionsManager: IEnumerable<ActionInfo>
    {
        private static ActionsManager _instance;

        private static uint _id = 0;
        private static Dictionary<uint, ActionInfo> _actionInfos = new Dictionary<uint, ActionInfo>();
        private static Dictionary<uint, ActionFunction> _actionFunctions = new Dictionary<uint, ActionFunction>();

        public delegate void ActionFunction(string actionParameters);

        private ActionsManager()
        {

        }

        #region Static Methods

        public static ActionsManager GetInstance()
        {
            if (_instance == null)
                _instance = new ActionsManager();
            return _instance;
        }

        public static uint RegisterAction(ActionFunction function, string name, string help)
        {
            if(function == null)
                throw new ArgumentNullException("function");
            if (_actionFunctions.ContainsValue(function))
                throw new ArgumentOutOfRangeException("function", "Action already registered");
            if (name == null)
                throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty(name))
                throw new ArgumentOutOfRangeException("name");
            if (_actionInfos.Values.FirstOrDefault(a => a.Name == name) != null)
                throw new ArgumentOutOfRangeException("name", "Action with name " + name + " already registered");

            uint actionId = ++_id;
            string actionHelp = (help == null) ? String.Empty : help;

            _actionFunctions.Add(actionId, function);
            _actionInfos.Add(actionId, new ActionInfo(actionId, name, actionHelp, true, String.Empty));

            return actionId;
        }

        public static uint AddCutomAction(uint id, string actionName, string parameters, string help)
        {
            if (!_actionFunctions.ContainsKey(id))
                throw new ArgumentOutOfRangeException("id");
            if (actionName == null)
                throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty(actionName))
                throw new ArgumentOutOfRangeException("name");
            if (_actionInfos.Values.FirstOrDefault(a => a.Name == actionName) != null)
                throw new ArgumentOutOfRangeException("name", "Action with name " + actionName + " already registered");
            if (String.IsNullOrEmpty(parameters))
                throw new ArgumentNullException("parameters");

            string actionHelp = (help == null) ? String.Empty : help;
            
            uint actionId = ++_id;

            _actionInfos.Add(id, new ActionInfo(actionId, actionName, actionHelp, false, parameters));

            return actionId;
        }

        public static void InvokeAction(uint id)
        {
            var kv = _actionInfos.FirstOrDefault(x => x.Value.Id == id);
            if (kv.IsNull())
                throw new ArgumentOutOfRangeException("id");
            _actionFunctions[kv.Key].Invoke(kv.Value.Parameters);
        }

        #endregion Static Methods

        #region Properties

        public ActionInfo this[uint id]
        {
            get
            {
                if (_actionInfos.ContainsKey(id))
                    return _actionInfos[id];
                else
                    throw new IndexOutOfRangeException();
            }
        }

        public uint Length
        {
            get { return (uint)_actionInfos.Count; }
        }

        #endregion Properties

        #region IEnumerable<ActionInfo> Implementation

        public IEnumerator<ActionInfo> GetEnumerator()
        {
            return _actionInfos.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion IEnumerable<ActionInfo> Implementation



        /*
        private static ReadOnlyDictionary<string, ActionFunction> _actionsReadOnly = new ReadOnlyDictionary<string, ActionFunction>(_actions);
        public static ReadOnlyDictionary<string, ActionFunction> Actions
        {
            get { return _actionsReadOnly; }
        }
        */

    }
}