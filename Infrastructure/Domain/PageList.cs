using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    /// <summary>
    /// 分页结果
    /// </summary>
    /// <typeparam name="V">数据类型</typeparam>
    public class PageList<V>
    {
        /// <summary>
        /// 回传结果
        /// </summary>
        public List<V> Page { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页多少条记录
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 添加分页表达式
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize">-1表示全表查询</param>
        /// <param name="list"></param>
        public IQueryable<T> Pagination<T>(int pageIndex, int pageSize, IQueryable<T> list)
        {
            TotalCount = list.Count();
            if(TotalCount > 0)
            {
                PageSize = pageSize < 0 ? TotalCount : pageSize;
                TotalPages = Convert.ToInt32(Math.Ceiling((double)TotalCount / (double)PageSize));
                PageIndex = pageIndex < 0 ? 1 : pageIndex > TotalPages ? TotalPages : pageIndex;
                return list.Skip(PageSize * (PageIndex - 1)).Take(PageSize);
            }
            return list;
        }
    }
}
