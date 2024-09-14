

using System.Collections;


namespace WYF.Bos.DataEntity
{
    public static class ListSync
    {
        // Methods
        private static void DefaultAdd<TargetT>(IEnumerable<TargetT> list, TargetT newItem)
        {
            ((IList<TargetT>)list).Add(newItem);
        }

        private static void DefaultAdd(IEnumerable list, object newItem)
        {
            ((IList)list).Add(newItem);
        }

        private static void DefaultRemove<TargetT>(IEnumerable<TargetT> list, TargetT removeItem, int index)
        {
            ((IList<TargetT>)list).RemoveAt(index);
        }

        private static void DefaultRemove(IEnumerable list, object removeItem, int index)
        {
            ((IList)list).RemoveAt(index);
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
