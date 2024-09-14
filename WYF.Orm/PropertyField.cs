using System.Collections.Generic;
using System;

namespace WYF.SqlParser
{
    public class PropertyField
    {
        public const string AsNullField = "__NULL_FIELD";
        private string _fullObjectName;
        private string _name;
        private string _field;
        private string _alias;
        private PropertySegExpress _propertySegExpress;
        private bool _multiLangProperty;
        private AtomicBoolean _glField;
        private string _entityAlias;
        private IDataEntityType _entityType;
        private IDataEntityProperty _peropertyType;
        private EntityItemProperty _propertyItem;
        private static Dictionary<string, string> _cachedEncryptData = new Dictionary<string, string>(2);
        private static readonly int MaxLength = 22;

        public PropertyField(string peropertyExpress)
        {
            string peropertyExpress2 = peropertyExpress.Trim();
            int blank = peropertyExpress2.IndexOf(' ');
            if (blank == -1)
            {
                _name = peropertyExpress2;
            }
            else
            {
                _name = peropertyExpress2.Substring(0, blank);
                _alias = peropertyExpress2.Substring(blank + 1).Trim();
                if (_alias.ToLowerInvariant().StartsWith("as "))
                {
                    _alias = _alias.Substring(3).Trim();
                }
            }
            int dot = _name.LastIndexOf('.');
            if (dot != -1)
            {
                _fullObjectName = _name.Substring(0, dot).ToLowerInvariant();
                _name = _name.Substring(dot + 1);
            }
            Init();
        }

        public PropertyField(string fullObjectName, string name, string alias)
        {
            _fullObjectName = fullObjectName?.ToLowerInvariant();
            _name = name;
            _alias = alias;
            Init();
        }

        private void Init()
        {
            if (_fullObjectName == null)
            {
                _fullObjectName = string.Empty;
            }
            if (string.IsNullOrEmpty(_alias))
            {
                _alias = _name;
                if (!string.IsNullOrEmpty(_fullObjectName))
                {
                    _alias = $"{_fullObjectName}.{_alias}";
                }
            }
        }

