using MockSchoolManagement.Models;
using System;
using System.Collections.Generic;

namespace MockSchoolManagement.Application.Dtos
{
    public class PagedResultDto<TEntity> : PagedSortedAndFilterInput
    {
        /// <summary>
        ///  数据总合计
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(TotalCount, MaxResultCount));

        public List<TEntity> Data { get; set;}


        public bool ShowPrevious => CurrentPage > 1;
        public bool ShowNext => CurrentPage < TotalPages;
        public bool ShowFirst => CurrentPage != 1;
        public bool ShowLast => CurrentPage != TotalPages;
    }
}
