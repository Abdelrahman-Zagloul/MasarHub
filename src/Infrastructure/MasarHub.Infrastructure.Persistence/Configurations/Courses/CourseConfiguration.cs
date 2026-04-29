using MasarHub.Domain.Modules.Categories;
using MasarHub.Domain.Modules.Courses;
using MasarHub.Infrastructure.Persistence.Configurations.Base;
using MasarHub.Infrastructure.Persistence.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MasarHub.Infrastructure.Persistence.Configurations.Courses
{
    internal sealed class CourseConfiguration : SoftDeletableEntityConfiguration<Course>, IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            ConfigureSoftDelete(builder);

            builder.ToTable("Courses", "courses", tb =>
            {
                tb.HasCheckConstraint("CK_Courses_Price_NonNegative", "[Price] >= 0");
            });
            builder.Property(c => c.Title)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(c => c.Slug)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(c => c.Description)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(4000)
                   .IsRequired();

            builder.Property(c => c.Price)
                   .HasPrecision(18, 2)
                   .IsRequired();

            builder.Property(c => c.Language)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(c => c.Status)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(c => c.Level)
                   .HasConversion<string>()
                   .HasColumnType("nvarchar")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(c => c.ThumbnailUrl)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(2000)
                   .IsRequired(false);

            builder.Property(c => c.PublishedAt)
                   .IsRequired(false);

            builder.Property(c => c.ApprovedBy)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired(false);

            builder.Property(c => c.RejectedBy)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired(false);

            builder.Property(c => c.RejectionReason)
                   .HasColumnType("nvarchar")
                   .HasMaxLength(1000)
                   .IsRequired(false);

            builder.Property(c => c.InstructorId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();

            builder.Property(c => c.CategoryId)
                   .HasColumnType("uniqueidentifier")
                   .IsRequired();


            builder.HasOne<Category>()
                   .WithMany()
                   .HasForeignKey(c => c.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>()
                   .WithMany()
                   .HasForeignKey(c => c.InstructorId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasIndex(c => c.Slug)
                   .IsUnique();

            builder.HasIndex(c => c.InstructorId);
            builder.HasIndex(c => c.CategoryId);
            builder.HasIndex(c => c.Status);

            ConfigureLearningObjectives(builder);
            ConfigurePrerequisites(builder);
            ConfigureRequirements(builder);
        }

        private static void ConfigurePrerequisites(EntityTypeBuilder<Course> builder)
        {
            builder.OwnsMany(c => c.Prerequisites, prerequisites =>
            {
                prerequisites.ToTable("CoursePrerequisites", "courses");

                prerequisites.WithOwner().HasForeignKey("CourseId");

                prerequisites.Property<int>("Id");
                prerequisites.HasKey("CourseId", "Id");

                prerequisites.Property(p => p.Value)
                             .HasColumnType("nvarchar")
                             .HasMaxLength(500)
                             .IsRequired();

                prerequisites.HasIndex("CourseId", nameof(CoursePrerequisite.Value))
                             .IsUnique();
            });

            builder.Navigation(c => c.Prerequisites)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }

        private static void ConfigureRequirements(EntityTypeBuilder<Course> builder)
        {
            builder.OwnsMany(c => c.Requirements, requirements =>
            {
                requirements.ToTable("CourseRequirements", "courses");

                requirements.WithOwner().HasForeignKey("CourseId");

                requirements.Property<int>("Id");
                requirements.HasKey("CourseId", "Id");

                requirements.Property(r => r.Value)
                            .HasColumnType("nvarchar")
                            .HasMaxLength(500)
                            .IsRequired();

                requirements.HasIndex("CourseId", nameof(CourseRequirement.Value))
                            .IsUnique();
            });

            builder.Navigation(c => c.Requirements)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }

        private static void ConfigureLearningObjectives(EntityTypeBuilder<Course> builder)
        {
            builder.OwnsMany(c => c.LearningObjectives, learningObjectives =>
            {
                learningObjectives.ToTable("CourseLearningObjectives", "courses");

                learningObjectives.WithOwner().HasForeignKey("CourseId");

                learningObjectives.Property<int>("Id");
                learningObjectives.HasKey("CourseId", "Id");

                learningObjectives.Property(l => l.Value)
                                  .HasColumnType("nvarchar")
                                  .HasMaxLength(500)
                                  .IsRequired();

                learningObjectives.HasIndex("CourseId", nameof(CourseLearningObjective.Value))
                                  .IsUnique();
            });

            builder.Navigation(c => c.LearningObjectives)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
