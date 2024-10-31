using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Serialization;

namespace WYF.Form.Mvc.Form
{
    public class ClientViewProxy : IClientViewProxy
    {
        private List<CallParameter> _methodCalls;

        private IPageCache _pageCache;
        private List<CallParameter> _premethodCalls;
        private Dictionary<String, Object> controlViewStates;
        private Dictionary<String, Object> _lockStates;
        private List<CallParameter> lockStates1;

        private List<CallParameter> visibleStates1;
        private JSONArray _actions = new JSONArray();
        private JSONObject _visibleStates;
        private JSONArray _controlsState;
        private Dictionary<String, JSONObject> _dctControlsStates;
        private Dictionary<String, JSONObject> _dctactions;

        private JSONObject _focusControl;
        private Dictionary<String, Dictionary<Object, JSONArray>> _entryDataRows;

        private List<JSONObject> _ruleActions;
        public bool NewRowAutoFocus { get; set; }
        public ClientViewProxy(IPageCache pageCache)
        {
            this._pageCache = pageCache;
            this.controlViewStates = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            this._lockStates = new Dictionary<string, object>();
            this._visibleStates = new JSONObject();

            this.NewRowAutoFocus = true;
            ResetActions();
            LoadViewStates();
        }

        private void LoadViewStates()
        {
            string states = this._pageCache.Get("controlstates");
            if (string.IsNullOrEmpty(states))
            {
                this.controlViewStates = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                this.controlViewStates = SerializationUtils.FromJsonString<Dictionary<String, Object>>(states);
            }
        }

        public void ResetActions()
        {
            this._controlsState = new JSONArray();
            this._methodCalls = new List<CallParameter>();
            this.lockStates1 = new List<CallParameter>();
            this.visibleStates1 = new List<CallParameter>();
            this._premethodCalls = new List<CallParameter>();
            this._dctControlsStates = new Dictionary<string, JSONObject>(StringComparer.OrdinalIgnoreCase);
            this._dctactions = new Dictionary<string, JSONObject>();
            this._focusControl = null;
            this._entryDataRows = null;
            this._ruleActions = new List<JSONObject>();
        }

        public List<object> GetActionResult()
        {
            QueueExistsActions();

            SaveViewStates();
            List<Object> acts = new List<object>();
            acts.AddRange(this._actions);
            this._actions.Clear();
            return acts;
        }

        private void SaveViewStates()
        {
            String states = this._pageCache.Get("controlstates");
            if (string.IsNullOrEmpty(states) && this.controlViewStates.Count == 0) return;
            this._pageCache.Add("controlstates", SerializationUtils.ToJsonString(this.controlViewStates));
        }

        private void QueueExistsActions()
        {
            if (_premethodCalls.Count > 0)
            {
                AddAction("InvokeControlMethod", this._premethodCalls);
            }
            if (_controlsState.Count > 0)
            {
                AddAction("u", this._controlsState);
            }
            if (_methodCalls.Count > 0)
            {
                AddAction("InvokeControlMethod", this._methodCalls, false);
            }
            if (lockStates1.Count > 0)
            {
                AddAction("InvokeControlMethod", this.lockStates1, false);
            }
            if (visibleStates1.Count > 0)
            {
                AddAction("InvokeControlMethod", this.visibleStates1, false);
            }
            if (_ruleActions.Count > 0)
            {
                AddAction("callClientAction", this._ruleActions);
            }
            if (_focusControl != null && _focusControl.Count > 0)
            {
                AddAction("setFocus", this._focusControl);
            }
            ResetActions();
        }

        public void AddAction(String actionName, Object param)
        {
            JSONObject action = null;
            JSONArray paras;
            if (this._dctactions.ContainsKey(actionName))
            {
                action = this._dctactions.GetValueOrDefault(actionName);
            }
            if (action == null)
            {
                action = new JSONObject();
                this._dctactions[actionName] = action;
                action["a"] = actionName;
                this._actions.Add(action);
            }
            if (!action.ContainsKey("p"))
            {
                paras = new JSONArray();
                action.Put("p", paras);
            }
            else
            {
                paras = (JSONArray)action.GetValueOrDefault("p");
            }

            if (param is JSONArray)
            {
                JSONArray p = (JSONArray)param;
                foreach (Object o in p)
                {
                    paras.Add(o);
                }
            }
            else
            {
                paras.Add(param);
            }
        }


        public void AddAction(String actionName, Object param, bool isMerge)
        {
            List<Object> paras;
            if (isMerge)
            {
                AddAction(actionName, param);
                return;
            }
            Dictionary<String, Object> action = null;
            if (action == null)
            {
                action = new Dictionary<String, Object>();
                action["a"] = actionName;
                this._actions.Add(action);
            }

            if (!action.ContainsKey("p"))
            {
                paras = new List<Object>();
                action["p"] = paras;
            }
            else
            {
                paras = (List<Object>)action.GetValueOrDefault("p");
            }

            if (param is IList)
            {
                List<Object> p = (List<Object>)param;
                foreach (Object o in p)
                {
                    paras.Add(o);
                }
            }
            else
            {
                paras.Add(param);
            }
        }

        public void SetFieldProperty(string key, string property, object v)
        {
            //GetControlState(key).Add(property, v);
            GetControlState(key)[property] = v;
        }


        private JSONObject GetControlState(string key)
        {
            JSONObject controlState = null;
            if ((controlState = this._dctControlsStates.GetValueOrDefault(key, null)) == null)
            {
                controlState = new JSONObject();
                //controlState["k"] = key;

                controlState.Put("k", key);
                this._dctControlsStates[key] = controlState;

                this._controlsState.Add(controlState);
            }
            return controlState;
        }
    }
}
