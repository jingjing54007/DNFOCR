namespace TestImage
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;

    public class OCRContext : DbContext
    {
        // Your context has been configured to use a 'Model1' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'TestImage.Model1' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model1' 
        // connection string in the application configuration file.
        public OCRContext()
            : base("name=DicValue")
        {
        }
         
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {                       
            base.OnModelCreating(modelBuilder);
        }
        

        // Add a DbSet for each entity type that you want to include in your model. For more information 
        // on configuring and using a Code First model, see http://go.microsoft.com/fwlink/?LinkId=390109.

        public virtual DbSet<DicValue> Values { get; set; }
    }

    public class DicValue
    {
        [Key]
        public string HashText { set; get; }

        [Required]
        public string Location { get; set; }

        public string Value { set; get; }

        public bool Verified { set; get; }
    }
}