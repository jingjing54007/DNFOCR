namespace TestImage
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public class DNFItem
    {
        [Key]
        public Guid ItemId { set; get; }

        public string PageIndex { set; get; }

        public string Name { set; get; }

        public string Location { set; get; }

        public bool Verified { set; get; }
    }
}
