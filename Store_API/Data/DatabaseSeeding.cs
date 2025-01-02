using Microsoft.AspNetCore.Identity;
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
                        Price = 25,
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
                        Price = 23,
                        Description = "Two Faces - Expolosive Attack - Solid Hard Defend",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729048365/cvha8vb07gw68tjy8n8a.png",
                        QuantityInStock = 10,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "ArcSaber 11 Pro (Ver.2021)",
                        Price = 22,
                        Description = "Controls - Focus - Feel",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735788498/products/arcsaber 11 pro/lmfmhdb2uhwbi5qlypi1.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735788500/products/arcsaber 11 pro/nvm3dgqrszkyfdwtvf6w.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735788501/products/arcsaber 11 pro/elbm2zrrpmjprgoaiwdv.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735788502/products/arcsaber 11 pro/tuv4fkgdsvt1jdcmjla5.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735788503/products/arcsaber 11 pro/cemxitfnbtfpp7y4cnvd.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735788503/products/arcsaber 11 pro/txafxg1tftqemx65oftx.webp', N'products/arcsaber 11 pro/lmfmhdb2uhwbi5qlypi1,products/arcsaber 11 pro/nvm3dgqrszkyfdwtvf6w,products/arcsaber 11 pro/elbm2zrrpmjprgoaiwdv,products/arcsaber 11 pro/tuv4fkgdsvt1jdcmjla5,products/arcsaber 11 pro/cemxitfnbtfpp7y4cnvd,products/arcsaber 11 pro/txafxg1tftqemx65oftx.webp",
                        QuantityInStock = 15,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 1,
                        BrandId = 1
                    }, 
                    new Product
                    {
                        Name = "Axtrox 100ZZ (Ver.Kurenai)",
                        Price = 25,
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
                        Name = "Astrox 99 Pro (Ver.2021)",
                        Price = 25,
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
                        Name = "Thuskter Ryuga Metalic (Ver.2023)",
                        Price = 20,
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
                        Price = 25,
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
                        Price = 21,
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
                        Price = 21.5,
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
                        Price = 22.5,
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
                        Price = 22.5,
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
                        Price = 23.5,
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
                        Price = 20.5,
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
                        Price = 20,
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
                        Price = 12,
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
                        Price = 15,
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
                        Price = 8,
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
                        Price = 12,
                        Description = "Speed - Jump - Dominate",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153621/dn25ivc2gpbcytdfqfim.webp",
                        QuantityInStock = 1,
                        ProductStatus = ProductStatus.Active,
                        Created = DateTime.Now,
                        CategoryId = 2,
                        BrandId = 3
                    }
                );

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

                context.SaveChanges();
            }
        }
    }
}
