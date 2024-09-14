using Antlr4.Runtime.Misc;
using WYF.Bos.algo;
using WYF.DataEntity.Metadata;
using WYF.Bos.db;
using WYF.Bos.lang;
using WYF.Bos.Orm.impl;
using WYF.Bos.Orm.query.crossdb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Bos.Orm.query.multi
{
    public class SingleQuery
    {
        private string _parentFK;
        private IDataEntityType _entityType;
        private bool withCrossDBObjectOrFilter = false;
        private string _fullObjName;
        private PropertyField[] _selectFields;
        private List<PropertyField> _performJoinFieldList;
        private Dictionary<string, QFilter> _allJoinFilterMap;
        private Dictionary<string, QFilter> _allFilterMap;
        private List<OrderByInfo> _allOrderByList;
        private List<GroupByInfo> _allGroupByList;
        private Dictionary<string, QFilter> _allHavingFilterMap;
        private bool _withUserOriginFilter;
        private bool _orderByOnSQL;
        private int _top;
        private int _start;
        private int _limit;
        private ISet<string> _curFilterObjSet = new HashSet<string>(); 
        private DBRoute _dbRoute;
        private QContext _allCtx;
        private QContext _ctx;
        private QueryParameter _queryParameter;
        private ISet<QFilter> _usedFilterAndHaving = new HashSet<QFilter>();
        private HashSet<string> _myFullObjNameSet = new HashSet<string>();
        private bool _shouldRebuildSQL = false;
        private static readonly string TempPrefix = "TEMP_".ToLower();
        public SingleQuery(DBRoute dbRoute, String parentFK, IDataEntityType entityType, String fullObjName, PropertyField[] selectFields, List<PropertyField> performJoinFieldList, Dictionary<String, QFilter> allJoinFilterMap, Dictionary<String, QFilter> allFilterMap, List<OrderByInfo> allOrderByList, List<GroupByInfo> allGroupByList, Dictionary<String, QFilter> allHavingFilterMap, int top, int start, int limit, QContext allCtx, bool withUserOriginFilter, bool initQueryParameter, bool orderByOnSQL)
        {
            this._parentFK = parentFK;
            this._entityType = entityType;
            this._fullObjName = fullObjName;
            this._selectFields = selectFields;
            this._performJoinFieldList = performJoinFieldList;
            this._allJoinFilterMap = allJoinFilterMap;
            this._allFilterMap = allFilterMap;
            this._allOrderByList = allOrderByList;
            this._allGroupByList = allGroupByList;
            this._allHavingFilterMap = allHavingFilterMap;
            this._withUserOriginFilter = withUserOriginFilter;
            this._orderByOnSQL = orderByOnSQL;
            this._top = top;
            this._start = start;
            this._limit = limit;
            this._curFilterObjSet.Add(fullObjName);
            //this._dbRoute = ReuseDBRoute.useDBRoute(entityType, dbRoute);
            this._dbRoute = dbRoute;
            this._allCtx = allCtx;
            bool silenceHandleAllFilterAndOrderAndGroupBy = !initQueryParameter;
            //MergeDBBeacon.markAboutHandleAllFilterAndOrderAndGroupBy();
            this._ctx = new QContext(entityType, fullObjName, selectFields, allCtx.EntityTypeCache, allCtx.ORMHint, allCtx.Distinctable, silenceHandleAllFilterAndOrderAndGroupBy, allCtx.SimpleEntityAliasMap, allCtx);
           // TenantAccountCrossDBRuntime.parseEntityTables(entityType, allCtx.getEntityTypeCache());
            this._queryParameter = CreateQueryParameter();



        }

        public bool ForceInnerJoin { get; set; }= false;

        public string FullObjName => this._fullObjName;

        public QContext AllCtx => this._allCtx;

        public DataSet Query(string algoKey, bool finallySingleQuery)
        {
            if (finallySingleQuery)
            {
                List<PropertyField> pfs = new List<PropertyField>(this._selectFields.Length);
                foreach (PropertyField pf in this._selectFields)
                {
                    if (!pf.IsInnerField)
                        pfs.Add(pf);
                }
                if (pfs.Count() < this._selectFields.Length)
                {
                    this._selectFields = pfs.ToArray();
                    this._shouldRebuildSQL = true;
                }
            }
            if (this._shouldRebuildSQL)
                this._queryParameter = CreateQueryParameter();
            string sql = this._queryParameter.Sql;
            if (finallySingleQuery && this._top >= 0)
                if (sql.StartsWith("SELECT DISTINCT "))
                {
                    if (this._limit > 0)
                    {
                        sql = "SELECT DISTINCT TOP " + this._limit + "," + this._start + ' ' + sql.Substring("SELECT DISTINCT ".Length);
                    }
                    else
                    {
                        sql = "SELECT DISTINCT TOP " + this._top + ' ' + sql.Substring("SELECT DISTINCT ".Length);
                    }
                }
                else if (this._limit > 0)
                {
                    sql = "SELECT TOP " + this._limit + "," + this._start + ' ' + sql.Substring("SELECT ".Length);
                }
                else
                {
                    sql = "SELECT TOP " + this._top + ' ' + sql.Substring("SELECT ".Length);
                }
            string fullSQL = "/*ORM*/ " + sql;
            DataSet ds = DBExt.queryDataSet(algoKey, this._dbRoute, fullSQL, this._queryParameter.Parameters, CreateQueryMeta());
            return ds;
        }

        private QueryMeta CreateQueryMeta()
        {
            return null;
        }

        private QueryParameter CreateQueryParameter()
        {
            if (this._performJoinFieldList != null)
            {
                foreach (var field in this._performJoinFieldList)
                {
                    this._ctx.PutPerformJoinField(field);
                }
            }
            QFilter whereFilter = this._allFilterMap.GetValueOrDefault(this._fullObjName);
            if (whereFilter != null)
                whereFilter = whereFilter.Copy();
            QFilter havingFilter = this._allHavingFilterMap.GetValueOrDefault(this._fullObjName);
            if (havingFilter != null)
                havingFilter = havingFilter.Copy();
            this._myFullObjNameSet.Add(this._fullObjName);
            foreach (JoinTableInfo join in this._ctx.JoinTableList)
            {
                JoinTableTypeEnum type = join.JoinTableType;
                if (type == JoinTableTypeEnum.extend || type == JoinTableTypeEnum.multi_lang)
                    continue;
                string joinFullObjName = join.EntityItem.FullObjectName;
                QFilter and = this._allFilterMap.GetValueOrDefault(joinFullObjName);
                if (and != null)
                    and = and.Copy();
                if (whereFilter == null)
                {
                    whereFilter = and;
                }
                else if (and != null)
                {
                    whereFilter = whereFilter.And(and);
                }
                and = this._allHavingFilterMap.GetValueOrDefault(joinFullObjName,null);
                if (and != null)
                    and = and.Copy();
                if (havingFilter == null)
                {
                    havingFilter = and;
                }
                else if (and != null)
                {
                    havingFilter = havingFilter.And(and);
                }
                this._myFullObjNameSet.Add(joinFullObjName);
            }
            return DoCreateQueryParameter(whereFilter, havingFilter, new Dictionary<JoinTableInfo, JoinTableInfo>(), null);
        }

        private QueryParameter DoCreateQueryParameter(QFilter whereFilter, QFilter havingFilter, Dictionary<JoinTableInfo, JoinTableInfo> replaceTableInfoMap, JoinTableInfo replaceFromJoinTableInfo)
        {
            QueryParameter ret = new QueryParameter();
            string sqlWhere = null;
            QParameter whereQP = null;
            if (whereFilter != null)
            {
                whereQP = whereFilter.ToQParameter(this._ctx);
                sqlWhere = whereQP.GetSql();
            }
            this._myFullObjNameSet.UnionWith(this._ctx.CurrentSelectObjectSet);
            string sqlOrderBy = null;
            if (this._orderByOnSQL)
                foreach (OrderByInfo orderBy in GetOrderInfoList())
                {
                    string seg = this._ctx.GetOrderBy(new OrderByInfo[] { orderBy });
                    if (sqlOrderBy == null)
                    {
                        sqlOrderBy = "\r\nORDER BY " + seg;
                        continue;
                    }
                    sqlOrderBy = sqlOrderBy + ", " + seg;
                }

            string sqlGroupBy = null;
            foreach (GroupByInfo groupBy in GetGroupByInfoList())
            {
                String seg = this._ctx.GetGroupBy(new GroupByInfo[] { groupBy });
                if (sqlGroupBy == null)
                {
                    sqlGroupBy = "\r\nGROUP BY " + seg;
                    continue;
                }
                sqlGroupBy = sqlGroupBy + ", " + seg;
            }
            string sqlHaving = null;
            QParameter havingQP = null;
            if (havingFilter != null)
            {
                havingQP = havingFilter.ToQParameter(this._ctx);
                sqlHaving = "\r\nHAVING " + havingQP.GetSql();
            }
            SortJoinTable();
            StringBuilder sql = new StringBuilder(1024);
            sql.Append("SELECT ");
            bool hasDistinct = false;
            if (this._ctx.Distinctable != null && this._fullObjName.IndexOf('.') == -1)
            {
                Dictionary<string, bool> joinEntitySelectFieldMap = new Dictionary<string, bool>();
                foreach (JoinTableInfo join in this._ctx.JoinTableList)
                    joinEntitySelectFieldMap[join.EntityItem.FullObjectName] = false;
                foreach (PropertyField f in this._selectFields)
                {
                    if (f.IsExpress)
                    {
                        if (f.PropertySegExpress != null)
                            foreach (PropertyField ef in f.PropertySegExpress.PropertyFields)
                            {
                                if (joinEntitySelectFieldMap.ContainsKey(ef.FullObjectName))
                                    joinEntitySelectFieldMap[ef.FullObjectName] = true;

                            }
                    }
                    else if (joinEntitySelectFieldMap.ContainsKey(f.FullObjectName))
                    {
                        joinEntitySelectFieldMap[f.FullObjectName] = true;

                    }
                }
                if (this._ctx.Distinctable.Distinct(this._entityType, joinEntitySelectFieldMap))
                {
                    sql.Append("DISTINCT ");
                    hasDistinct = true;
                }
            }
            if (!hasDistinct)
                foreach (JoinTableInfo join in this._ctx.JoinTableList)
                {
                    EntityItem ei = join.EntityItem;
                    if (ORMConfiguration.IsMulBasedata(ei.EntityType))
                    {
                        sql.Append("DISTINCT ");
                        hasDistinct = true;
                        break;
                    }
                }
            this.withCrossDBObjectOrFilter = HasCrossDBObjectOrFilter(whereFilter, havingFilter);
            sql.Append(this._ctx.GetSelects(this._fullObjName, this._selectFields));
            sql.Append("\r\nFROM ");
            QParameter langAsMainQP = null;
            if (replaceFromJoinTableInfo != null)
            {
              
            }
            else
            {
                sql.Append(TenantAccountCrossDBRuntime.GetCrossDBTable(this._entityType.Alias, this._dbRoute.RouteKey, this.withCrossDBObjectOrFilter))
                  .Append(' ').Append(this._ctx.GetSimpleEntityAlias(this._fullObjName));
            }
            List<JoinTableInfo> joinWithFilters = new List<JoinTableInfo>();
            List<JoinTableInfo> joinTableList = new List<JoinTableInfo>(this._ctx.JoinTableList);
      
            for (int i = 0; i < joinTableList.Count; i++)
            {
                JoinTableInfo join = joinTableList[i];
                if (replaceTableInfoMap.ContainsValue(join) || join == replaceFromJoinTableInfo)
                    continue;
                JoinTableInfo mainJoin = replaceTableInfoMap.GetValueOrDefault(join, null);
                if (mainJoin != null)
                {
                    join = join.Copy();
                    join.JoinField = mainJoin.JoinField;
                    join.JoinTableAlias = mainJoin.JoinTableAlias;
                }
                JoinTableTypeEnum type = join.JoinTableType;
                if (type != JoinTableTypeEnum.extend && type != JoinTableTypeEnum.multi_lang)
                {
                    QFilter joinFilter = this._allJoinFilterMap.GetValueOrDefault(join.EntityItem.FullObjectName,null);
                    if (joinFilter != null && !joinFilter.IsJoinSQLFilter())
                    {
                        if (joinFilter.IsOnMetaJoinPropertyFilter())
                        {
                            join.JoinFilter= joinFilter;
                        }
                        else
                        {
                            List<QFilter.QFilterNest> joinFilterNests = joinFilter.GetNests(false);
                            if (joinFilterNests.Count > 0)
                            {
                                if (!joinFilterNests[0].IsAnd)
                                    throw new ArgumentException("JoinFilter格式不正确，请用QFilter.join创建。");
                                QFilter nestFilter = joinFilter.Copy();
                                nestFilter.MaskCurrent();
                                join.JoinFilter = nestFilter;
                            }
                        }
                        joinWithFilters.Add(join);
                    }
                }
                sql.Append(join.ToJoinSql(this._dbRoute.RouteKey, this.withCrossDBObjectOrFilter, this._ctx));
            }

            if (joinTableList.Count() != this._ctx.JoinTableList.Count())
                throw new ArgumentException("on条件必须是在join的对象上。");
            List<Object[]> joinSQLParams = new List<Object[]>();
            List<QFilter> joinFilters = new List<QFilter>();
            foreach (string objectName in this._myFullObjNameSet)
            {
                QFilter joinFilter = this._allJoinFilterMap.GetValueOrDefault(objectName);
                joinFilters.Clear();
                if (joinFilter != null && joinFilter.IsJoinSQLFilter())
                {
                    joinFilters.Add(joinFilter);
                    List<QFilter.QFilterNest> nests = joinFilter.GetNests(true);
                    foreach (QFilter.QFilterNest nest in nests)
                    {
                        if (nest.Filter.IsJoinSQLFilter())
                        {
                            if (!nest.IsAnd)
                                throw new Exception("Join filter cann't be or: " + joinFilter);
                            joinFilters.Add(nest.Filter);
                        }
                    }
                    foreach (QFilter joinFilterItem in joinFilters)
                    {
                        sql.Append("\r\n");
                        QParameter qp = joinFilterItem.SelfDefinedQParameter;
                        sql.Append(qp.GetSql());
                        joinSQLParams.Add(qp.Parameters);
                    }
                }
            }
            if (sqlWhere != null)
                sql.Append("\r\nWHERE ").Append(sqlWhere);
            if (sqlGroupBy != null)
                sql.Append(sqlGroupBy);
            if (sqlHaving != null)
                sql.Append(sqlHaving);
            if (sqlOrderBy != null)
                sql.Append(sqlOrderBy);
            ret.Sql = sql.ToString();
            if (whereFilter != null)
                this._usedFilterAndHaving.Add(whereFilter);
            if (havingFilter != null)
                this._usedFilterAndHaving.Add(havingFilter);
            foreach (JoinTableInfo join in joinWithFilters)
                ret.AddParams(join.JoinParameters);
            foreach (Object[] ps in joinSQLParams)
                ret.AddParams(ps);
            if (whereQP != null)
                ret.AddParams(whereQP.Parameters);
            if (havingQP != null)
                ret.AddParams(havingQP.Parameters);
            if (langAsMainQP != null)
                ret.AddParams(langAsMainQP.Parameters);
            ret.GenQueryParameterWhereQFilter = whereFilter;

            if (ret.Parameters != null && ret.Parameters.Length > 0) {
                bool hasRemoveParameter = false;
                foreach (object param in ret.Parameters)
                {
                    //if (param == HugeInUtil.REMOVED_PARAMETER)
                    //{
                    //    hasRemoveParameter = true;
                    //    break;
                    //}
                }
                if (hasRemoveParameter)
                {
                    List<object> newParams = new List<object>(ret.Parameters.Length);
                    //foreach (object param in ret.Parameters)
                    //{
                    //    if (param != HugeInUtil.REMOVED_PARAMETER)
                    //        newParams.Add(param);
                    //}
                    ret.Parameters = newParams.ToArray();
                }
            }
            return ret;
        }


        public bool WithWhereFilter()
        {
            foreach (string objName in this._curFilterObjSet)
            {
                QFilter objFilter = this._allFilterMap.GetValueOrDefault(objName);
                if (objFilter != null)
                    return true;
            }
            return false;
        }
        private bool HasCrossDBObjectOrFilter(QFilter whereFilter, QFilter havingFilter)
        {
            return (HasCrossDBObjectOrFilter(whereFilter) || HasCrossDBObjectOrFilter(havingFilter));
        }

        private bool HasCrossDBObjectOrFilter(QFilter havingFilter)
        {
            return false;
        }
        public bool WithUserOriginFilter
        {
            get
            {
                return this._withUserOriginFilter;
            }
            set
            {
                this._withUserOriginFilter = value;
                if (_withUserOriginFilter)
                {
                    this.ForceInnerJoin = true;
                }
            }
        }
  
        private void SortJoinTable()
        {
            this._ctx.JoinTableList.Sort((o1, o2) =>
            {
                string a1 = o1.TableAlias.ToLower();
                string a2 = o2.TableAlias.ToLower();
                if (a1.StartsWith(SingleQuery.TempPrefix) || a2.StartsWith(SingleQuery.TempPrefix))
                    return o1.JoinTableAlias.CompareTo(o2.JoinTableAlias);
                int level1 = a1.Length - a1.Replace(".", "").Length;
                int level2 = a2.Length - a2.Replace(".", "").Length;
                int dt = level1 - level2;
                if (dt == 0)
                    dt = a1.CompareTo(a2);
                return dt;
            });
        }

        public List<OrderByInfo> GetOrderInfoList()
        {
            List<OrderByInfo> ret = new ArrayList<OrderByInfo>();
            foreach (OrderByInfo orderBy in this._allOrderByList)
            {
                if (this._myFullObjNameSet.Contains(orderBy.FullObjectName))
                    ret.Add(orderBy);
            }
            return ret;

        }

        public List<GroupByInfo> GetGroupByInfoList()
        {
            List<GroupByInfo> ret = new ArrayList<GroupByInfo>();
            foreach (GroupByInfo groupBy in this._allGroupByList)
            {
                if (this._myFullObjNameSet.Contains(groupBy.FullObjectName))
                    ret.Add(groupBy);
            }
            return ret;
        }


        private class QueryParameter
        {
            String sql;

            Object[] parameters;

            public QFilter GenQueryParameterWhereQFilter { get; set; }

            public string Sql { get => sql;set { this.sql = value; } }
            public object[] Parameters { get => parameters;set { this.parameters = value; } }

            public void AddParams(object[] parameters)
            {
                if (parameters != null && parameters.Length > 0)
                {
                    if (this.parameters == null || this.parameters.Length == 0)
                    {
                        this.parameters = parameters;
                    }
                    else
                    {
                        object[] both = new object[this.parameters.Length + parameters.Length];
                        Array.Copy(this.parameters, 0, both, 0, this.parameters.Length);
                        Array.Copy(parameters, 0, both, this.parameters.Length, parameters.Length);
                        this.parameters = both;
                    }
                }
            }


            public override String ToString()
            {
                return this.sql;
            }

       
        }
    }
}
