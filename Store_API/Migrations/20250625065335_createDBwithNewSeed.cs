using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Store_API.Migrations
{
    /// <inheritdoc />
    public partial class createDBwithNewSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dob = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Provider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Consumed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnqueueTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BasketHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Star = table.Column<double>(type: "float", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockHolds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PaymentIntentId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockHolds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Technologies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Technologies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuperAdminOnly = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Baskets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Baskets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Baskets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DeliveryFee = table.Column<double>(type: "float", nullable: false),
                    GrandTotal = table.Column<double>(type: "float", nullable: false),
                    ClientSecret = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    District = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ward = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Promotions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PercentageDiscount = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Promotions_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Promotions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockHoldItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StockHoldId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockHoldItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockHoldItem_StockHolds_StockHoldId",
                        column: x => x.StockHoldId,
                        principalTable: "StockHolds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserWarehouses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWarehouses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserWarehouses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserWarehouses_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductPrice = table.Column<double>(type: "float", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SubTotal = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GHNOrderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingStatus = table.Column<int>(type: "int", nullable: false),
                    ToName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToWard = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToDistrict = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToProvince = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<int>(type: "int", nullable: false),
                    Length = table.Column<int>(type: "int", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    CODAmount = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExtraName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTechnologies",
                columns: table => new
                {
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechnologyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTechnologies", x => new { x.ProductId, x.TechnologyId });
                    table.ForeignKey(
                        name: "FK_ProductTechnologies_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTechnologies_Technologies_TechnologyId",
                        column: x => x.TechnologyId,
                        principalTable: "Technologies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BasketItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BasketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BasketItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BasketItems_Baskets_BasketId",
                        column: x => x.BasketId,
                        principalTable: "Baskets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BasketItems_ProductDetails_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stocks_ProductDetails_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Stocks_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductDetailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransactions_ProductDetails_ProductDetailId",
                        column: x => x.ProductDetailId,
                        principalTable: "ProductDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockTransactions_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, null, "SuperAdmin", "SUPERADMIN" },
                    { 2, null, "Admin", "ADMIN" },
                    { 3, null, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Dob", "Email", "EmailConfirmed", "FullName", "ImageUrl", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "Provider", "PublicId", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 1, 0, "483d2597-6872-48e6-9a55-d1de6f919a2f", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "spadmin@example.com", true, "SuperAdmin", null, false, null, "SPADMIN@EXAMPLE.COM", "SPADMIN", "AQAAAAIAAYagAAAAELyT/CpZSO35p4BlUCfFQcU3ZVflgoZGE7fyfGjVS9XGqdzLto3lDTNbCneyyulvqw==", null, false, "System", null, "50a4578f-7962-483e-98d8-3fc7e7ec171d", false, "spadmin" },
                    { 2, 0, "2e133dff-0f1d-4ea6-98d5-fc81a327075d", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin1@example.com", true, "Admin1", null, false, null, "ADMIN1@EXAMPLE.COM", "ADMIN1", "AQAAAAIAAYagAAAAEHJVTAr07lSguH5SvsH2i4ntlhf2dhn0B2QfjfqTn7jFOTmencA3eTTgCNYGVWdu5A==", null, false, "System", null, "9dcc2a37-f082-4e41-95b7-33cbf1bb363e", false, "admin1" },
                    { 3, 0, "c49adc77-de99-46f7-bf98-83cd4b8289cc", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin2@example.com", true, "Admin2", null, false, null, "ADMIN2@EXAMPLE.COM", "ADMIN2", "AQAAAAIAAYagAAAAELTxO3vW2NunHcbWbHuZt9WGsWuQkxm8Guy3HLowhe0HCF74hI91v14+mxfNiscVVA==", null, false, "System", null, "50596dd1-7cf1-448c-b55a-d784f205b199", false, "admin2" },
                    { 4, 0, "368d3d40-5a58-4914-ad39-1ef489106595", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admi3n@example.com", true, "Admin3", null, false, null, "ADMIN3@EXAMPLE.COM", "ADMIN3", "AQAAAAIAAYagAAAAENskZunKI2sbMFKGQ7k13y0QpoAs3HfHb8yPDvc90hyJ+fUVTlo0E+RwKy+zWyg5Ow==", null, false, "System", null, "8e7fae9f-374d-400d-8ff0-6505dd4c939f", false, "admin3" }
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Country", "Name" },
                values: new object[,]
                {
                    { new Guid("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d"), "Taiwan", "Victor" },
                    { new Guid("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), "China", "Lining" },
                    { new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), "Japan", "Yonex" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), "Racket" },
                    { new Guid("8a0ef9d4-79bb-418f-9e12-8f5f6df62049"), "Clothes" },
                    { new Guid("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"), "Shoes" },
                    { new Guid("af0b3a7a-5898-43cf-8f98-d0c5712ec5f3"), "Others" },
                    { new Guid("c1dcf6b8-4c24-493c-a828-7b1e4cc26a6b"), "Items" }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "ProductDetailId", "ProductId", "Star", "UserId" },
                values: new object[,]
                {
                    { 1, new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), 4.0, 2 },
                    { 2, new Guid("e2c8ff1c-2db0-4a02-9a2a-7b8d05eeb6d4"), new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), 4.5, 2 },
                    { 3, new Guid("5f3c3a57-1f41-4e32-9c7a-12d4686dbf8b"), new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), 4.7999999999999998, 4 },
                    { 4, new Guid("6a4e5f76-3c84-4f4e-bb76-61768c5d3e7d"), new Guid("123e4567-e89b-12d3-a456-426614174000"), 4.5, 3 }
                });

            migrationBuilder.InsertData(
                table: "Technologies",
                columns: new[] { "Id", "Description", "ImageUrl", "Name", "PublicId" },
                values: new object[,]
                {
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d0"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735801837/tech10_myratn.jpg", "ENERGY BOOST CAP PLUS", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d1"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735801836/tech9_jfuxth.jpg", "ROTATIONAL GENARATOR SYSTEM", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d2"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech8_mqshpr.webp", "NEW Built-in T-Joint", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d3"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech7_qfd4za.webp", "AERO-BOX FRAME", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d4"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735801836/tech6_foheeo.webp", "ISOMETRIC ", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d5"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech5_axkpsh.webp", "DUAL OPTIMUM SYSTEM", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d6"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735800436/technologies/tech3_olpj1g.png", "ISOMETRIC PLUS", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d7"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735800436/technologies/tech3_olpj1g.png", "POCKETING BOOSTER", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d8"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735800437/technologies/tech2_nhiopm.jpg", "CONTROL-ASSIST BUMPER", null },
                    { new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), "", "https://res.cloudinary.com/duat1htay/image/upload/v1735800437/technologies/tech1_qoof2s.jpg", "ENHANCED ARCSABER FRAME", null }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Created", "IsSuperAdminOnly", "Location", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Ho Chi Minh City", "HCM Warehouse" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Thu Duc City", "Thu Duc Warehouse" },
                    { new Guid("33333333-3333-3333-3333-333333333333"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, "Vung Tau City", "HHT Warehouse" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 2, 4 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "Created", "Description", "ImageUrl", "Name", "PublicId" },
                values: new object[,]
                {
                    { new Guid("00112233-4455-6677-8899-aabbccddeeff"), new Guid("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Speed - Control - Attack", "https://res.cloudinary.com/duat1htay/image/upload/v1729153834/rweggufmgnga3zjklf2f.jpg", "Brave Sword 12 (Ver.55th 2024)", "rweggufmgnga3zjklf2f" },
                    { new Guid("123e4567-e89b-12d3-a456-426614174000"), new Guid("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "All England Champ (2021) Racket Chosen", "https://res.cloudinary.com/duat1htay/image/upload/v1729152504/bobdvzdutlsnhkgd3csa.webp", "Thuskter Ryuga Metalic (Ver.2023)", "bobdvzdutlsnhkgd3csa" },
                    { new Guid("2f8c6a10-5633-4b91-90a1-7c924df78e68"), new Guid("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), new Guid("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Speed - Jump - Dominate", "https://res.cloudinary.com/duat1htay/image/upload/v1729153621/dn25ivc2gpbcytdfqfim.webp", "Accelarate Advanced (Ver.2024)", "dn25ivc2gpbcytdfqfim" },
                    { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "King Double - Speed - Power", "https://res.cloudinary.com/duat1htay/image/upload/v1735830149/products/nanoflare 1000z/yj14npg3jorqi1dhbygd.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830150/products/nanoflare 1000z/ptbxakwyi6dtxsedhog4.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830151/products/nanoflare 1000z/rm4ymkkeupgo5jfuzrts.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830152/products/nanoflare 1000z/syrhneosnjsnoyuwwdte.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830153/products/nanoflare 1000z/f8hfmdfuhux7bvs4s1zs.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830154/products/nanoflare 1000z/tyv2w5kyqdonv5qib1rt.jpg", "Nanoflare 1000Z", "products/nanoflare 1000z/yj14npg3jorqi1dhbygd,products/nanoflare 1000z/ptbxakwyi6dtxsedhog4,products/nanoflare 1000z/rm4ymkkeupgo5jfuzrts,products/nanoflare 1000z/syrhneosnjsnoyuwwdte,products/nanoflare 1000z/f8hfmdfuhux7bvs4s1zs,products/nanoflare 1000z/tyv2w5kyqdonv5qib1rt" },
                    { new Guid("4d21b8e5-8a14-4b37-b84b-3d1c2e2e5f76"), new Guid("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), new Guid("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smooth - Jump - Reach the win", "https://res.cloudinary.com/duat1htay/image/upload/v1734926089/products/hxlh389m9vsug2zumawz.jpg", "Accelarate Booster (Ver.2022)", "products/hxlh389m9vsug2zumawz" },
                    { new Guid("550e8400-e29b-41d4-a716-446655440000"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Racket Choosing by The Olympic Champion (2020) Viktor Axelsen", "https://res.cloudinary.com/duat1htay/image/upload/v1729152726/ldpbvqnabfaq7o2uggia.webp", "Axtrox 100ZZ (Ver.Kurenai)", "ldpbvqnabfaq7o2uggia" },
                    { new Guid("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "The upgrade of Comfort Z Version", "https://res.cloudinary.com/duat1htay/image/upload/v1750748254/products/comfort z3/stpn0skt1oehm2onmyf9.jpg", "Comfort Z3", "bpjcwixbyweafni7t5sz" },
                    { new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Two Faces - Expolosive Attack - Solid Hard Defend", "https://res.cloudinary.com/duat1htay/image/upload/v1735828714/products/duora z strike %28ver.2017%29/he85nkkpfkdc6gh2w9ak.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828716/products/duora z strike %28ver.2017%29/ez7dx5lamzyjkwrs4zya.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828717/products/duora z strike %28ver.2017%29/zv47em4nf8cfex61lwh1.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828718/products/duora z strike %28ver.2017%29/jlj5w0sliaquoxfxziaf.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828719/products/duora z strike %28ver.2017%29/u8mj5igvuamhih3mgely.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828720/products/duora z strike %28ver.2017%29/qnelzoomlaqll2aikfbj.jpg", "Duora Z Strike (Ver.2017)", "products/duora z strike %28ver.2017%29/he85nkkpfkdc6gh2w9ak,products/duora z strike %28ver.2017%29/ez7dx5lamzyjkwrs4zya,products/duora z strike %28ver.2017%29/zv47em4nf8cfex61lwh1,products/duora z strike %28ver.2017%29/jlj5w0sliaquoxfxziaf,products/duora z strike %28ver.2017%29/u8mj5igvuamhih3mgely,products/duora z strike %28ver.2017%29/qnelzoomlaqll2aikfbj" },
                    { new Guid("7d9e6679-7425-40de-944b-e07fc1f90ae7"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Controls - Focus - Feel", "https://res.cloudinary.com/duat1htay/image/upload/v1735828775/products/arcsaber 11 pro %28ver.2021%29/ljra5olhsvaxrjptk0re.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828777/products/arcsaber 11 pro %28ver.2021%29/uskrx81pgum1grefnhcx.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828778/products/arcsaber 11 pro %28ver.2021%29/ewj0zhky5g7e5wvsi9sv.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828779/products/arcsaber 11 pro %28ver.2021%29/grokd1zbead4twuac0ly.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/wmnhk6gabpegyoq3lcpp.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/xkl3k09jjrixdaq8qypu.jpg", "ArcSaber 11 Pro (Ver.2021)", "arcsaber 11 pro %28ver.2021%29/ljra5olhsvaxrjptk0re,products/arcsaber 11 pro %28ver.2021%29/uskrx81pgum1grefnhcx,products/arcsaber 11 pro %28ver.2021%29/ewj0zhky5g7e5wvsi9sv,products/arcsaber 11 pro %28ver.2021%29/grokd1zbead4twuac0ly,products/arcsaber 11 pro %28ver.2021%29/wmnhk6gabpegyoq3lcpp,products/arcsaber 11 pro %28ver.2021%29/xkl3k09jjrixdaq8qypu" },
                    { new Guid("8a97f9a6-221d-4f5b-bc37-6e5cb7a979b6"), new Guid("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Blow White Attack", "https://res.cloudinary.com/duat1htay/image/upload/v1729153705/w9s0ruep5gxnelaunfgq.jpg", "Techtonic 9", "w9s0ruep5gxnelaunfgq" },
                    { new Guid("9b9f0b80-4f3d-11ec-81d3-0242ac130003"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "New Shape - New Tech - New Feelings", "https://res.cloudinary.com/duat1htay/image/upload/v1729152808/erovfedlbzb0xkzqglbj.jpg", "Axtrox 88D Pro (Ver.2024)", "erovfedlbzb0xkzqglbj" },
                    { new Guid("a2cf7e92-29fd-4d61-90b3-d3f2f8a7e9c6"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Smooth - Jump - Reach the win", "https://res.cloudinary.com/duat1htay/image/upload/v1729153525/bpjcwixbyweafni7t5sz.jpg", "Comfort Z", "bpjcwixbyweafni7t5sz" },
                    { new Guid("b3e2f5f0-7e44-4e06-b69e-8f87be0c30f7"), new Guid("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Speed - Control - Attack", "https://res.cloudinary.com/duat1htay/image/upload/v1729153808/cy4dqkjmsqakqxsqonl5.jpg", "Thuskter Falcon White (Ver. Limited TYZ)", "cy4dqkjmsqakqxsqonl5" },
                    { new Guid("cb3b0e7d-5ad3-4ec7-9b9a-4f06efb27c03"), new Guid("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chosen By World Champion 2021 - Loh Kean Yew", "https://res.cloudinary.com/duat1htay/image/upload/v1729153857/algdodmsmknzhilm9wds.webp", "Axforce 90", "algdodmsmknzhilm9wds" },
                    { new Guid("dd36bf61-fc77-4cfb-82e1-6b2ff6f9b1d4"), new Guid("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chosen by The World Champion (2014, 2015), The OLP Champion (2016) - Chen Long", "https://res.cloudinary.com/duat1htay/image/upload/v1729153773/cetkfwcafc8xliwnim9n.jpg", "Flame N55", "cetkfwcafc8xliwnim9n" },
                    { new Guid("e029d3c5-b6b3-4e31-bada-1e6b7d5af7c8"), new Guid("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern Technologies - Powerful Attack", "https://res.cloudinary.com/duat1htay/image/upload/v1734927477/products/axforce 100 %28kirin%29/czjdbrlre4jnbrhfabyi.jpg", "Axforce 100 (Ver.Kirin)", "products/axforce 100 %28kirin%29/czjdbrlre4jnbrhfabyi" },
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Attack - Dominate - Conquers", "https://res.cloudinary.com/duat1htay/image/upload/v1734927449/products/astrox 99 pro 2021/hgkgglo91lbmjhxby5h0.webp", "Astrox 99 Pro (Ver.2021)", "products/astrox 99 pro 2021/hgkgglo91lbmjhxby5h0" }
                });

            migrationBuilder.InsertData(
                table: "Promotions",
                columns: new[] { "Id", "BrandId", "CategoryId", "EndDate", "PercentageDiscount", "StartDate" },
                values: new object[] { new Guid("3e5d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new Guid("e1798a79-327e-4851-9028-b1c9b2e82ec6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), 15.0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "UserWarehouses",
                columns: new[] { "Id", "UserId", "WarehouseId" },
                values: new object[,]
                {
                    { new Guid("8df45413-2c4f-408a-9953-d3ac46038246"), 2, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("f172ede6-eb39-4e29-936b-5413d6106c9b"), 3, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("f75b375e-9397-45d0-b39f-853b90036da5"), 4, new Guid("33333333-3333-3333-3333-333333333333") }
                });

            migrationBuilder.InsertData(
                table: "ProductDetails",
                columns: new[] { "Id", "Color", "ExtraName", "Price", "ProductId", "Status" },
                values: new object[,]
                {
                    { new Guid("0c462b3e-61c9-4e34-bab2-7d82c4c5e8e1"), "#880808", "Ver.Tiger Max", 3850000.0, new Guid("cb3b0e7d-5ad3-4ec7-9b9a-4f06efb27c03"), 1 },
                    { new Guid("13c87621-8b94-4515-90d4-35f5f8a4b23e"), "#fff", "", 3300000.0, new Guid("8a97f9a6-221d-4f5b-bc37-6e5cb7a979b6"), 1 },
                    { new Guid("2e8c3bc1-23e5-4df9-822c-2f7d9dd4f5f3"), "#FDDA0D", "The Yellow Flash", 4350000.0, new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 1 },
                    { new Guid("2fa0f68b-efc9-4a92-b4c3-8f62c4d8e5a1"), "#880808", "Chen Long Edition (Rio 2016)", 5000000.0, new Guid("dd36bf61-fc77-4cfb-82e1-6b2ff6f9b1d4"), 1 },
                    { new Guid("3b6e123a-f75c-4de5-86a5-d2b5e8b6c9d2"), "#fff", "", 2700000.0, new Guid("b3e2f5f0-7e44-4e06-b69e-8f87be0c30f7"), 1 },
                    { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), "#880808", "Red Tiger", 4200000.0, new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), 1 },
                    { new Guid("41d3f8b7-c1e2-456f-a9c8-72b3d2e5f9a4"), "#FF69B4", "Ver.Pink", 2800000.0, new Guid("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"), 1 },
                    { new Guid("51fa47d3-9baf-4e71-bdd8-6206533a126c"), "#880808", "Limited Edition (2025)", 15000000.0, new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 1 },
                    { new Guid("5d479eab-b8c6-4df1-99f7-df3a7b2e6f87"), "#333", "", 2200000.0, new Guid("a2cf7e92-29fd-4d61-90b3-d3f2f8a7e9c6"), 1 },
                    { new Guid("5f3c3a57-1f41-4e32-9c7a-12d4686dbf8b"), "#fff", "Chou Tien Chen Signature!", 4150000.0, new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), 1 },
                    { new Guid("63e7c5d2-9b4f-4f38-b7d1-85f9a3e2c4d8"), "#fff", "", 1700000.0, new Guid("2f8c6a10-5633-4b91-90a1-7c924df78e68"), 1 },
                    { new Guid("6a4e5f76-3c84-4f4e-bb76-61768c5d3e7d"), "#FF5733", "Lee Zii Ja Chosen", 3600000.0, new Guid("123e4567-e89b-12d3-a456-426614174000"), 1 },
                    { new Guid("7a3f4036-942f-4f8a-a823-0f3c5c791e20"), "#880808", "", 4250000.0, new Guid("7d9e6679-7425-40de-944b-e07fc1f90ae7"), 1 },
                    { new Guid("7f1b9d38-3b5d-474f-832b-85c7c5d2a9b4"), "#880808", "Ver.Red", 2850000.0, new Guid("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"), 1 },
                    { new Guid("8b7f69d4-459c-45c8-bf38-9f5b214a9d7e"), "#4169E1", "Ver.Dragon Max", 3880000.0, new Guid("cb3b0e7d-5ad3-4ec7-9b9a-4f06efb27c03"), 1 },
                    { new Guid("9d5a72c4-1f87-4b3a-b7e8-d4c5f9a2e3b6"), "#4169E1", "", 1500000.0, new Guid("4d21b8e5-8a14-4b37-b84b-3d1c2e2e5f76"), 1 },
                    { new Guid("a2e987b6-fdbc-4d9a-a86b-6f9cb4e7f236"), "#880808", "Ver.Kirin", 4250000.0, new Guid("e029d3c5-b6b3-4e31-bada-1e6b7d5af7c8"), 1 },
                    { new Guid("b9f376e1-6a5d-4b34-9a1c-3f9e8a7b2d5c"), "#4169E1", "Ver.Blue", 2830000.0, new Guid("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"), 1 },
                    { new Guid("c9b74e77-dc8b-4c4e-96c9-d6b2e8adf2cf"), "#4169E1", "Navy Blue", 4500000.0, new Guid("550e8400-e29b-41d4-a716-446655440000"), 1 },
                    { new Guid("d55b3f65-68b2-4c5e-85ae-8f2a3bfb6b8f"), "#7393B3", "", 4200000.0, new Guid("9b9f0b80-4f3d-11ec-81d3-0242ac130003"), 1 },
                    { new Guid("e2c8ff1c-2db0-4a02-9a2a-7b8d05eeb6d4"), "#fff", "White Tiger", 4300000.0, new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), 1 },
                    { new Guid("e4d849aa-7683-47e5-9f45-2e4894a3ddf4"), "#4169E1", "", 2800000.0, new Guid("00112233-4455-6677-8899-aabbccddeeff"), 1 },
                    { new Guid("f01d30c9-b2a1-4d37-95b4-018cbacfd6ef"), "#880808", "Ver.Kurenai", 4450000.0, new Guid("550e8400-e29b-41d4-a716-446655440000"), 1 }
                });

            migrationBuilder.InsertData(
                table: "ProductTechnologies",
                columns: new[] { "ProductId", "TechnologyId", "Created" },
                values: new object[,]
                {
                    { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d5"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6724) },
                    { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d7"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6722) },
                    { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d8"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6720) },
                    { new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6706) },
                    { new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d2"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6697) },
                    { new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d3"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6695) },
                    { new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d4"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6691) },
                    { new Guid("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d5"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6575) },
                    { new Guid("7d9e6679-7425-40de-944b-e07fc1f90ae7"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d0"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6704) },
                    { new Guid("7d9e6679-7425-40de-944b-e07fc1f90ae7"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d1"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6702) },
                    { new Guid("7d9e6679-7425-40de-944b-e07fc1f90ae7"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6699) },
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d6"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6573) },
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d7"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6571) },
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d8"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6569) },
                    { new Guid("f47ac10b-58cc-4372-a567-0e02b2c3d479"), new Guid("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), new DateTime(2025, 6, 25, 6, 53, 33, 807, DateTimeKind.Utc).AddTicks(6561) }
                });

            migrationBuilder.InsertData(
                table: "StockTransactions",
                columns: new[] { "Id", "Created", "ProductDetailId", "Quantity", "TransactionType", "WarehouseId" },
                values: new object[,]
                {
                    { new Guid("88888888-1111-3333-2222-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("e2c8ff1c-2db0-4a02-9a2a-7b8d05eeb6d4"), 15, 1, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("99999333-befa-3333-4545-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("5f3c3a57-1f41-4e32-9c7a-12d4686dbf8b"), 18, 1, new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("99999875-abcd-aaaa-8217-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("51fa47d3-9baf-4e71-bdd8-6206533a126c"), 4, 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999875-abcd-aaaa-8217-111111111988"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("51fa47d3-9baf-4e71-bdd8-6206533a126c"), 1, 0, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999875-abcd-aaaa-8217-222111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("51fa47d3-9baf-4e71-bdd8-6206533a126c"), 2, 1, new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("99999875-abef-3333-8217-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("f01d30c9-b2a1-4d37-95b4-018cbacfd6ef"), 20, 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999999-1111-3333-2222-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 8, 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999999-1111-3333-4545-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 3, 1, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999999-2222-3333-4545-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 1, 0, new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "ProductDetailId", "Quantity", "Updated", "WarehouseId" },
                values: new object[,]
                {
                    { new Guid("99999999-1111-1111-1111-111111111111"), new Guid("3fa85f64-5717-4562-b3fc-2c963f66afa6"), 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999999-1111-1111-8888-111111111111"), new Guid("e2c8ff1c-2db0-4a02-9a2a-7b8d05eeb6d4"), 15, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("99999999-1111-5555-8888-111111111111"), new Guid("51fa47d3-9baf-4e71-bdd8-6206533a126c"), 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("99999999-1111-5555-8888-111111111158"), new Guid("51fa47d3-9baf-4e71-bdd8-6206533a126c"), 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("22222222-2222-2222-2222-222222222222") },
                    { new Guid("99999999-1111-7777-1111-111111111111"), new Guid("5f3c3a57-1f41-4e32-9c7a-12d4686dbf8b"), 18, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("33333333-3333-3333-3333-333333333333") },
                    { new Guid("99999999-6666-1111-1111-111111111111"), new Guid("f01d30c9-b2a1-4d37-95b4-018cbacfd6ef"), 20, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_BasketId",
                table: "BasketItems",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_ProductDetailId",
                table: "BasketItems",
                column: "ProductDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_UserId",
                table: "Baskets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true,
                filter: "[InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "[OutboxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductId",
                table: "ProductDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTechnologies_TechnologyId",
                table: "ProductTechnologies",
                column: "TechnologyId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_BrandId",
                table: "Promotions",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_CategoryId",
                table: "Promotions",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingOrders_OrderId",
                table: "ShippingOrders",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockHoldItem_StockHoldId",
                table: "StockHoldItem",
                column: "StockHoldId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductDetailId",
                table: "Stocks",
                column: "ProductDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_WarehouseId",
                table: "Stocks",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_ProductDetailId",
                table: "StockTransactions",
                column: "ProductDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_WarehouseId",
                table: "StockTransactions",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWarehouses_UserId",
                table: "UserWarehouses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserWarehouses_WarehouseId",
                table: "UserWarehouses",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BasketItems");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "OutboxState");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "ProductTechnologies");

            migrationBuilder.DropTable(
                name: "Promotions");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "ShippingOrders");

            migrationBuilder.DropTable(
                name: "StockHoldItem");

            migrationBuilder.DropTable(
                name: "Stocks");

            migrationBuilder.DropTable(
                name: "StockTransactions");

            migrationBuilder.DropTable(
                name: "UserAddresses");

            migrationBuilder.DropTable(
                name: "UserWarehouses");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Baskets");

            migrationBuilder.DropTable(
                name: "Technologies");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "StockHolds");

            migrationBuilder.DropTable(
                name: "ProductDetails");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
