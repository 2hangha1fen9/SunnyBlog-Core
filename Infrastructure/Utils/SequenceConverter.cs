using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utils
{
    //将字符串转换为筛选条件集合
    public static class SequenceConverter
    {
        /// <summary>
        /// JSON条件格式转集合格式
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static List<SearchCondition> ConvertToCondition(string condition)
        {
            List<SearchCondition> con = new List<SearchCondition>();
            try { con = JsonConvert.DeserializeObject<List<SearchCondition>>(condition); }
            catch (Exception) { }
            return con;
        }
    }
}
