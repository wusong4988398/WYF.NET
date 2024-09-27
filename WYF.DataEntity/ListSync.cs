

using System.Collections;
using WYF.DataEntity.Entity;


namespace WYF.Bos.DataEntity
{
  

    public static class ListSync
    {
        public static void Sync<SourceT, TargetT>(ICollection<SourceT> sourceList, ICollection<TargetT> targetList, ListSyncFunction<SourceT, TargetT> syncFunction, bool callUpdateFuncWhenCreated)
        {
            if (sourceList == null)
                throw new ArgumentException(nameof(sourceList));
            if (targetList == null)
                throw new ArgumentException(nameof(targetList));

            TargetT[] targetArray = targetList.ToArray();
            List<int> indexArray = new List<int>(targetList.Count);
            for (int i = 0; i < targetList.Count; i++)
                indexArray.Add(i);

            foreach (SourceT sourceItem in sourceList)
            {
                bool found = false;
                for (int j = 0; j < targetArray.Length; j++)
                {
                    TargetT targetItem = targetArray[j];
                    if (syncFunction.EqualsFunc(sourceItem, targetItem))
                    {
                      
                        syncFunction.UpdateFunc(sourceItem, targetItem);
                        if (indexArray.Contains(j))
                        {
                            found = true;
                            indexArray.Remove(j);
                            break;
                        }
                    }
                }
                if (!found)
                {
                    TargetT targetItem = syncFunction.CreateFunc(sourceItem);
                    if (targetItem != null)
                    {
                        if (callUpdateFuncWhenCreated)
                            syncFunction.UpdateFunc(sourceItem, targetItem);
                        syncFunction.AddFunc(targetList, targetItem);
                    }
                }
            }

            foreach (int index in indexArray)
            {
                syncFunction.RemoveFunc(targetList, targetArray[index], index);
            }
        }

        public static void Sync<SourceT, TargetT>(IEnumerable<SourceT> sourceList, IEnumerable<TargetT> targetList, Func<SourceT, TargetT, bool> equatable, Action<SourceT, TargetT> updateFunc, Func<SourceT, TargetT> createFunc, Action<IEnumerable<TargetT>, TargetT> addFunc = null, Action<IEnumerable<TargetT>, TargetT, int> removeFunc = null, bool callUpdateFuncWhenCreated = true)
        {
            if (sourceList == null)
            {
                throw new ArgumentNullException("sourceList");
            }
            if (targetList == null)
            {
                throw new ArgumentNullException("targetList");
            }

            TargetT[] targetArray = targetList.ToArray();

            List<int> indexArray = new List<int>(targetList.Count());
            for (int i = 0; i < targetList.Count(); i++)
            {
                indexArray.Add(i);
            }
            foreach (SourceT sourceItem in sourceList)
            {
                bool Found = false;
                for (int j = 0; j < targetArray.Length; j++)
                {
                    TargetT targetItem = targetArray[j];
                    if (equatable(sourceItem, targetItem))
                    {
                        updateFunc(sourceItem, targetItem);
                        if (indexArray.Contains(j))
                        {
                            Found = true;
                            indexArray.Remove(j);
                            break;
                        }
                    }

                }

                if (!Found)
                {
                    TargetT targetItem = createFunc(sourceItem);
                    if (targetItem == null)
                    {
                        if (callUpdateFuncWhenCreated)
                        {
                            updateFunc(sourceItem, targetItem);
                        }
                        addFunc(targetList, targetItem);
                    }

                }


            }

            foreach (int index in indexArray)
            {
                removeFunc(targetList, targetArray[index], index);
            }

        }

    }
}
