namespace WYF.Algo
{
    /// <summary>
    /// 可以指定Join的方式，是使用nest,merge还是其他方式
    /// </summary>
    public class JoinHint: Hint
    {
        /// <summary>
        /// 是否当值为Null时，当做0处理
        /// </summary>
        public bool IsNullAsZero {  get; set; }

        public  bool IsUseHHJ {  get; set; }
        /// <summary>
        /// join的方式是否为nest
        /// </summary>
        public bool IsUseNest { get; set; }
        /// <summary>
        /// 是否是merge方式的join
        /// </summary>
        public bool IsUseMerge { get; set; }
    }
}