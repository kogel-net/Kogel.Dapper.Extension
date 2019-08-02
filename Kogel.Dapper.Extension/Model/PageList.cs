using System.Collections.Generic;

namespace Kogel.Dapper.Extension.Model
{
    /// <summary>
    /// 分页实体类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageList<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页项</param>
        /// <param name="totalCount">总数</param>
        /// <param name="items">元素</param>
        public PageList(int pageIndex, int pageSize, int totalCount, List<T> items)
        {
            Total = totalCount;
            PageSize = pageSize;
            PageIndex = pageIndex;
            Items = items;
            TotalPage = Total % PageSize == 0 ? Total / PageSize : Total / PageSize + 1;
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; }

        /// <summary>
        /// 元素
        /// </summary>
        public List<T> Items { get; }

        /// <summary>
        /// 页项
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage { get; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool HasPrev => PageIndex > 1;

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool HasNext => PageIndex < TotalPage;
    }
}
