using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace NomadicBook.Models.db
{
    public partial class NomadicBookContext : DbContext
    {
        public NomadicBookContext()
        {
        }

        public NomadicBookContext(DbContextOptions<NomadicBookContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Addresses { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BookCategory> BookCategories { get; set; }
        public virtual DbSet<BookPhoto> BookPhotoes { get; set; }
        public virtual DbSet<ConvenienceStore> ConvenienceStores { get; set; }
        public virtual DbSet<Imailbox> Imailboxes { get; set; }
        public virtual DbSet<Isbn> Isbns { get; set; }
        public virtual DbSet<Notify> Notifies { get; set; }
        public virtual DbSet<RoomMessage> RoomMessages { get; set; }
        public virtual DbSet<SeekRecord> SeekRecords { get; set; }
        public virtual DbSet<UserData> UserDatas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasKey(e => new { e.City, e.Area, e.Road });

                entity.ToTable("Address");

                entity.Property(e => e.City)
                    .HasMaxLength(3)
                    .IsFixedLength(true);

                entity.Property(e => e.Area).HasMaxLength(4);

                entity.Property(e => e.Road).HasMaxLength(10);
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.BookName)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.CellphoneNumber)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Condition).HasMaxLength(100);

                entity.Property(e => e.Experience).HasMaxLength(2000);

                entity.Property(e => e.ExperienceDay).HasColumnType("date");

                entity.Property(e => e.FaceTradeArea).HasMaxLength(10);

                entity.Property(e => e.FaceTradeCity)
                    .HasMaxLength(3)
                    .IsFixedLength(true);

                entity.Property(e => e.FaceTradeDetail).HasMaxLength(50);

                entity.Property(e => e.FaceTradePath).HasMaxLength(50);

                entity.Property(e => e.FaceTradeRoad).HasMaxLength(10);

                entity.Property(e => e.HomeAddress).HasMaxLength(200);

                entity.Property(e => e.Isbn)
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("ISBN");

                entity.Property(e => e.MailBoxAddress).HasMaxLength(200);

                entity.Property(e => e.MailBoxName).HasMaxLength(20);

                entity.Property(e => e.PublishDate).HasColumnType("date");

                entity.Property(e => e.PublishingHouse)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.ReleaseDate).HasColumnType("date");

                entity.Property(e => e.StoreAddress).HasMaxLength(200);

                entity.Property(e => e.StoreName).HasMaxLength(6);

                entity.Property(e => e.TrueName)
                    .IsRequired()
                    .HasMaxLength(15);
            });

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.ToTable("BookCategory");

                entity.Property(e => e.CategoryId)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.BigCategory)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.DetailCategory)
                    .IsRequired()
                    .HasMaxLength(25);
            });

            modelBuilder.Entity<BookPhoto>(entity =>
            {
                entity.ToTable("BookPhoto");

                entity.Property(e => e.BookPhoto1)
                    .IsRequired()
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("BookPhoto");
            });

            modelBuilder.Entity<ConvenienceStore>(entity =>
            {
                entity.HasKey(e => e.ShopId);

                entity.ToTable("ConvenienceStore");

                entity.Property(e => e.ShopId).ValueGeneratedNever();

                entity.Property(e => e.ShopAddress)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ShopArea)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.ShopCity)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsFixedLength(true);

                entity.Property(e => e.ShopName)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<Imailbox>(entity =>
            {
                entity.HasKey(e => e.MailboxId);

                entity.ToTable("IMailbox");

                entity.Property(e => e.MailboxAddress)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.MailboxArea)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.MailboxCity)
                    .IsRequired()
                    .HasMaxLength(3)
                    .IsFixedLength(true);

                entity.Property(e => e.MailboxName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<Isbn>(entity =>
            {
                entity.HasKey(e => e.Isbn1);

                entity.ToTable("ISBN");

                entity.Property(e => e.Isbn1)
                    .HasMaxLength(13)
                    .IsUnicode(false)
                    .HasColumnName("ISBN");

                entity.Property(e => e.Author)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.BookName)
                    .IsRequired()
                    .HasMaxLength(400);

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Introduction).IsRequired();

                entity.Property(e => e.PublishDate)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.PublishingHouse)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Notify>(entity =>
            {
                entity.ToTable("Notify");

                entity.Property(e => e.Notify1)
                    .IsRequired()
                    .HasMaxLength(600)
                    .HasColumnName("Notify");

                entity.Property(e => e.NotifyDate).HasColumnType("date");
            });

            modelBuilder.Entity<RoomMessage>(entity =>
            {
                entity.HasKey(e => e.MessageId);

                entity.ToTable("RoomMessage");

                entity.Property(e => e.Message)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(e => e.MessageTime).HasPrecision(0);
            });

            modelBuilder.Entity<SeekRecord>(entity =>
            {
                entity.HasKey(e => e.SeekId);

                entity.ToTable("SeekRecord");

                entity.Property(e => e.SeekCellphone)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SeekDate).HasColumnType("date");

                entity.Property(e => e.SeekName).HasMaxLength(15);

                entity.Property(e => e.SeekToAddress)
                    .IsRequired()
                    .HasMaxLength(300);

                entity.Property(e => e.SeekToName).HasMaxLength(50);

                entity.Property(e => e.SeekedCellphone)
                    .IsRequired()
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.SeekedDate).HasColumnType("date");

                entity.Property(e => e.SeekedName)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.SeekedToAddress).HasMaxLength(300);

                entity.Property(e => e.SeekedToName).HasMaxLength(50);
            });

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("UserData");

                entity.Property(e => e.CellphoneNumber)
                    .HasMaxLength(15)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.FaceTradeArea).HasMaxLength(10);

                entity.Property(e => e.FaceTradeCity)
                    .HasMaxLength(3)
                    .IsFixedLength(true);

                entity.Property(e => e.FaceTradeDetail).HasMaxLength(50);

                entity.Property(e => e.FaceTradePath).HasMaxLength(50);

                entity.Property(e => e.FaceTradeRoad).HasMaxLength(10);

                entity.Property(e => e.HomeAddress).HasMaxLength(200);

                entity.Property(e => e.MailBoxAddress).HasMaxLength(200);

                entity.Property(e => e.MailBoxName).HasMaxLength(20);

                entity.Property(e => e.NickName)
                    .IsRequired()
                    .HasMaxLength(8);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.SelfIntroduction)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.StoreAddress).HasMaxLength(200);

                entity.Property(e => e.StoreName).HasMaxLength(6);

                entity.Property(e => e.TrueName).HasMaxLength(15);

                entity.Property(e => e.UserPhoto)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
