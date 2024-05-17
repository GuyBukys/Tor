using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tor.Domain.CategoryAggregate;
using Tor.Domain.CategoryAggregate.Enums;

namespace Tor.Infrastructure.Configurations;

public class CategoryConfigurations : IEntityTypeConfiguration<Category>
{
    private readonly List<Category> _defaultCategories =
    [
        new(new Guid("208c5853-b795-4b54-95aa-0dfb615a4843"))
        {
            Type = CategoryType.Barbershop,
            DisplayName = "מספרה",
        },
        new(new Guid("48ca5438-3987-4cbf-ba8a-b736a17bfc9a"))
        {
            Type = CategoryType.NailSalon,
            DisplayName = "ציפורניים",
        },
        new(new Guid("50f9c731-29e5-490f-86a3-e4fff61b3160"))
        {
            Type = CategoryType.HairSalon,
            DisplayName = "שיער",
        },
        new(new Guid("48258ac6-200c-454b-88d1-3ca708b50ed0"))
        {
            Type = CategoryType.CosmeticsAndBeauty,
            DisplayName = "יופי, קוסמטיקה וטיפוח אישי",
        },
        new(new Guid("7cd54324-407a-4876-beb1-f3d7d68d10a2"))
        {
            Type = CategoryType.Massage,
            DisplayName = "עיסוי ורפואה משלימה",
        },
        new(new Guid("abe0f760-2cb1-426e-96aa-a202ed13a6df"))
        {
            Type = CategoryType.EyebrowsAndLashes,
            DisplayName = "גבות וריסים",
        },
        new(new Guid("960de054-89d7-4f66-849d-fc129137e0f0"))
        {
            Type = CategoryType.Piercing,
            DisplayName = "פירסינג",
        },
        new(new Guid("b733e0c7-a4f0-44e7-92d3-8c5af8799b7c"))
        {
            Type = CategoryType.Makeup,
            DisplayName = "איפור",
        },
        new(new Guid("cceddb81-ba91-45fb-b1c5-871ac3bb5c93"))
        {
            Type = CategoryType.PersonalTrainer,
            DisplayName = "אימונים אישיים",
        },
        new(new Guid("d3c9378f-df01-465c-a6a1-fbfbadac4880"))
        {
            Type = CategoryType.PetServices,
            DisplayName = "חיות מחמד",
        },
        new(new Guid("761145fd-46d6-4902-9cd6-7259009a9fd8"))
        {
            Type = CategoryType.PrivateTutor,
            DisplayName = "שיעורים פרטיים",
        },
        new(new Guid("f2e123ed-59a1-4893-99e9-7d11a6691186"))
        {
            Type = CategoryType.Other,
            DisplayName = "אחר",
        },
    ];

    public void Configure(EntityTypeBuilder<Category> builder)
    {
        ConfigureCategoriesTable(builder);
    }

    private void ConfigureCategoriesTable(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.Type)
            .IsRequired();

        builder.HasMany(x => x.Businesses)
            .WithMany(x => x.Categories);

        builder.OwnsOne(x => x.Image);

        builder.HasData(_defaultCategories);
    }
}
