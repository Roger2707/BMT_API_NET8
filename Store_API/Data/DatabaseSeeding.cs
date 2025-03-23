using Microsoft.Data.SqlClient;
using Store_API.Models;

namespace Store_API.Data
{
    public static class DatabaseSeeding
    {
        public static async void SeedData(StoreContext context)
        {
            // 1. Category
            if(!context.Categories.Any())
            {
                await context.AddRangeAsync(
                    new Category { Name = "Racket" },
                    new Category { Name = "Shoes" },
                    new Category { Name = "Clothes" },
                    new Category { Name = "Items" },
                    new Category { Name = "Others" }
                );
            }

            // 2. Brand
            if (!context.Brands.Any())
            {
                await context.AddRangeAsync(
                    new Brand { Name = "Yonex", Country = "Japan" },
                    new Brand { Name = "Victor", Country = "Taiwan" },
                    new Brand { Name = "Lining", Country = "China" }
                );
            }

            // 3. Seed Products
            if (!context.Products.Any())
            {
                await context.AddRangeAsync(
                    new Product
                    {
                        Name = "Astrox 99 Pro (Ver.2021)",
                        Description = "Attack - Dominate - Conquers",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734927449/products/astrox 99 pro 2021/hgkgglo91lbmjhxby5h0.webp",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Duora Z Strike (Ver.2017)",
                        Description = "Two Faces - Expolosive Attack - Solid Hard Defend",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735828714/products/duora z strike %28ver.2017%29/he85nkkpfkdc6gh2w9ak.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828716/products/duora z strike %28ver.2017%29/ez7dx5lamzyjkwrs4zya.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828717/products/duora z strike %28ver.2017%29/zv47em4nf8cfex61lwh1.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828718/products/duora z strike %28ver.2017%29/jlj5w0sliaquoxfxziaf.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828719/products/duora z strike %28ver.2017%29/u8mj5igvuamhih3mgely.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828720/products/duora z strike %28ver.2017%29/qnelzoomlaqll2aikfbj.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "ArcSaber 11 Pro (Ver.2021)",

                        Description = "Controls - Focus - Feel",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735828775/products/arcsaber 11 pro %28ver.2021%29/ljra5olhsvaxrjptk0re.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828777/products/arcsaber 11 pro %28ver.2021%29/uskrx81pgum1grefnhcx.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828778/products/arcsaber 11 pro %28ver.2021%29/ewj0zhky5g7e5wvsi9sv.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828779/products/arcsaber 11 pro %28ver.2021%29/grokd1zbead4twuac0ly.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/wmnhk6gabpegyoq3lcpp.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/xkl3k09jjrixdaq8qypu.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "Axtrox 100ZZ (Ver.Kurenai)",
                        Description = "Racket Choosing by The Olympic Champion (2020) Viktor Axelsen",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152726/ldpbvqnabfaq7o2uggia.webp",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "Nanoflare 1000Z",
                        Description = "King Double - Speed - Power",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735830149/products/nanoflare 1000z/yj14npg3jorqi1dhbygd.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830150/products/nanoflare 1000z/ptbxakwyi6dtxsedhog4.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830151/products/nanoflare 1000z/rm4ymkkeupgo5jfuzrts.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830152/products/nanoflare 1000z/syrhneosnjsnoyuwwdte.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830153/products/nanoflare 1000z/f8hfmdfuhux7bvs4s1zs.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830154/products/nanoflare 1000z/tyv2w5kyqdonv5qib1rt.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Thuskter Ryuga Metalic (Ver.2023)",
                        Description = "All England Champ (2021) Racket Chosen",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152504/bobdvzdutlsnhkgd3csa.webp",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 2
                    },
                    new Product
                    {
                        Name = "Axtrox 88D Pro (Ver.2024)",
                        Description = "New Shape - New Tech - New Feelings",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152808/erovfedlbzb0xkzqglbj.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Brave Sword 12 (Ver.55th 2024)",
                        Description = "Speed - Control - Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153834/rweggufmgnga3zjklf2f.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 2
                    },
                    new Product
                    {
                        Name = "Thuskter Falcon White (Ver. Limited TYZ)",
                        Description = "Speed - Control - Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153808/cy4dqkjmsqakqxsqonl5.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 2
                    },
                    new Product
                    {
                        Name = "Axforce 90",
                        Description = "Chosen By World Champion 2021 - Loh Kean Yew",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153857/algdodmsmknzhilm9wds.webp",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Axforce 100 (Ver.Kirin)",
                        Description = "Modern Technologies - Powerful Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734927477/products/axforce 100 %28kirin%29/czjdbrlre4jnbrhfabyi.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Techtonic 9",
                        Description = "Blow White Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153705/w9s0ruep5gxnelaunfgq.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Flame N55",
                        Description = "Chosen by The World Champion (2014, 2015), The OLP Champion (2016) - Chen Long",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153773/cetkfwcafc8xliwnim9n.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Comfort Z",
                        Description = "Smooth - Jump - Reach the win",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153525/bpjcwixbyweafni7t5sz.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Comfort Z3",
                        Description = "The upgrade of Comfort Z Version",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153525/bpjcwixbyweafni7t5sz.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Accelarate Booster (Ver.2022)",
                        Description = "Smooth - Jump - Reach the win",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734926089/products/hxlh389m9vsug2zumawz.jpg",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Accelarate Advanced (Ver.2024)",
                        Description = "Speed - Jump - Dominate",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153621/dn25ivc2gpbcytdfqfim.webp",
                        ProductStatus = 1,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 3
                    }
                );
            }

            // 4. Seed Technology
            if (!context.Technologies.Any())
            {
                await context.AddRangeAsync(
                    new Technology { Name = "ENHANCED ARCSABER FRAME", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800437/technologies/tech1_qoof2s.jpg" },
                    new Technology { Name = "CONTROL-ASSIST BUMPER", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800437/technologies/tech2_nhiopm.jpg" },
                    new Technology { Name = "POCKETING BOOSTER", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800436/technologies/tech3_olpj1g.png" },
                    new Technology { Name = "ISOMETRIC PLUS", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800436/technologies/tech3_olpj1g.png" },
                    new Technology { Name = "DUAL OPTIMUM SYSTEM", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech5_axkpsh.webp" },
                    new Technology { Name = "ISOMETRIC ", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801836/tech6_foheeo.webp" },
                    new Technology { Name = "AERO-BOX FRAME", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech7_qfd4za.webp" },
                    new Technology { Name = "NEW Built-in T-Joint", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech8_mqshpr.webp" },
                    new Technology { Name = "ROTATIONAL GENARATOR SYSTEM", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801836/tech9_jfuxth.jpg" },
                    new Technology { Name = "ENERGY BOOST CAP PLUS", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801837/tech10_myratn.jpg" }
                );
            }

            // 5. Seed Extra Prop Products
            if (!context.ProductColors.Any())
            {
                await context.AddRangeAsync(
                    // 99 Pro
                    new ProductColor { ProductId = 1, Price = 4200000, QuantityInStock = 10, Color = "#880808", ExtraName = "Red Tiger" },
                    new ProductColor { ProductId = 1, Price = 4300000, QuantityInStock = 10, Color = "#fff", ExtraName = "White Tiger" },
                    // Z-strike
                    new ProductColor { ProductId = 2, Price = 4150000, QuantityInStock = 3, Color = "#fff", ExtraName = "Chou Tien Chen Signature!" },
                    // Arc 11 Pro
                    new ProductColor { ProductId = 3, Price = 4250000, QuantityInStock = 8, Color = "#880808" },
                    // 100ZZ
                    new ProductColor { ProductId = 4, Price = 4500000, QuantityInStock = 18, Color = "#4169E1", ExtraName = "Navy Blue" },
                    new ProductColor { ProductId = 4, Price = 4450000, QuantityInStock = 17, Color = "#880808", ExtraName = "Ver.Kurenai" },
                    // 1000Z
                    new ProductColor { ProductId = 5, Price = 4350000, QuantityInStock = 15, Color = "#FDDA0D", ExtraName = "The Yellow Flash" },
                    new ProductColor { ProductId = 5, Price = 15000000, QuantityInStock = 5, Color = "#880808", ExtraName = "Limited Edition (2025)" },
                    // Ryuga Metalic
                    new ProductColor { ProductId = 6, Price = 3600000, QuantityInStock = 10, Color = "#FF5733", ExtraName = "Lee Zii Ja Chosen" },
                    // 88D Pro
                    new ProductColor { ProductId = 7, Price = 4200000, QuantityInStock = 12, Color = "#7393B3" },
                    // BS12
                    new ProductColor { ProductId = 8, Price = 2800000, QuantityInStock = 5, Color = "#4169E1" },
                    // Thuster Falcon TTY
                    new ProductColor { ProductId = 9, Price = 2700000, QuantityInStock = 10, Color = "#fff" },
                    // Ax90
                    new ProductColor { ProductId = 10, Price = 3850000, QuantityInStock = 15, Color = "#880808", ExtraName = "Ver.Tiger Max" },
                    new ProductColor { ProductId = 10, Price = 3880000, QuantityInStock = 18, Color = "#4169E1", ExtraName = "Ver.Dragon Max" },
                    // Ax100
                    new ProductColor { ProductId = 11, Price = 4250000, QuantityInStock = 8, Color = "#880808", ExtraName = "Ver.Kirin" },
                    // Tectonic 9
                    new ProductColor { ProductId = 12, Price = 3300000, QuantityInStock = 20, Color = "#fff" },
                    // Flame N55
                    new ProductColor { ProductId = 13, Price = 5000000, QuantityInStock = 1, Color = "#880808", ExtraName = "Chen Long Edition (Rio 2016)" },
                    // Comfort Z
                    new ProductColor { ProductId = 14, Price = 2200000, QuantityInStock = 9, Color = "#333" },
                    // Comfort Z3
                    new ProductColor { ProductId = 15, Price = 2850000, QuantityInStock = 12, Color = "#880808", ExtraName = "Ver.Red" },
                    new ProductColor { ProductId = 15, Price = 2830000, QuantityInStock = 12, Color = "#4169E1", ExtraName = "Ver.Blue" },
                    new ProductColor { ProductId = 15, Price = 2800000, QuantityInStock = 12, Color = "#FF69B4", ExtraName = "Ver.Pink" },
                    // Victor
                    new ProductColor { ProductId = 16, Price = 1500000, QuantityInStock = 6, Color = "#4169E1" },
                    new ProductColor { ProductId = 17, Price = 1700000, QuantityInStock = 9, Color = "#fff" }

                );
            }

            // 6. Seed Promotions
            if (!context.Promotions.Any())
            {
                await context.AddRangeAsync(
                    new Promotion { CategoryId = 1, BrandId = 1, Start = DateTime.Now, End = DateTime.MaxValue, PercentageDiscount = 15 }
                );
            }

            context.SaveChanges();
        }

        public static void SeedData(string connectionString)
        {
            SeedProductTechnologyData(connectionString);
        }

        private static void SeedProductTechnologyData(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string checkQuery = "SELECT COUNT(*) FROM ProductTechnology";
                using (var checkCommand = new SqlCommand(checkQuery, connection))
                {
                    var count = (int)checkCommand.ExecuteScalar();
                    if (count > 0)
                        return;
                }

                string seedQuery = @"
                                    INSERT INTO ProductTechnology (ProductsId, TechnologiesId) 
                                    VALUES 
                                    (1,1), 
                                    (1,2),
                                    (1,3),
                                    (1,4),
                                    (2,5),
                                    (2,6),
                                    (2,7),
                                    (2,8),
                                    (3,1),
                                    (3,9),
                                    (3,10),
                                    (5, 1),
                                    (5, 2),
                                    (5, 3),
                                    (5, 5)
                                    ";

                using (var seedCommand = new SqlCommand(seedQuery, connection))
                {
                    seedCommand.ExecuteNonQuery();
                }
            }
        }
    }
}
