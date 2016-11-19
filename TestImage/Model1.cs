namespace TestImage
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DNFContext : DbContext
    {
        public DNFContext()
            : base("name=DNFDataContext")
        {
        }

        public virtual DbSet<DNFItem> DNFItems { get; set; }
    }
}
