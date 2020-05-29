using System;

namespace CoreApi.Models
{
    public interface ITimespan
    {
        DateTimeOffset DateCreated { get; set; }
        DateTimeOffset DateUpdated { get; set; }
    }
}
