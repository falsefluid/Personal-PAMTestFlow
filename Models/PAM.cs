using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PAMTestFlow
{
    public class TypeBlock
    {
        [Key]
        public int TypeID { get; set; }
        [Required]
        public string? TypeName { get; set; }
        public string? TypeDescription { get; set; }
        public int? NoRows { get; set; } // Number of user input rows
    }

    public class Template
    {
        [Key]
        public int TemplateID { get; set; } // Unique identifier
        [Required]
        public string? TemplateName { get; set; } // Name of the template
        public DateTime? CreatedDate { get; set; } // When the template was created
        public DateTime? UpdatedDate { get; set; } // Last updated time (optional)
        public string? WebsiteURL {get; set; } // URL of the website
    }

    public class BuildingBlock
    {
        [Key]
        public int BlockID { get; set; } // Unique identifier
        [Required]
        public string? BlockName { get; set; } // Name of the block
        public int TypeID { get; set; } // Foreign key to TypeBlock
        public int TemplateID { get; set; } // Foreign key to Template
        public int SequenceOrder { get; set; } // The order of the block in the template
        public int BlockStatus { get; set; } // Status of the block
        public int WaitInterval { get; set; } // Wait interval in seconds

        public Template? Template { get; set; } // Navigation property
        public TypeBlock? TypeBlock { get; set; } // Navigation property
        public ICollection<UserInputs>? UserInputs { get; set; } // Navigation property
    }

    public class UserInputs 
    {
        [Key]
        public int InputID { get; set; } // Unique identifier
        public int BlockID { get; set; } // Foreign key to BuildingBlock
        [Required]
        public string? UserInput { get; set; } // User input
        public BuildingBlock? BuildingBlock { get; set; } // Navigation property
        
    }

    public class PAMContext : DbContext
    {
        public PAMContext(DbContextOptions<PAMContext> options)
            : base(options)
        {
        }

        public required DbSet<TypeBlock> TypeBlocks { get; set; }
        public required DbSet<BuildingBlock> BuildingBlocks { get; set; }
        public required DbSet<Template> Templates { get; set; }
        public required DbSet<UserInputs> UserInputs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships
            modelBuilder.Entity<BuildingBlock>()
                .HasOne(bb => bb.Template)
                .WithMany()
                .HasForeignKey(bb => bb.TemplateID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete, if required

            modelBuilder.Entity<BuildingBlock>()
                .HasOne(bb => bb.TypeBlock)
                .WithMany()
                .HasForeignKey(bb => bb.TypeID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete, if required

            modelBuilder.Entity<UserInputs>()
                .HasOne(ui => ui.BuildingBlock)
                .WithMany(bb => bb.UserInputs)
                .HasForeignKey(ui => ui.BlockID)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete, if required
        }
    }
}
