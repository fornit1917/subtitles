using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Subtitles.Models
{
    public class PageResult<T>
    {
        public IReadOnlyList<T> Items { get; }
        public int TotalCount { get; }
        public int Offset { get; }

        public bool HasNext => Offset + Items.Count < TotalCount;
        public bool HasPrev => Offset > 0;

        public PageResult(IReadOnlyList<T> items, int totalCount, int offset)
        {
            Items = items;
            TotalCount = totalCount;
            Offset = offset;
        }
    }
}
