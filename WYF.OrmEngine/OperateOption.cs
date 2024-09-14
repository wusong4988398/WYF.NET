using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.Bos.DataEntity;

namespace WYF.OrmEngine
{
    [Serializable]
    public abstract class OperateOption
    {

        protected OperateOption()
        {
        }

        public abstract bool ContainsVariable(string name);
        public abstract OperateOption Copy();
        public static OperateOption Create()
        {
            return new OperateOptionPrivate();
        }

        public abstract Dictionary<string, object> GetVariables();
        public T GetVariableValue<T>(string name)
        {
            T local;
            if (!this.TryGetVariableValue<T>(name, out local))
            {
                throw new ORMDesignException("??????", string.Format("从额外选项OperateOption中获取变量失败，变量[{0}]不存在！", name));
            }
            return local;
        }

        public T GetVariableValue<T>(string name, T defaultValue)
        {
            T local;
            if (!this.TryGetVariableValue<T>(name, out local))
            {
                local = defaultValue;
            }
            return local;
        }

        public OperateOption Merge(OperateOption baseOption)
        {
            if ((baseOption != null) && !object.ReferenceEquals(baseOption, this))
            {
                return new MergedOperateOption(this, baseOption);
            }
            return this;
        }

        public abstract OperateOption MergeValue(OperateOption other);
        public abstract bool RemoveVariable(string name);
        public abstract void SetVariableValue(string name, object value);
        public abstract bool TryGetVariableValue<T>(string name, out T result);

        // Nested Types
        [Serializable]
        private sealed class MergedOperateOption : OperateOption
        {
            // Fields
            private readonly OperateOption _baseOption;
            private readonly OperateOption _currentOption;

            // Methods
            public MergedOperateOption(OperateOption currentOption, OperateOption baseOption)
            {
                this._currentOption = currentOption;
                this._baseOption = baseOption;
            }

            public override bool ContainsVariable(string name)
            {
                if (!this._currentOption.ContainsVariable(name))
                {
                    return this._baseOption.ContainsVariable(name);
                }
                return true;
            }

            public override OperateOption Copy()
            {
                throw new Exception("当前对象不支持Copy操作");
            }

            public override Dictionary<string, object> GetVariables()
            {
                Dictionary<string, object> variables = this._currentOption.GetVariables();
                Dictionary<string, object> dictionary2 = this._baseOption.GetVariables();
                Dictionary<string, object> dictionary3 = new Dictionary<string, object>(variables);
                foreach (KeyValuePair<string, object> pair in dictionary2)
                {
                    if (!dictionary3.ContainsKey(pair.Key))
                    {
                        dictionary3.Add(pair.Key, pair.Value);
                    }
                }
                return dictionary3;
            }

            public override OperateOption MergeValue(OperateOption other)
            {
                foreach (KeyValuePair<string, object> pair in other.GetVariables())
                {
                    if (!this.ContainsVariable(pair.Key))
                    {
                        this._currentOption.SetVariableValue(pair.Key, pair.Value);
                    }
                }
                return this;
            }

            public override bool RemoveVariable(string name)
            {
                return this._currentOption.RemoveVariable(name);
            }

            public override void SetVariableValue(string name, object value)
            {
                this._currentOption.SetVariableValue(name, value);
            }

            public override bool TryGetVariableValue<T>(string name, out T result)
            {
                if (!this._currentOption.TryGetVariableValue<T>(name, out result))
                {
                    return this._baseOption.TryGetVariableValue<T>(name, out result);
                }
                return true;
            }
        }

        [Serializable]
        private sealed class OperateOptionPrivate : OperateOption
        {
            // Fields
            private Dictionary<string, object> dict;

            // Methods
            public override bool ContainsVariable(string name)
            {
                return this._dict.ContainsKey(name);
            }

            public override OperateOption Copy()
            {
                OperateOption.OperateOptionPrivate @private = new OperateOption.OperateOptionPrivate();
                foreach (KeyValuePair<string, object> pair in this._dict)
                {
                    @private._dict.Add(pair.Key, pair.Value);
                }
                return @private;
            }

            public override Dictionary<string, object> GetVariables()
            {
                return new Dictionary<string, object>(this._dict);
            }

            public override OperateOption MergeValue(OperateOption other)
            {
                foreach (KeyValuePair<string, object> pair in other.GetVariables())
                {
                    if (!this._dict.ContainsKey(pair.Key))
                    {
                        this._dict.Add(pair.Key, pair.Value);
                    }
                }
                return this;
            }

            public override bool RemoveVariable(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return false;
                }
                return this._dict.Remove(name);
            }

            public override void SetVariableValue(string name, object value)
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ORMDesignException("??????", "设置额外选项OperateOption的变量失败，变量名不能为空！");
                }
                this._dict[name] = value;
            }

            public override bool TryGetVariableValue<T>(string name, out T result)
            {
                object obj2;
                if (string.IsNullOrEmpty(name))
                {
                    result = default(T);
                    return false;
                }
                if (this._dict.TryGetValue(name, out obj2))
                {
                    result = (T)obj2;
                    return true;
                }
                result = default(T);
                return false;
            }

            private Dictionary<string, object> _dict
            {
                get
                {
                    if (this.dict == null)
                    {
                        this.dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    }
                    return this.dict;
                }
            }
        }
    }
}
