using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public class Response
    {
        /// <summary>
        /// 操作消息 当Status不为200时，显示详细的错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 操作状态码，200正常
        /// </summary>
        public int Status { get; set; }
        public Response()
        {
            Status = 200;
            Message = "操作成功";
        }
    }

    /// <summary>
    /// WebApi通用返回泛型基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Response<T> : Response
    {
        /// <summary>
        /// 回传结果
        /// </summary>
        public T Result { get; set; }
    }

    /// <summary>
    /// WebApi分页列表通用返回泛型基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PResponse<T> : Response
    {
        /// <summary>
        /// 回传结果
        /// </summary>
        public List<T> Result { get; set; }
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
        /// 对结果分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="list"></param>
        public void Pagination(int pageIndex,int pageSize,List<T> list)
        {
            TotalCount = list.Count();
            PageSize = pageSize < 0 ? 10 : pageSize;
            TotalPages = Convert.ToInt32(Math.Ceiling((double)list.Count / (double)PageSize));
            PageIndex = pageIndex < 0 ? 1 : pageIndex > TotalPages ? TotalPages : pageIndex;
            Result = list.Skip(PageSize * (PageIndex - 1)).Take(PageSize).ToList();
        }
    }
}
