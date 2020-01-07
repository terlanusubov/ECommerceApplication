using DahlizApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DahlizApp.Data
{
    public class DahlizDb : IdentityDbContext<User>
    {
        public DahlizDb(DbContextOptions option) : base(option) { }
        public virtual DbSet<Noti> Noti { get; set; }
        public virtual DbSet<NotiLanguage> NotiLanguage { get; set; }

        public virtual DbSet<Shipping> Shippings { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryLanguage> CategoryLanguages { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<ColorLanguage> ColorLanguage { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductColor> ProductColors { get; set; }
        public virtual DbSet<ProductLanguage> ProductLanguages { get; set; }
        public virtual DbSet<ProductPhoto> ProductPhotos { get; set; }
        public virtual DbSet<ProductReview> ProductReviews { get; set; }
        public virtual DbSet<ProductSize> ProductSizes { get; set; }
        public virtual DbSet<Size> Sizes { get; set; }
        public virtual DbSet<Subcategory> Subcategories { get; set; }
        public virtual DbSet<Brand> Brands { get; set; }
        public virtual DbSet<SubcategoryLanguage> SubcategoryLanguages { get; set; }
        public virtual DbSet<Slide> Slides { get; set; }
        public virtual DbSet<Ad> Ads { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<ProductSizeCount> ProductSizeCounts { get; set; }
        public virtual DbSet<CategoryBrand> CategoryBrands { get; set; }
        public virtual DbSet<CardType> CardTypes { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        public virtual DbSet<Terms> Terms { get; set; }
        public virtual DbSet<TermsLanguage> TermsLanguages { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public DbSet<AboutLanguage> AboutLanguages { get; set; }
        public DbSet<About> Abouts { get; set; }
    }
}
