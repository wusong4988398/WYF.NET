using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.OrmEngine.Query.Multi
{
    public class JoinTableInfo
    {
        public EntityItem EntityItem { get; private set; }

        public string Table { get; set; }

        public string TableAlias { get; set; }

        public string Field { get; set; }

        public ORMHint.JoinHint Join { get; set; }

        public JoinTableTypeEnum JoinTableType { get; private set; }

        public string JoinTableAlias { get; set; }

        public string JoinField { get; set; }

        public string AndExpress { get; private set; }

        public QFilter JoinFilter { get; set; }

        public Object[] JoinParameters { get; private set; }

        public JoinTableInfo Copy()
        {
            JoinTableInfo copy = new JoinTableInfo(this.EntityItem, this.JoinTableType);
            copy.Table = this.Table;
            copy.TableAlias = this.TableAlias;
            copy.Field = this.Field;
            copy.Join = this.Join;
            copy.JoinTableAlias = this.JoinTableAlias;
            copy.JoinField = this.JoinField;
            copy.AndExpress = this.AndExpress;
            copy.JoinFilter = this.JoinFilter;
            copy.JoinParameters = this.JoinParameters;
            return copy;
        }
        public string ToJoinSql(string crossToRouteKey, bool withCrossDBObjectOrFilter, QContext ctx)
        {
            return ToJoinString(ctx.GetSimpleEntityAlias(this.TableAlias), ctx.GetSimpleEntityAlias(this.JoinTableAlias), crossToRouteKey, withCrossDBObjectOrFilter, ctx);
        }

        private string ToJoinString(String tableAlias, String joinTableAlias, String crossToRouteKey, bool withCrossDBObjectOrFilter, QContext ctx)
        {
            StringBuilder sb = new StringBuilder(128);
            sb.Append("\r\n");
            sb.Append(this.Join.GetDescription<ORMHint.JoinHint>());
            sb.Append(' ');
            if (crossToRouteKey == null)
            {
                sb.Append(this.Table);
            }
            else
            {
                //sb.Append(TenantAccountCrossDBRuntime.GetCrossDBTable(this.Table, crossToRouteKey, withCrossDBObjectOrFilter));
            }
            sb.Append(' ');
            sb.Append(tableAlias);
            sb.Append(" ON ");
            sb.Append(tableAlias).Append('.').Append(this.Field).Append('=').Append(joinTableAlias).Append('.')
              .Append(this.JoinField);
            if (this.AndExpress != null)
                sb.Append(" AND ").Append(this.AndExpress);
            if (this.JoinFilter != null)
                if (ctx != null)
                {
                    QParameter qp = this.JoinFilter.ToQParameter(ctx);
                    sb.Append(" AND ").Append(qp.GetSql());
                    this.JoinParameters = qp.Parameters;
                }
                else
                {
                    sb.Append(" AND ").Append(this.JoinFilter);
                }
            return sb.ToString();
        }
        public JoinTableInfo(EntityItem entityItem, JoinTableTypeEnum joinTableType)
        {
            this.EntityItem = entityItem;
            this.JoinTableType = joinTableType;
        }

    }
}
