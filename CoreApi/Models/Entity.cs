using System.ComponentModel.DataAnnotations;

namespace CoreApi.Models
{
    /// <summary>
    /// Base Business Model
    /// </summary>
    /// <typeparam name="T">Type of Id Property</typeparam>
    public class Entity<T>
    {
        [Key]
        public T Id { get; set; }
    }
}