        public string FullObjectName
        {
            get => _fullObjectName;
            set => _fullObjectName = value?.ToLowerInvariant();
        }

        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(_fullObjectName))
                {
                    return $"{_fullObjectName}.{_name}";
                }
                return _name;
            }
        }

        public string ParentFullObjectName
        {
            get
            {
                int dot;
                if (!string.IsNullOrEmpty(_fullObjectName) && (dot = _fullObjectName.IndexOf('.')) != -1)
                {
                    return _fullObjectName.Substring(dot + 1);
                }
                return _fullObjectName;
            }
        }

        public bool CanIgnoreJoinMainTable => _multiLangProperty && !IsGLField;

        public string ParentFullName
        {
            get
            {
                int dot;
                if (!string.IsNullOrEmpty(_fullObjectName) && (dot = _fullObjectName.IndexOf('.')) != -1)
                {
                    return $"{_fullObjectName.Substring(dot + 1)}.{_name}";
                }
                return _name;
            }
        }

        public string Name => _name;

        public string Field
        {
            get => _field;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException($"字段不能为空.", nameof(value));
                }
                _field = value;
            }
        }

        public string Alias => _alias;

        public string EntityAlias
        {
            get => _entityAlias;
            set => _entityAlias = value;
        }

        public IDataEntityProperty PeropertyType
        {
            get => _peropertyType;
            set => _peropertyType = value;
        }

        public IDataEntityType EntityType
        {
            get => _entityType;
            set => _entityType = value;
        }

        public bool IsSameWith(PropertyField sf) => sf.Name.Equals(_name, StringComparison.OrdinalIgnoreCase) && sf.FullObjectName.Equals(_fullObjectName, StringComparison.OrdinalIgnoreCase);

        public bool IsExpress => _propertySegExpress != null;

 

        public string ToSingleTableSelectField(bool alias, Func<string, PropertyField> propertyTrans)
        {
            if (IsExpress)
            {
                if (alias)
                {
                    return _propertySegExpress.ToSingleTableString(alias, propertyTrans) + $" \"{_alias}\"";
                }
                return _propertySegExpress.ToSingleTableString(alias, propertyTrans);
            }
            PropertyField pf = propertyTrans(_name);
            if (alias)
            {
                return $"{pf.Field} \"{_alias}\"";
            }
            return pf.Field;
        }

        public string ToSelectField(bool withAlias, QContext ctx)
        {
            return ToSelectField(withAlias, ctx, false);
        }

        public string ToSelectField(bool withAlias, bool newWhenSame, QContext ctx)
        {
            return ToSelectField(withAlias, ctx, false, newWhenSame);
        }

        public string ToSelectField(bool withAlias, QContext ctx, bool ignoreGL)
        {
            return ToSelectField(withAlias, ctx, ignoreGL, false);
        }

        private string ToSelectField(bool withAlias, QContext ctx, bool ignoreGL, bool newWhenSame)
        {
            string rootObjName;
            if (AsNullField.Equals(this.Field))
            {
                if (withAlias)
                {
                    return "NULL " + GenShortAlias(this.Name, newWhenSame);
                }
                return "NULL";
            }
            var selectField = new System.Text.StringBuilder(64);
            if (IsExpress)
            {
                int dot = this.FullObjectName.IndexOf('.');
                rootObjName = dot == -1 ? this.FullObjectName : this.FullObjectName.Substring(0, dot);
                selectField.Append(PropertySegExpress.ToString(rootObjName, ctx, ignoreGL));
            }
            else
            {
                selectField.Append(ctx.GetSimpleEntityAlias(this.EntityAlias)).Append('.').Append(this.Field);
                if (this.PeropertyType is ISimpleProperty simpleProperty && simpleProperty.PrivacyType != 0)
                {
                    selectField.Append(EntityConst.PrivacyPropertyFieldSuffix);
                }
                else if (this.PeropertyType is ISimpleProperty simpleProperty2 && simpleProperty2.IsEncrypt())
                {
                    selectField.Append(EntityConst.EncryptPropertyFieldSuffix);
                }
            }
            if (this.MultiLangProperty && !ignoreGL && IsGLField())
            {
                string parentEntityAlias = this.FullObjectName;
                string glField = ctx.GetSimpleEntityAlias(parentEntityAlias) + '.' + this.PeropertyType.Alias;
                var tempSb = new System.Text.StringBuilder(128);
                bool isSimpleProp = this.PeropertyType is ISimpleProperty;
                if (isSimpleProp && ((ISimpleProperty)this.PeropertyType).IsEncrypt())
                {
                    string glField2 = glField + EntityConst.EncryptPropertyFieldSuffix;
                    tempSb.Append("CASE WHEN ").Append(selectField).Append(" IS NULL THEN ").Append(glField2)
                          .Append(" WHEN TO_CHAR(").Append(selectField).Append(") = '")
                          .Append(GetEncryptedData("")).Append("' THEN ").Append(glField2)
                          .Append(" WHEN TO_CHAR(").Append(selectField).Append(") = '")
                          .Append(GetEncryptedData(" ")).Append("' THEN ").Append(glField2)
                          .Append(" ELSE ").Append(selectField).Append(" END");
                }
                else if (isSimpleProp && ((ISimpleProperty)this.PeropertyType).PrivacyType != 0)
                {
                    tempSb.Append("CASE WHEN ").Append(selectField).Append(" IS NULL THEN ")
                          .Append(glField + EntityConst.PrivacyPropertyFieldSuffix).Append(" ELSE ").Append(selectField).Append(" END");
                }
                else if (ORMConfiguration.UseSingleLang())
                {
                    tempSb.Append(glField);
                }
                else
                {
                    tempSb.Append("CASE WHEN ").Append(selectField).Append(" IS NULL THEN ").Append(glField)
                          .Append(" WHEN ").Append(selectField).Append(" = '' THEN ").Append(glField)
                          .Append(" WHEN ").Append(selectField).Append(" = ' ' THEN ").Append(glField)
                          .Append(" ELSE ").Append(selectField).Append(" END");
                }
                selectField = tempSb;
            }
            if (withAlias)
            {
                selectField.Append(GenShortAlias(selectField.ToString(), newWhenSame));
            }
            return selectField.ToString();
        }

        public bool IsGLField()
        {
            ISimpleProperty mainProperty;

            if (this._glField != null)
            {
                return this._glField.Get();
            }
            this._glField = new AtomicBoolean();
            IDataEntityType mainDT = this.PeropertyType.Parent.Parent;
            if (mainDT != null && (mainProperty = (ISimpleProperty)mainDT.Properties[this.Name]) != null &&
                !mainProperty.IsDbIgnore() && !this.PeropertyType.IsDbIgnore())
            {
                this._glField.Set(true);
                return true;
            }
            return false;
        }

        private string GetEncryptedData(string str)
        {
            if (CachedEncryptData.ContainsKey(str))
            {
                return CachedEncryptData[str];
            }
            string v = EncrypterFactory.GetEncrypter().Encode(str);
            CachedEncryptData[str] = v;
            return v;
        }

        private string GenShortAlias(string selectField, bool newWhenSame)
        {
            if (this.Alias.Length <= MaxLength)
            {
                return $" \"{this.Alias}\"";
            }
            if (selectField.Length < MaxLength && !newWhenSame)
            {
                return $" \"{selectField}\"";
            }
            int h = selectField.GetHashCode();
            if (h == int.MinValue)
            {
                h = 0;
            }
            else if (h < 0)
            {
                h = -h;
            }
            return $" \"_F_{h}\"";
        }

        public bool MultiLangProperty { get; set; }

        public EntityItemProperty PropertyItem { get; set; }

        public bool InnerField => this.Alias != null && this.Alias.StartsWith(ORMImpl.QueryInnerPkPrefix);


        public PropertySegExpress PropertySegExpress { get; set; }

        public string OriginalPropertyString
        {
            get
            {
                var s = new System.Text.StringBuilder(128);
                if (IsExpress)
                {
                    s.Append(this.PropertySegExpress);
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.FullObjectName))
                    {
                        s.Append(this.FullObjectName).Append('.');
                    }
                    s.Append(this.Name);
                }
                return s.ToString();
            }
        }

        public override string ToString()
        {
            var s = new System.Text.StringBuilder(OriginalPropertyString);
            if (this.Alias != null)
            {
                s.Append(' ').Append('\"').Append(this.Alias).Append('\"');
            }
            return s.ToString();
        }


    }
}