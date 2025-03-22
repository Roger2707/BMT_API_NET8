using Microsoft.Data.SqlClient;
using Store_API.Models;

namespace Store_API.Data
{
    public static class DatabaseSeeding
    {
        public static void SeedData(StoreContext context)
        {
            // 1. Category
            if(!context.Categories.Any())
            {
                context.AddRange(
                    new Category { Name = "Racket" },
                    new Category { Name = "Shoes" },
                    new Category { Name = "Clothes" },
                    new Category { Name = "Items" },
                    new Category { Name = "Others" }
                );
            }

            // 2. Brand
            if (!context.Categories.Any())
            {
                context.AddRange(
                    new Brand { Name = "Yonex", Country = "Japan" },
                    new Brand { Name = "Victor", Country = "Taiwan" },
                    new Brand { Name = "Lining", Country = "China" }
                );
            }

            // 3. Seed Products
            if (!context.Products.Any())
            {
                context.AddRange(
                    new Product
                    {
                        Name = "Astrox 99 Pro (Ver.2021)",
                        Price = 4100000,
                        Description = "Attack - Dominate - Conquers",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734927449/products/astrox 99 pro 2021/hgkgglo91lbmjhxby5h0.webp",
                        QuantityInStock = 15,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Duora Z Strike (Ver.2017)",
                        Price = 4200000,
                        Description = "Two Faces - Expolosive Attack - Solid Hard Defend",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735828714/products/duora z strike %28ver.2017%29/he85nkkpfkdc6gh2w9ak.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828716/products/duora z strike %28ver.2017%29/ez7dx5lamzyjkwrs4zya.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828717/products/duora z strike %28ver.2017%29/zv47em4nf8cfex61lwh1.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828718/products/duora z strike %28ver.2017%29/jlj5w0sliaquoxfxziaf.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828719/products/duora z strike %28ver.2017%29/u8mj5igvuamhih3mgely.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828720/products/duora z strike %28ver.2017%29/qnelzoomlaqll2aikfbj.jpg",
                        QuantityInStock = 10,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "ArcSaber 11 Pro (Ver.2021)",
                        Price = 4050000,
                        Description = "Controls - Focus - Feel",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735828775/products/arcsaber 11 pro %28ver.2021%29/ljra5olhsvaxrjptk0re.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828777/products/arcsaber 11 pro %28ver.2021%29/uskrx81pgum1grefnhcx.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828778/products/arcsaber 11 pro %28ver.2021%29/ewj0zhky5g7e5wvsi9sv.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828779/products/arcsaber 11 pro %28ver.2021%29/grokd1zbead4twuac0ly.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/wmnhk6gabpegyoq3lcpp.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/xkl3k09jjrixdaq8qypu.jpg",
                        QuantityInStock = 15,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "Axtrox 100ZZ (Ver.Kurenai)",
                        Price = 4450000,
                        Description = "Racket Choosing by The Olympic Champion (2020) Viktor Axelsen",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152726/ldpbvqnabfaq7o2uggia.webp",
                        QuantityInStock = 15,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "Nanoflare 1000Z",
                        Price = 4350000,
                        Description = "King Double - Speed - Power",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735830149/products/nanoflare 1000z/yj14npg3jorqi1dhbygd.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830150/products/nanoflare 1000z/ptbxakwyi6dtxsedhog4.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830151/products/nanoflare 1000z/rm4ymkkeupgo5jfuzrts.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830152/products/nanoflare 1000z/syrhneosnjsnoyuwwdte.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830153/products/nanoflare 1000z/f8hfmdfuhux7bvs4s1zs.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830154/products/nanoflare 1000z/tyv2w5kyqdonv5qib1rt.jpg",
                        QuantityInStock = 13,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Thuskter Ryuga Metalic (Ver.2023)",
                        Price = 3600000,
                        Description = "All England Champ (2021) Racket Chosen",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152504/bobdvzdutlsnhkgd3csa.webp",
                        QuantityInStock = 10,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 2
                    },
                    new Product
                    {
                        Name = "Axtrox 88D Pro (Ver.2024)",
                        Price = 3900000,
                        Description = "New Shape - New Tech - New Feelings",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152808/erovfedlbzb0xkzqglbj.jpg",
                        QuantityInStock = 18,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Brave Sword 12 (Ver.55th 2024)",
                        Price = 3200000,
                        Description = "Speed - Control - Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153834/rweggufmgnga3zjklf2f.jpg",
                        QuantityInStock = 18,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 2
                    },
                    new Product
                    {
                        Name = "Thuskter Falcon White (Ver. Limited TYZ)",
                        Price = 2850000,
                        Description = "Speed - Control - Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153808/cy4dqkjmsqakqxsqonl5.jpg",
                        QuantityInStock = 18,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 2
                    },
                    new Product
                    {
                        Name = "Axforce 90 (Ver.Dragon Max)",
                        Price = 3600000,
                        Description = "Chosen By World Champion 2021 - Loh Kean Yew",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153857/algdodmsmknzhilm9wds.webp",
                        QuantityInStock = 18,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Axforce 90 (Ver.Tiger Max)",
                        Price = 3600000,
                        Description = "Chosen By All England 2024 Champion - Jonathan Christie",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153874/oq2ei2kd5fffz0fykkny.webp",
                        QuantityInStock = 18,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Axforce 100 (Ver.Kirin)",
                        Price = 4200000,
                        Description = "Modern Technologies - Powerful Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734927477/products/axforce 100 %28kirin%29/czjdbrlre4jnbrhfabyi.jpg",
                        QuantityInStock = 10,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Techtonic 9",
                        Price = 2800000,
                        Description = "Blow White Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153705/w9s0ruep5gxnelaunfgq.jpg",
                        QuantityInStock = 18,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Flame N55",
                        Price = 2500000,
                        Description = "Chosen by The World Champion (2014, 2015), The OLP Champion (2016) - Chen Long",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153773/cetkfwcafc8xliwnim9n.jpg",
                        QuantityInStock = 1,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Comfort Z",
                        Price = 2200000,
                        Description = "Smooth - Jump - Reach the win",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153525/bpjcwixbyweafni7t5sz.jpg",
                        QuantityInStock = 1,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Comfort Z3",
                        Price = 2750000,
                        Description = "The upgrade of Comfort Z Version",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153525/bpjcwixbyweafni7t5sz.jpg",
                        QuantityInStock = 1,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 1
                    },
                    new Product
                    {
                        Name = "Accelarate Booster (Ver.2022)",
                        Price = 1500000,
                        Description = "Smooth - Jump - Reach the win",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734926089/products/hxlh389m9vsug2zumawz.jpg",
                        QuantityInStock = 1,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 3
                    },
                    new Product
                    {
                        Name = "Accelarate Advanced (Ver.2024)",
                        Price = 1550000,
                        Description = "Speed - Jump - Dominate",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153621/dn25ivc2gpbcytdfqfim.webp",
                        QuantityInStock = 1,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 3
                    }
                );

                // 4. Seed Technology
                if(!context.Technologies.Any())
                {
                    context.AddRange(
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

                // 6. Seed Promotions
                if (!context.Promotions.Any())
                {
                    context.AddRange(
                        new Promotion { CategoryId = 1, BrandId = 1, Start = DateTime.Now, End = DateTime.MaxValue, PercentageDiscount = 15 }
                    );
                }

                context.SaveChanges();
            }
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
