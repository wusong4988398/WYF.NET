using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WYF.DataEntity.Entity;
using WYF.DataEntity.Metadata.Dynamicobject;

namespace WYF.Entity.Property
{
    public class TreeEntryProp : EntryProp
    {
        private static readonly long serialVersionUID = -6203696348595789107L;

        public TreeEntryProp()
        {
        }

        public TreeEntryProp(string name, DynamicObjectType dynamicItemPropertyType) : base(name, dynamicItemPropertyType)
        {
        }

        public static int GetTreeEntryInsertPosition(DynamicObject[] entryRows, int row)
        {
            var pidSet = new HashSet<long>();
            for (int i = row - 1; i >= 0; i--)
            {
                
                long? pid = (long?)entryRows[i]["pid"];
                if (pid == null || pid == 0)
                    break;
                pidSet.Add(pid.Value);
            }

            int pos = -1;
            long? entryPId = (long?)entryRows[row]["pid"];
            for (int j = row + 1; j < entryRows.Length; j++)
            {
                long? pid = (long?)entryRows[j]["pid"];
                if (pid == null || pid == 0 || pid == entryPId)
                {
                    pos = j;
                    break;
                }

                if (pidSet.Contains(pid.Value))
                {
                    pos = j;
                    break;
                }
            }

            return pos != -1 ? pos : entryRows.Length;
        }

        public static int GetTreeEntryInsertPosition(DynamicObjectCollection entryRows, int row)
        {
            return GetTreeEntryInsertPosition(entryRows.Cast<DynamicObject>().ToArray(), row);
        }

        public static int GetEntryNextRowCount(DynamicObjectCollection entryRows, int row)
        {
            int count = 0;
            object pid = entryRows[row]["pid"];
            for (int i = row + 1; i < entryRows.Count; i++)
            {
                if (pid.Equals(entryRows[i]["pid"]))
                    count++;
            }
            return count;
        }

        public static int[] GetEntryNextRows(DynamicObjectCollection entryRows, int row, bool includeSelf)
        {
            var nextRows = new List<int>();
            if (includeSelf)
                nextRows.Add(row);

            object pid = entryRows[row]["pid"];
            for (int i = row + 1; i < entryRows.Count; i++)
            {
                if (pid.Equals(entryRows[i]["pid"]))
                    nextRows.Add(i);
            }

            return nextRows.ToArray();
        }

        public static bool CanDeleteTreeEntryRows(DynamicObjectCollection entryRows, int[] rows)
        {
            var childEntryIds = new Dictionary<object, List<object>>();
            var delEntryIds = new HashSet<object>();

            foreach (int i in rows)
            {
                object entryId = entryRows[i].PkValue;
                delEntryIds.Add(entryId);
            }

            foreach (DynamicObject entryRow in entryRows)
            {
                object pid = entryRow["pid"];
                if (!childEntryIds.ContainsKey(pid))
                    childEntryIds[pid] = new List<object>();

                childEntryIds[pid].Add(entryRow.PkValue);
            }

            foreach (object entryId in delEntryIds)
            {
                if (childEntryIds.ContainsKey(entryId))
                {
                    foreach (object childEntryId in childEntryIds[entryId])
                    {
                        if (!delEntryIds.Contains(childEntryId))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
