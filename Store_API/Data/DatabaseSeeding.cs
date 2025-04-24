using Store_API.Models;
using Store_API.Models.Inventory;
using Store_API.Models.Users;

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
                    new Category { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), Name = "Racket" },
                    new Category { Id = Guid.Parse("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"), Name = "Shoes" },
                    new Category { Id = Guid.Parse("8a0ef9d4-79bb-418f-9e12-8f5f6df62049"), Name = "Clothes" },
                    new Category { Id = Guid.Parse("c1dcf6b8-4c24-493c-a828-7b1e4cc26a6b"), Name = "Items" },
                    new Category { Id = Guid.Parse("af0b3a7a-5898-43cf-8f98-d0c5712ec5f3"), Name = "Others" }
                );
            }

            // 2. Brand
            if (!context.Brands.Any())
            {
                await context.AddRangeAsync(
                    new Brand { Id = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6"), Name = "Yonex", Country = "Japan" },
                    new Brand { Id = Guid.Parse("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d"), Name = "Victor", Country = "Taiwan" },
                    new Brand { Id = Guid.Parse("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb"), Name = "Lining", Country = "China" }
                );
            }

            // 3. Seed Products
            if (!context.Products.Any())
            {
                await context.AddRangeAsync(
                    new Product
                    {
                        Id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                        Name = "Astrox 99 Pro (Ver.2021)",
                        Description = "Attack - Dominate - Conquers",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734927449/products/astrox 99 pro 2021/hgkgglo91lbmjhxby5h0.webp",
                        PublicId = "products/astrox 99 pro 2021/hgkgglo91lbmjhxby5h0",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    },
                    new Product
                    {
                        Id = Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"),
                        Name = "Duora Z Strike (Ver.2017)",
                        Description = "Two Faces - Expolosive Attack - Solid Hard Defend",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735828714/products/duora z strike %28ver.2017%29/he85nkkpfkdc6gh2w9ak.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828716/products/duora z strike %28ver.2017%29/ez7dx5lamzyjkwrs4zya.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828717/products/duora z strike %28ver.2017%29/zv47em4nf8cfex61lwh1.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828718/products/duora z strike %28ver.2017%29/jlj5w0sliaquoxfxziaf.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828719/products/duora z strike %28ver.2017%29/u8mj5igvuamhih3mgely.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828720/products/duora z strike %28ver.2017%29/qnelzoomlaqll2aikfbj.jpg",                       
                        PublicId = @"products/duora z strike %28ver.2017%29/he85nkkpfkdc6gh2w9ak,products/duora z strike %28ver.2017%29/ez7dx5lamzyjkwrs4zya,products/duora z strike %28ver.2017%29/zv47em4nf8cfex61lwh1,products/duora z strike %28ver.2017%29/jlj5w0sliaquoxfxziaf,products/duora z strike %28ver.2017%29/u8mj5igvuamhih3mgely,products/duora z strike %28ver.2017%29/qnelzoomlaqll2aikfbj",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    }, 
                    new Product
                    {
                        Id = Guid.Parse("7d9e6679-7425-40de-944b-e07fc1f90ae7"),
                        Name = "ArcSaber 11 Pro (Ver.2021)",
                        Description = "Controls - Focus - Feel",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735828775/products/arcsaber 11 pro %28ver.2021%29/ljra5olhsvaxrjptk0re.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828777/products/arcsaber 11 pro %28ver.2021%29/uskrx81pgum1grefnhcx.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828778/products/arcsaber 11 pro %28ver.2021%29/ewj0zhky5g7e5wvsi9sv.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828779/products/arcsaber 11 pro %28ver.2021%29/grokd1zbead4twuac0ly.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/wmnhk6gabpegyoq3lcpp.webp,https://res.cloudinary.com/duat1htay/image/upload/v1735828780/products/arcsaber 11 pro %28ver.2021%29/xkl3k09jjrixdaq8qypu.jpg",
                        PublicId = "arcsaber 11 pro %28ver.2021%29/ljra5olhsvaxrjptk0re,products/arcsaber 11 pro %28ver.2021%29/uskrx81pgum1grefnhcx,products/arcsaber 11 pro %28ver.2021%29/ewj0zhky5g7e5wvsi9sv,products/arcsaber 11 pro %28ver.2021%29/grokd1zbead4twuac0ly,products/arcsaber 11 pro %28ver.2021%29/wmnhk6gabpegyoq3lcpp,products/arcsaber 11 pro %28ver.2021%29/xkl3k09jjrixdaq8qypu",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    }, 
                    new Product
                    {
                        Id = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"),
                        Name = "Axtrox 100ZZ (Ver.Kurenai)",
                        Description = "Racket Choosing by The Olympic Champion (2020) Viktor Axelsen",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152726/ldpbvqnabfaq7o2uggia.webp",
                        PublicId = "ldpbvqnabfaq7o2uggia",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    }, 
                    new Product
                    {
                        Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        Name = "Nanoflare 1000Z",
                        Description = "King Double - Speed - Power",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735830149/products/nanoflare 1000z/yj14npg3jorqi1dhbygd.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830150/products/nanoflare 1000z/ptbxakwyi6dtxsedhog4.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830151/products/nanoflare 1000z/rm4ymkkeupgo5jfuzrts.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830152/products/nanoflare 1000z/syrhneosnjsnoyuwwdte.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830153/products/nanoflare 1000z/f8hfmdfuhux7bvs4s1zs.jpg,https://res.cloudinary.com/duat1htay/image/upload/v1735830154/products/nanoflare 1000z/tyv2w5kyqdonv5qib1rt.jpg",
                        PublicId = "products/nanoflare 1000z/yj14npg3jorqi1dhbygd,products/nanoflare 1000z/ptbxakwyi6dtxsedhog4,products/nanoflare 1000z/rm4ymkkeupgo5jfuzrts,products/nanoflare 1000z/syrhneosnjsnoyuwwdte,products/nanoflare 1000z/f8hfmdfuhux7bvs4s1zs,products/nanoflare 1000z/tyv2w5kyqdonv5qib1rt",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    },
                    new Product
                    {
                        Id = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"),
                        Name = "Thuskter Ryuga Metalic (Ver.2023)",
                        Description = "All England Champ (2021) Racket Chosen",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152504/bobdvzdutlsnhkgd3csa.webp",
                        PublicId = "bobdvzdutlsnhkgd3csa",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d")
                    },
                    new Product
                    {
                        Id = Guid.Parse("9b9f0b80-4f3d-11ec-81d3-0242ac130003"),
                        Name = "Axtrox 88D Pro (Ver.2024)",
                        Description = "New Shape - New Tech - New Feelings",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729152808/erovfedlbzb0xkzqglbj.jpg",
                        PublicId = "erovfedlbzb0xkzqglbj",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    },
                    new Product
                    {
                        Id = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff"),
                        Name = "Brave Sword 12 (Ver.55th 2024)",
                        Description = "Speed - Control - Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153834/rweggufmgnga3zjklf2f.jpg",
                        PublicId = "rweggufmgnga3zjklf2f",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d")
                    },
                    new Product
                    {
                        Id = Guid.Parse("b3e2f5f0-7e44-4e06-b69e-8f87be0c30f7"),
                        Name = "Thuskter Falcon White (Ver. Limited TYZ)",
                        Description = "Speed - Control - Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153808/cy4dqkjmsqakqxsqonl5.jpg",
                        PublicId = "cy4dqkjmsqakqxsqonl5",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("5378f75e-4a8a-4531-86f5-0c9b2f8a1b6d")
                    },
                    new Product
                    {
                        Id = Guid.Parse("cb3b0e7d-5ad3-4ec7-9b9a-4f06efb27c03"),
                        Name = "Axforce 90",
                        Description = "Chosen By World Champion 2021 - Loh Kean Yew",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153857/algdodmsmknzhilm9wds.webp",
                        PublicId = "algdodmsmknzhilm9wds",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb")
                    },
                    new Product
                    {
                        Id = Guid.Parse("e029d3c5-b6b3-4e31-bada-1e6b7d5af7c8"),
                        Name = "Axforce 100 (Ver.Kirin)",
                        Description = "Modern Technologies - Powerful Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734927477/products/axforce 100 %28kirin%29/czjdbrlre4jnbrhfabyi.jpg",
                        PublicId = "products/axforce 100 %28kirin%29/czjdbrlre4jnbrhfabyi",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb")
                    },
                    new Product
                    {
                        Id = Guid.Parse("8a97f9a6-221d-4f5b-bc37-6e5cb7a979b6"),
                        Name = "Techtonic 9",
                        Description = "Blow White Attack",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153705/w9s0ruep5gxnelaunfgq.jpg",
                        PublicId = "w9s0ruep5gxnelaunfgq",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb")
                    },
                    new Product
                    {
                        Id = Guid.Parse("dd36bf61-fc77-4cfb-82e1-6b2ff6f9b1d4"),
                        Name = "Flame N55",
                        Description = "Chosen by The World Champion (2014, 2015), The OLP Champion (2016) - Chen Long",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153773/cetkfwcafc8xliwnim9n.jpg",
                        PublicId = "cetkfwcafc8xliwnim9n",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"),
                        BrandId = Guid.Parse("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb")
                    },
                    new Product
                    {
                        Id = Guid.Parse("a2cf7e92-29fd-4d61-90b3-d3f2f8a7e9c6"),
                        Name = "Comfort Z",
                        Description = "Smooth - Jump - Reach the win",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153525/bpjcwixbyweafni7t5sz.jpg",
                        PublicId = "bpjcwixbyweafni7t5sz",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    },
                    new Product
                    {
                        Id = Guid.Parse("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"),
                        Name = "Comfort Z3",
                        Description = "The upgrade of Comfort Z Version",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153525/bpjcwixbyweafni7t5sz.jpg",
                        PublicId = "bpjcwixbyweafni7t5sz",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"),
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6")
                    },
                    new Product
                    {
                        Id = Guid.Parse("4d21b8e5-8a14-4b37-b84b-3d1c2e2e5f76"),
                        Name = "Accelarate Booster (Ver.2022)",
                        Description = "Smooth - Jump - Reach the win",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1734926089/products/hxlh389m9vsug2zumawz.jpg",
                        PublicId = "products/hxlh389m9vsug2zumawz",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"),
                        BrandId = Guid.Parse("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb")
                    },
                    new Product
                    {
                        Id = Guid.Parse("2f8c6a10-5633-4b91-90a1-7c924df78e68"),
                        Name = "Accelarate Advanced (Ver.2024)",
                        Description = "Speed - Jump - Dominate",
                        ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1729153621/dn25ivc2gpbcytdfqfim.webp",
                        PublicId = "dn25ivc2gpbcytdfqfim",
                        Created = DateTime.Now,
                        CategoryId = Guid.Parse("9d19c053-8b47-4e6d-9e9a-4188cb50d2e6"),
                        BrandId = Guid.Parse("b07c2e46-76a5-4b8a-92fb-7cc62e13b5cb")
                    }
                );
            }

            // 4. Seed Technology
            if (!context.Technologies.Any())
            {
                await context.AddRangeAsync(
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), Name = "ENHANCED ARCSABER FRAME", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800437/technologies/tech1_qoof2s.jpg" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d8"), Name = "CONTROL-ASSIST BUMPER", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800437/technologies/tech2_nhiopm.jpg" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d7"), Name = "POCKETING BOOSTER", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800436/technologies/tech3_olpj1g.png" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d6"), Name = "ISOMETRIC PLUS", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735800436/technologies/tech3_olpj1g.png" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d5"), Name = "DUAL OPTIMUM SYSTEM", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech5_axkpsh.webp" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d4"), Name = "ISOMETRIC ", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801836/tech6_foheeo.webp" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d3"), Name = "AERO-BOX FRAME", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech7_qfd4za.webp" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d2"), Name = "NEW Built-in T-Joint", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801835/tech8_mqshpr.webp" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d1"), Name = "ROTATIONAL GENARATOR SYSTEM", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801836/tech9_jfuxth.jpg" },
                    new Technology { Id = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d0"), Name = "ENERGY BOOST CAP PLUS", Description = "", ImageUrl = "https://res.cloudinary.com/duat1htay/image/upload/v1735801837/tech10_myratn.jpg" }
                );
            }

            // 5. Seed Product - Technologies
            if (context.Products.Any() && context.Technologies.Any() && !context.ProductTechnologies.Any())
            {
                context.AddRange(
                    new ProductTechnology { ProductId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9") },
                    new ProductTechnology { ProductId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d8") },
                    new ProductTechnology { ProductId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d7") },
                    new ProductTechnology { ProductId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d6") },
                    
                    new ProductTechnology { ProductId = Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d5") },
                    new ProductTechnology { ProductId = Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d4") },
                    new ProductTechnology { ProductId = Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d3") },
                    new ProductTechnology { ProductId = Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d2") },
                    
                    new ProductTechnology { ProductId = Guid.Parse("7d9e6679-7425-40de-944b-e07fc1f90ae7"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9") },
                    new ProductTechnology { ProductId = Guid.Parse("7d9e6679-7425-40de-944b-e07fc1f90ae7"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d1") },
                    new ProductTechnology { ProductId = Guid.Parse("7d9e6679-7425-40de-944b-e07fc1f90ae7"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d0") },
                    
                    new ProductTechnology { ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9") },
                    new ProductTechnology { ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d8") },
                    new ProductTechnology { ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d7") },
                    new ProductTechnology { ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), TechnologyId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d5") }
                    );
            }

            // 6. Seed ProductDetail
            if (context.Products.Any() && !context.ProductDetails.Any())
            {
                await context.AddRangeAsync(
                    // 99 Pro
                    new ProductDetail { Id = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), ProductId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), Price = 4200000, Status = 1, Color = "#880808", ExtraName = "Red Tiger" },
                    new ProductDetail { Id = Guid.Parse("e2c8ff1c-2db0-4a02-9a2a-7b8d05eeb6d4"), ProductId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"), Price = 4300000, Status = 1, Color = "#fff", ExtraName = "White Tiger" },
                    // Z-strike        
                    new ProductDetail { Id = Guid.Parse("5f3c3a57-1f41-4e32-9c7a-12d4686dbf8b"), ProductId = Guid.Parse("6f9619ff-8b86-d011-b42d-00cf4fc964ff"), Price = 4150000, Status = 1, Color = "#fff", ExtraName = "Chou Tien Chen Signature!" },
                    // Arc 11 Pro      
                    new ProductDetail { Id = Guid.Parse("7a3f4036-942f-4f8a-a823-0f3c5c791e20"), ProductId = Guid.Parse("7d9e6679-7425-40de-944b-e07fc1f90ae7"), Price = 4250000, Status = 1, Color = "#880808" },
                    // 100ZZ           
                    new ProductDetail { Id = Guid.Parse("c9b74e77-dc8b-4c4e-96c9-d6b2e8adf2cf"), ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), Price = 4500000, Status = 1, Color = "#4169E1", ExtraName = "Navy Blue" },
                    new ProductDetail { Id = Guid.Parse("f01d30c9-b2a1-4d37-95b4-018cbacfd6ef"), ProductId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000"), Price = 4450000, Status = 1, Color = "#880808", ExtraName = "Ver.Kurenai" },
                    // 1000Z            
                    new ProductDetail { Id = Guid.Parse("2e8c3bc1-23e5-4df9-822c-2f7d9dd4f5f3"), ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Price = 4350000, Status = 1, Color = "#FDDA0D", ExtraName = "The Yellow Flash" },
                    new ProductDetail { Id = Guid.Parse("51fa47d3-9baf-4e71-bdd8-6206533a126c"), ProductId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"), Price = 15000000, Status = 1, Color = "#880808", ExtraName = "Limited Edition (2025)" },
                    // Ryuga Metalic    
                    new ProductDetail { Id = Guid.Parse("6a4e5f76-3c84-4f4e-bb76-61768c5d3e7d"), ProductId = Guid.Parse("123e4567-e89b-12d3-a456-426614174000"), Price = 3600000, Status = 1, Color = "#FF5733", ExtraName = "Lee Zii Ja Chosen" },
                    // 88D Pro          
                    new ProductDetail { Id = Guid.Parse("d55b3f65-68b2-4c5e-85ae-8f2a3bfb6b8f"), ProductId = Guid.Parse("9b9f0b80-4f3d-11ec-81d3-0242ac130003"), Price = 4200000, Status = 1, Color = "#7393B3" },
                    // BS12             
                    new ProductDetail { Id = Guid.Parse("e4d849aa-7683-47e5-9f45-2e4894a3ddf4"), ProductId = Guid.Parse("00112233-4455-6677-8899-aabbccddeeff"), Price = 2800000, Status = 1, Color = "#4169E1" },
                    // Thuster Falcon TTY
                    new ProductDetail { Id = Guid.Parse("3b6e123a-f75c-4de5-86a5-d2b5e8b6c9d2"), ProductId = Guid.Parse("b3e2f5f0-7e44-4e06-b69e-8f87be0c30f7"), Price = 2700000, Status = 1, Color = "#fff" },
                    // Ax90             
                    new ProductDetail { Id = Guid.Parse("0c462b3e-61c9-4e34-bab2-7d82c4c5e8e1"), ProductId = Guid.Parse("cb3b0e7d-5ad3-4ec7-9b9a-4f06efb27c03"), Price = 3850000, Status = 1, Color = "#880808", ExtraName = "Ver.Tiger Max" },
                    new ProductDetail { Id = Guid.Parse("8b7f69d4-459c-45c8-bf38-9f5b214a9d7e"), ProductId = Guid.Parse("cb3b0e7d-5ad3-4ec7-9b9a-4f06efb27c03"), Price = 3880000, Status = 1, Color = "#4169E1", ExtraName = "Ver.Dragon Max" },
                    // Ax100           
                    new ProductDetail { Id = Guid.Parse("a2e987b6-fdbc-4d9a-a86b-6f9cb4e7f236"), ProductId = Guid.Parse("e029d3c5-b6b3-4e31-bada-1e6b7d5af7c8"), Price = 4250000, Status = 1, Color = "#880808", ExtraName = "Ver.Kirin" },
                    // Tectonic 9      
                    new ProductDetail { Id = Guid.Parse("13c87621-8b94-4515-90d4-35f5f8a4b23e"), ProductId = Guid.Parse("8a97f9a6-221d-4f5b-bc37-6e5cb7a979b6"), Price = 3300000, Status = 1, Color = "#fff" },
                    // Flame N55       
                    new ProductDetail { Id = Guid.Parse("2fa0f68b-efc9-4a92-b4c3-8f62c4d8e5a1"), ProductId = Guid.Parse("dd36bf61-fc77-4cfb-82e1-6b2ff6f9b1d4"), Price = 5000000, Status = 1, Color = "#880808", ExtraName = "Chen Long Edition (Rio 2016)" },
                    // Comfort Z        
                    new ProductDetail { Id = Guid.Parse("5d479eab-b8c6-4df1-99f7-df3a7b2e6f87"), ProductId = Guid.Parse("a2cf7e92-29fd-4d61-90b3-d3f2f8a7e9c6"), Price = 2200000, Status = 1, Color = "#333" },
                    // Comfort Z3       
                    new ProductDetail { Id = Guid.Parse("7f1b9d38-3b5d-474f-832b-85c7c5d2a9b4"), ProductId = Guid.Parse("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"), Price = 2850000, Status = 1, Color = "#880808", ExtraName = "Ver.Red" },
                    new ProductDetail { Id = Guid.Parse("b9f376e1-6a5d-4b34-9a1c-3f9e8a7b2d5c"), ProductId = Guid.Parse("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"), Price = 2830000, Status = 1, Color = "#4169E1", ExtraName = "Ver.Blue" },
                    new ProductDetail { Id = Guid.Parse("41d3f8b7-c1e2-456f-a9c8-72b3d2e5f9a4"), ProductId = Guid.Parse("68d0b964-88b1-4c56-a6ea-7253c8a94b4d"), Price = 2800000, Status = 1, Color = "#FF69B4", ExtraName = "Ver.Pink" },
                    // Victor            
                    new ProductDetail { Id = Guid.Parse("9d5a72c4-1f87-4b3a-b7e8-d4c5f9a2e3b6"), ProductId = Guid.Parse("4d21b8e5-8a14-4b37-b84b-3d1c2e2e5f76"), Price = 1500000, Status = 1, Color = "#4169E1" },
                    new ProductDetail { Id = Guid.Parse("63e7c5d2-9b4f-4f38-b7d1-85f9a3e2c4d8"), ProductId = Guid.Parse("2f8c6a10-5633-4b91-90a1-7c924df78e68"), Price = 1700000, Status = 1, Color = "#fff" }

                );
            }

            // 7. Seed Promotions
            if (!context.Promotions.Any())
            {
                await context.AddRangeAsync(
                    new Promotion 
                    {   CategoryId = Guid.Parse("3f8d2c42-bf26-4f93-b2d4-7e3c75e7a6d9"), 
                        BrandId = Guid.Parse("e1798a79-327e-4851-9028-b1c9b2e82ec6"),
                        StartDate = DateTime.Now, 
                        EndDate = DateTime.MaxValue, 
                        PercentageDiscount = 15 
                    }
                );
            }

            // 8. Seed Warehouse
            if(!context.Warehouses.Any())
            {
                await context.AddRangeAsync(
                    new Warehouse
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Name = "HCM Warehouse",
                        Location = "Ho Chi Minh City",
                        Created = DateTime.Now,
                    },
                    new Warehouse
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Name = "Thu Duc Warehouse",
                        Location = "Thu Duc City",
                        Created = DateTime.Now,
                    },
                    new Warehouse
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Name = "HHT Warehouse",
                        Location = "Vung Tau City",
                        Created = DateTime.Now,
                    }
                );
            }

            // 9. Seed Stocks
            if (context.Warehouses.Any() && context.ProductDetails.Any() && !context.Stocks.Any())
            {
                await context.AddRangeAsync(
                    new Stock
                    {
                        Id = Guid.Parse("99999999-1111-1111-1111-111111111111"),
                        ProductDetailId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 10,
                        Updated = DateTime.Now,                
                    },
                    new Stock
                    {
                        Id = Guid.Parse("99999999-1111-1111-8888-111111111111"),
                        ProductDetailId = Guid.Parse("e2c8ff1c-2db0-4a02-9a2a-7b8d05eeb6d4"),
                        WarehouseId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Quantity = 15,
                        Updated = DateTime.Now,
                    },
                    new Stock
                    {
                        Id = Guid.Parse("99999999-1111-7777-1111-111111111111"),
                        ProductDetailId = Guid.Parse("5f3c3a57-1f41-4e32-9c7a-12d4686dbf8b"),
                        WarehouseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Quantity = 18,
                        Updated = DateTime.Now,
                    },
                    new Stock
                    {
                        Id = Guid.Parse("99999999-6666-1111-1111-111111111111"),
                        ProductDetailId = Guid.Parse("f01d30c9-b2a1-4d37-95b4-018cbacfd6ef"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 20,
                        Updated = DateTime.Now,
                    },
                    new Stock
                    {
                        Id = Guid.Parse("99999999-1111-5555-8888-111111111111"),
                        ProductDetailId = Guid.Parse("51fa47d3-9baf-4e71-bdd8-6206533a126c"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 3,
                        Updated = DateTime.Now,
                    },
                    new Stock
                    {
                        Id = Guid.Parse("99999999-1111-5555-8888-111111111158"),
                        ProductDetailId = Guid.Parse("51fa47d3-9baf-4e71-bdd8-6206533a126c"),
                        WarehouseId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Quantity = 2,
                        Updated = DateTime.Now,
                    }
                );
            }

            // 10. Seed StockTransaction
            if (context.Warehouses.Any() && context.ProductDetails.Any() && !context.StockTransactions.Any())
            {
                await context.AddRangeAsync(

                    // Ax99 Red Transaction Stock
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999999-1111-3333-2222-111111111111"),
                        ProductDetailId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 8,
                        TransactionType = 1, // Import                       
                        Created = DateTime.Now,
                    },
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999999-1111-3333-4545-111111111111"),
                        ProductDetailId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 3,
                        TransactionType = 1, // Import                       
                        Created = DateTime.Now,
                    },
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999999-2222-3333-4545-111111111111"),
                        ProductDetailId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 1,
                        TransactionType = 0, // Export                       
                        Created = DateTime.Now,
                    },

                    // Ax99 White
                    new StockTransaction
                    {
                        Id = Guid.Parse("88888888-1111-3333-2222-111111111111"),
                        ProductDetailId = Guid.Parse("e2c8ff1c-2db0-4a02-9a2a-7b8d05eeb6d4"),
                        WarehouseId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Quantity = 15,
                        TransactionType = 1, // Import                       
                        Created = DateTime.Now,
                    },
                    
                    // Z-Strike
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999333-befa-3333-4545-111111111111"),
                        ProductDetailId = Guid.Parse("5f3c3a57-1f41-4e32-9c7a-12d4686dbf8b"),
                        WarehouseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Quantity = 18,
                        TransactionType = 1, // Import                       
                        Created = DateTime.Now,
                    },
                    
                    // 100ZZ
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999875-abef-3333-8217-111111111111"),
                        ProductDetailId = Guid.Parse("f01d30c9-b2a1-4d37-95b4-018cbacfd6ef"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 20,
                        TransactionType = 1, // Import                       
                        Created = DateTime.Now,
                    },

                    // 1000Z
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999875-abcd-aaaa-8217-111111111111"),
                        ProductDetailId = Guid.Parse("51fa47d3-9baf-4e71-bdd8-6206533a126c"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 4,
                        TransactionType = 1, // Import                       
                        Created = DateTime.Now,
                    },
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999875-abcd-aaaa-8217-222111111111"),
                        ProductDetailId = Guid.Parse("51fa47d3-9baf-4e71-bdd8-6206533a126c"),
                        WarehouseId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Quantity = 2,
                        TransactionType = 1, // Import                       
                        Created = DateTime.Now,
                    },
                    new StockTransaction
                    {
                        Id = Guid.Parse("99999875-abcd-aaaa-8217-111111111988"),
                        ProductDetailId = Guid.Parse("51fa47d3-9baf-4e71-bdd8-6206533a126c"),
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Quantity = 1,
                        TransactionType = 0, // Export                       
                        Created = DateTime.Now,
                    }
                );
            }

            // 11. Seed User Warehouse
            if(context.Warehouses.Any() && !context.UserWarehouses.Any())
            {
                await context.AddRangeAsync(
                    new UserWarehouse
                    {
                        Id = Guid.NewGuid(),
                        UserId = 2,
                        WarehouseId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    },
                    new UserWarehouse
                    {
                        Id = Guid.NewGuid(),
                        UserId = 3,
                        WarehouseId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    },
                    new UserWarehouse
                    {
                        Id = Guid.NewGuid(),
                        UserId = 4,
                        WarehouseId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    }
                );
            }

            // 12. Seed Rating
            if (!context.Ratings.Any() && context.Users.Any() && context.Products.Any() && context.ProductDetails.Any())
            {
                await context.AddRangeAsync(
                    // 99 Pro
                    new Rating
                    {
                        UserId = 2,
                        ProductId = Guid.Parse("F47AC10B-58CC-4372-A567-0E02B2C3D479"),
                        ProductDetailId = Guid.Parse("3FA85F64-5717-4562-B3FC-2C963F66AFA6"),
                        Star = 4,
                    },
                    new Rating
                    {
                        UserId = 2,
                        ProductId = Guid.Parse("F47AC10B-58CC-4372-A567-0E02B2C3D479"),
                        ProductDetailId = Guid.Parse("E2C8FF1C-2DB0-4A02-9A2A-7B8D05EEB6D4"),
                        Star = 4.5,
                    },
                    // Z Strike
                    new Rating
                    {
                        UserId = 4,
                        ProductId = Guid.Parse("6F9619FF-8B86-D011-B42D-00CF4FC964FF"),
                        ProductDetailId = Guid.Parse("5F3C3A57-1F41-4E32-9C7A-12D4686DBF8B"),
                        Star = 4.8,
                    },
                    // Ryuga Metalic
                    new Rating
                    {
                        UserId = 3,
                        ProductId = Guid.Parse("123E4567-E89B-12D3-A456-426614174000"),
                        ProductDetailId = Guid.Parse("6A4E5F76-3C84-4F4E-BB76-61768C5D3E7D"),
                        Star = 4.5,
                    }
                );
            }

            // Save changes to the database
            context.SaveChanges();
        }
    }
}
