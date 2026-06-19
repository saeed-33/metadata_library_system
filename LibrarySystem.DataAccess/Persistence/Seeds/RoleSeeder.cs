using LibrarySystem.DataAccess.Persistence.Contexts;
using LibrarySystem.DataAccess.Persistence.models;
using LibrarySystem.Domain.common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.DataAccess.Persistence.Seeds
{
    public static class RoleSeeder
    {
        private const string SeedUser = "development-seeder";

        public static async Task SeedRolesAsync(RoleManager<AppRoleModel> roleManager)
        {
            string[] roles =
            {
                SystemRoles.Admin,
                SystemRoles.Librarian,
                SystemRoles.User,
                SystemRoles.Guest
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new AppRoleModel
                    {
                        Name = role,
                        Description = $"{role} role"
                    });
                }
            }
        }

        public static class MetadataSeeder
        {
            public static async Task SeedAsync(ApplicationDbContext context)
            {
                if (await context.Items.AnyAsync(item => item.CreatedBy == SeedUser))
                {
                    return;
                }

                var vocabularies = await SeedVocabulariesAsync(context);
                var properties = await SeedPropertiesAsync(context, vocabularies);
                var templates = await SeedTemplatesAsync(context, properties);
                var users = await SeedSystemUsersAsync(context);
                var items = await SeedItemsAsync(context, templates, users, properties);

                await SeedMediaAsync(context, items, users, properties);
                await SeedItemSetsAsync(context, items, users);
                await SeedLibrarySettingsAsync(context);

            }
            public static async Task SeedLibrarySettingsAsync(ApplicationDbContext context)
            {
                // التحقق من وجود الإعداد مسبقاً لمنع التكرار
                if (!await context.SystemSettings.AnyAsync(s => s.Key == "GlobalBorrowDays"))
                {
                    var defaultSetting = new SystemSettingModel
                    {
                        Key = "GlobalBorrowDays",
                        Value = "14", // القيمة الافتراضية 14 يوم
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = SeedUser,
                        IsDeleted = false
                    };

                    await context.SystemSettings.AddAsync(defaultSetting);
                    await context.SaveChangesAsync();
                }
            }

            private static async Task<Dictionary<string, VocabularyModel>> SeedVocabulariesAsync(ApplicationDbContext context)
            {
                var seeds = new[]
                {
                    new VocabularyModel { Prefix = "dc", NamespaceUri = "http://purl.org/dc/elements/1.1/", Label = "Dublin Core", CreatedBy = SeedUser },
                    new VocabularyModel { Prefix = "dcterms", NamespaceUri = "http://purl.org/dc/terms/", Label = "Dublin Core Terms", CreatedBy = SeedUser },
                    new VocabularyModel { Prefix = "schema", NamespaceUri = "https://schema.org/", Label = "Schema.org", CreatedBy = SeedUser },
                    new VocabularyModel { Prefix = "bibo", NamespaceUri = "http://purl.org/ontology/bibo/", Label = "Bibliographic Ontology", CreatedBy = SeedUser },
                    new VocabularyModel { Prefix = "lib", NamespaceUri = "https://sh3raa.local/terms/", Label = "Sh3raa Library Terms", CreatedBy = SeedUser }
                };

                foreach (var seed in seeds)
                {
                    if (!await context.Vocabularies.AnyAsync(v => v.Prefix == seed.Prefix))
                    {
                        await context.Vocabularies.AddAsync(seed);
                    }
                }

                await context.SaveChangesAsync();

                return await context.Vocabularies
                    .Where(v => seeds.Select(seed => seed.Prefix).Contains(v.Prefix))
                    .ToDictionaryAsync(v => v.Prefix);
            }

            private static async Task<Dictionary<string, PropertyModel>> SeedPropertiesAsync(
                ApplicationDbContext context,
                Dictionary<string, VocabularyModel> vocabularies)
            {
                var seeds = new[]
                {
                    Property("title", "dc", "title", "Title", "http://purl.org/dc/elements/1.1/title"),
                    Property("creator", "dc", "creator", "Creator", "http://purl.org/dc/elements/1.1/creator"),
                    Property("subject", "dc", "subject", "Subject", "http://purl.org/dc/elements/1.1/subject"),
                    Property("description", "dc", "description", "Description", "http://purl.org/dc/elements/1.1/description"),
                    Property("publisher", "dc", "publisher", "Publisher", "http://purl.org/dc/elements/1.1/publisher"),
                    Property("date", "dc", "date", "Date", "http://purl.org/dc/elements/1.1/date"),
                    Property("language", "dc", "language", "Language", "http://purl.org/dc/elements/1.1/language"),
                    Property("identifier", "dc", "identifier", "Identifier", "http://purl.org/dc/elements/1.1/identifier"),
                    Property("type", "dc", "type", "Type", "http://purl.org/dc/elements/1.1/type"),
                    Property("format", "dc", "format", "Format", "http://purl.org/dc/elements/1.1/format"),
                    Property("rights", "dc", "rights", "Rights", "http://purl.org/dc/elements/1.1/rights"),
                    Property("issued", "dcterms", "issued", "Issued", "http://purl.org/dc/terms/issued"),
                    Property("abstract", "dcterms", "abstract", "Abstract", "http://purl.org/dc/terms/abstract"),
                    Property("spatial", "dcterms", "spatial", "Place", "http://purl.org/dc/terms/spatial"),
                    Property("isbn", "bibo", "isbn", "ISBN", "http://purl.org/ontology/bibo/isbn"),
                    Property("edition", "bibo", "edition", "Edition", "http://purl.org/ontology/bibo/edition"),
                    Property("pages", "bibo", "numPages", "Pages", "http://purl.org/ontology/bibo/numPages"),
                    Property("genre", "schema", "genre", "Genre", "https://schema.org/genre"),
                    Property("keywords", "schema", "keywords", "Keywords", "https://schema.org/keywords"),
                    Property("callNumber", "lib", "callNumber", "Call Number", "https://sh3raa.local/terms/callNumber"),
                    Property("shelf", "lib", "shelf", "Shelf", "https://sh3raa.local/terms/shelf"),
                    Property("availability", "lib", "availability", "Availability", "https://sh3raa.local/terms/availability")
                };

                foreach (var seed in seeds)
                {
                    if (!await context.Properties.AnyAsync(p => p.TermUri == seed.TermUri))
                    {
                        seed.VocabularyId = vocabularies[seed.CreatedBy!].Id;
                        seed.CreatedBy = SeedUser;
                        await context.Properties.AddAsync(seed);
                    }
                }

                await context.SaveChangesAsync();

                return await context.Properties
                    .Where(p => seeds.Select(seed => seed.TermUri).Contains(p.TermUri))
                    .ToDictionaryAsync(p => p.LocalName);

                static PropertyModel Property(string key, string vocabularyPrefix, string localName, string label, string termUri)
                {
                    return new PropertyModel
                    {
                        CreatedBy = vocabularyPrefix,
                        LocalName = localName,
                        Label = label,
                        TermUri = termUri
                    };
                }
            }

            private static async Task<Dictionary<string, ResourceTemplateModel>> SeedTemplatesAsync(
                ApplicationDbContext context,
                Dictionary<string, PropertyModel> properties)
            {
                var templates = new[]
                {
                    new ResourceTemplateModel { Label = "Printed Book", Description = "Books with bibliographic and shelving metadata.", CreatedBy = SeedUser },
                    new ResourceTemplateModel { Label = "Manuscript", Description = "Historical manuscripts and handwritten works.", CreatedBy = SeedUser },
                    new ResourceTemplateModel { Label = "Thesis", Description = "Academic theses and dissertations.", CreatedBy = SeedUser },
                    new ResourceTemplateModel { Label = "Journal Article", Description = "Articles, papers, and periodical records.", CreatedBy = SeedUser },
                    new ResourceTemplateModel { Label = "Audio Visual", Description = "Audio, video, image, and scanned resources.", CreatedBy = SeedUser }
                };

                foreach (var template in templates)
                {
                    if (!await context.ResourceTemplates.AnyAsync(t => t.Label == template.Label))
                    {
                        await context.ResourceTemplates.AddAsync(template);
                    }
                }

                await context.SaveChangesAsync();

                var result = await context.ResourceTemplates
                    .Where(t => templates.Select(seed => seed.Label).Contains(t.Label))
                    .ToDictionaryAsync(t => t.Label);

                await AddTemplatePropertiesAsync(context, result["Printed Book"], properties, true, "title", "creator", "publisher", "date", "language", "isbn", "edition", "numPages", "genre", "callNumber", "shelf", "availability");
                await AddTemplatePropertiesAsync(context, result["Manuscript"], properties, true, "title", "creator", "date", "language", "description", "spatial", "rights", "callNumber", "shelf");
                await AddTemplatePropertiesAsync(context, result["Thesis"], properties, true, "title", "creator", "publisher", "date", "language", "abstract", "subject", "keywords", "numPages", "callNumber");
                await AddTemplatePropertiesAsync(context, result["Journal Article"], properties, true, "title", "creator", "publisher", "date", "language", "abstract", "subject", "identifier", "keywords");
                await AddTemplatePropertiesAsync(context, result["Audio Visual"], properties, true, "title", "creator", "date", "language", "format", "description", "rights", "availability");

                return result;
            }

            private static async Task AddTemplatePropertiesAsync(
                ApplicationDbContext context,
                ResourceTemplateModel template,
                Dictionary<string, PropertyModel> properties,
                bool firstTwoRequired,
                params string[] propertyKeys)
            {
                for (var index = 0; index < propertyKeys.Length; index++)
                {
                    var property = properties[propertyKeys[index]];
                    var exists = await context.TemplateProperties.AnyAsync(tp =>
                        tp.TemplateId == template.Id && tp.PropertyId == property.Id);

                    if (!exists)
                    {
                        await context.TemplateProperties.AddAsync(new TemplatePropertyModel
                        {
                            TemplateId = template.Id,
                            PropertyId = property.Id,
                            IsRequired = firstTwoRequired && index < 2,
                            DisplayOrder = index + 1
                        });
                    }
                }

                await context.SaveChangesAsync();
            }

            private static async Task<Dictionary<string, SystemUserModel>> SeedSystemUsersAsync(ApplicationDbContext context)
            {
                var users = new[]
                {
                    new SystemUserModel { ExternalId = "seed-admin", FullName = "Omar Al-Khatib", Bio = "System administrator and catalog reviewer.", ProfilePicturePath = "/uploads/profiles/omar.png", CreatedBy = SeedUser },
                    new SystemUserModel { ExternalId = "seed-librarian-1", FullName = "Lina Haddad", Bio = "Metadata librarian focused on Arabic collections.", ProfilePicturePath = "/uploads/profiles/lina.png", CreatedBy = SeedUser },
                    new SystemUserModel { ExternalId = "seed-librarian-2", FullName = "Yazan Darwish", Bio = "Digital resources and media curator.", ProfilePicturePath = "/uploads/profiles/yazan.png", CreatedBy = SeedUser },
                    new SystemUserModel { ExternalId = "seed-researcher-1", FullName = "Maya Nasser", Bio = "Researcher interested in Levantine cultural history.", ProfilePicturePath = "/uploads/profiles/maya.png", CreatedBy = SeedUser },
                    new SystemUserModel { ExternalId = "seed-guest-1", FullName = "Sami Barakat", Bio = "Guest contributor for test workflows.", ProfilePicturePath = "/uploads/profiles/sami.png", CreatedBy = SeedUser }
                };

                foreach (var user in users)
                {
                    if (!await context.SystemUsers.AnyAsync(u => u.ExternalId == user.ExternalId))
                    {
                        await context.SystemUsers.AddAsync(user);
                    }
                }

                await context.SaveChangesAsync();

                return await context.SystemUsers
                    .Where(u => users.Select(seed => seed.ExternalId).Contains(u.ExternalId))
                    .ToDictionaryAsync(u => u.ExternalId);
            }

            private static async Task<List<ItemModel>> SeedItemsAsync(
                ApplicationDbContext context,
                Dictionary<string, ResourceTemplateModel> templates,
                Dictionary<string, SystemUserModel> users,
                Dictionary<string, PropertyModel> properties)
            {
                var seeds = new[]
                {
                    Item("Printed Book", "seed-librarian-1", "The Damascus Reader", "Nadia Qabbani", "Damascus University Press", "2018", "en", "978-1-23456-001-7", "History", "DS99.D3 R43", "A-01", "Available"),
                    Item("Printed Book", "seed-librarian-1", "مدخل إلى الفهرسة الحديثة", "سليم منصور", "دار المعرفة", "2021", "ar", "978-9933-22-100-4", "Library Science", "Z695 M36", "A-02", "Available"),
                    Item("Printed Book", "seed-librarian-2", "Digital Archives in Practice", "Helen Morgan", "Open Heritage", "2020", "en", "978-0-55555-777-2", "Archives", "CD973 M67", "B-03", "Checked out"),
                    Item("Manuscript", "seed-librarian-1", "رسالة في الحساب", "مؤلف مجهول", "Private Collection", "1842", "ar", "MS-1842-11", "Manuscripts", "MS 12.4", "M-01", "Reading room only"),
                    Item("Manuscript", "seed-researcher-1", "Travel Notes from Aleppo", "Fares Al-Halabi", "Family Papers", "1911", "en", "MS-1911-03", "Travel", "MS 18.2", "M-02", "Digitized"),
                    Item("Thesis", "seed-researcher-1", "Metadata Quality in Community Libraries", "Maya Nasser", "Sh3raa Institute", "2024", "en", "TH-2024-015", "Metadata", "Z666 N37", "T-01", "Available"),
                    Item("Thesis", "seed-librarian-2", "أثر المكتبات الرقمية على التعليم", "رنا يوسف", "جامعة دمشق", "2023", "ar", "TH-2023-044", "Digital Libraries", "ZA4080 Y68", "T-02", "Available"),
                    Item("Journal Article", "seed-librarian-2", "Linked Data for Small Libraries", "Adam West", "Journal of Library Systems", "2022", "en", "10.1000/jls.2022.18", "Linked Data", "ART-2022-18", "J-01", "Online"),
                    Item("Journal Article", "seed-librarian-1", "الفهارس العربية وتجربة المستخدم", "ليلى حموي", "مجلة المعلومات", "2020", "ar", "ART-2020-07", "UX", "ART-2020-07", "J-02", "Online"),
                    Item("Audio Visual", "seed-librarian-2", "Oral History: Old Damascus Markets", "Sh3raa Media Team", "Sh3raa Library", "2019", "ar", "AV-2019-021", "Oral History", "AV 21", "AV-01", "Available"),
                    Item("Audio Visual", "seed-librarian-2", "Cataloging Workshop Recording", "Training Office", "Sh3raa Library", "2025", "en", "AV-2025-004", "Training", "AV 44", "AV-02", "Available"),
                    Item("Printed Book", "seed-admin", "Introduction to REST APIs", "Karim Saleh", "Tech House", "2022", "en", "978-1-99999-121-5", "Software", "QA76.73 S25", "C-01", "Available")
                };

                var items = new List<ItemModel>();

                foreach (var seed in seeds)
                {
                    if (await context.Values.AnyAsync(v =>
                        v.PropertyId == properties["title"].Id && v.ValueText == seed.Title))
                    {
                        continue;
                    }

                    var item = new ItemModel
                    {
                        Type = "Item",
                        TemplateId = templates[seed.Template].Id,
                        OwnerId = users[seed.Owner].Id,
                        CreatedBy = SeedUser
                    };
                    await context.Items.AddAsync(item);
                    items.Add(item);
                }

                await context.SaveChangesAsync();

                foreach (var item in items)
                {
                    var seed = seeds[items.IndexOf(item)];
                    await AddValuesAsync(context, item.Id, properties, new Dictionary<string, string?>
                    {
                        ["title"] = seed.Title,
                        ["creator"] = seed.Creator,
                        ["publisher"] = seed.Publisher,
                        ["date"] = seed.Date,
                        ["language"] = seed.Language,
                        ["identifier"] = seed.Identifier,
                        ["genre"] = seed.Genre,
                        ["callNumber"] = seed.CallNumber,
                        ["shelf"] = seed.Shelf,
                        ["availability"] = seed.Availability,
                        ["description"] = $"Development sample for {seed.Title}.",
                        ["subject"] = seed.Genre,
                        ["keywords"] = $"{seed.Genre}, library, sample data"
                    }, seed.Language);
                }

                return await context.Items
                    .Where(i => i.CreatedBy == SeedUser)
                    .Include(i => i.Values)
                    .ToListAsync();
            }

            private static async Task SeedMediaAsync(
                ApplicationDbContext context,
                List<ItemModel> items,
                Dictionary<string, SystemUserModel> users,
                Dictionary<string, PropertyModel> properties)
            {
                var selectedItems = items.Take(8).ToList();

                foreach (var item in selectedItems)
                {
                    if (await context.Media.AnyAsync(m => m.ItemId == item.Id && m.CreatedBy == SeedUser))
                    {
                        continue;
                    }

                    var title = item.Values.FirstOrDefault(v => v.PropertyId == properties["title"].Id)?.ValueText ?? $"item-{item.Id}";
                    var safeName = title.ToLowerInvariant().Replace(" ", "-").Replace(":", string.Empty);
                    var media = new MediaModel
                    {
                        Type = "Media",
                        ItemId = item.Id,
                        OwnerId = item.OwnerId ?? users["seed-librarian-2"].Id,
                        StoragePath = $"/uploads/sample/{safeName}.pdf",
                        FileName = $"{safeName}.pdf",
                        MimeType = "application/pdf",
                        FileSize = 256_000 + item.Id * 1_024,
                        AltText = $"Sample digital preview for {title}",
                        CreatedBy = SeedUser
                    };

                    await context.Media.AddAsync(media);
                    await context.SaveChangesAsync();

                    await AddValuesAsync(context, media.Id, properties, new Dictionary<string, string?>
                    {
                        ["title"] = $"{title} - Digital file",
                        ["format"] = "PDF",
                        ["description"] = $"Attached development media for {title}.",
                        ["rights"] = "Sample file for development use"
                    }, "en");
                }
            }

            private static async Task SeedItemSetsAsync(
                ApplicationDbContext context,
                List<ItemModel> items,
                Dictionary<string, SystemUserModel> users)
            {
                var sets = new[]
                {
                    new { Title = "Featured Arabic Collection", Description = "Arabic books, articles, and manuscripts for UI testing.", Owner = "seed-librarian-1", IsPublic = true, Count = 4 },
                    new { Title = "Digital Library Starters", Description = "Resources useful for digital archive screens.", Owner = "seed-librarian-2", IsPublic = true, Count = 5 },
                    new { Title = "Research Desk", Description = "Academic and research-oriented materials.", Owner = "seed-researcher-1", IsPublic = false, Count = 3 },
                    new { Title = "Recently Digitized", Description = "Items with sample media attached.", Owner = "seed-librarian-2", IsPublic = true, Count = 6 }
                };

                for (var index = 0; index < sets.Length; index++)
                {
                    var seed = sets[index];
                    if (await context.ItemSets.AnyAsync(s => s.Title == seed.Title))
                    {
                        continue;
                    }

                    var set = new ItemSetModel
                    {
                        Type = "ItemSet",
                        Title = seed.Title,
                        Description = seed.Description,
                        OwnerId = users[seed.Owner].Id,
                        IsPublic = seed.IsPublic,
                        CreatedBy = SeedUser
                    };

                    foreach (var item in items.Skip(index).Take(seed.Count))
                    {
                        set.Items.Add(item);
                    }

                    await context.ItemSets.AddAsync(set);
                }

                await context.SaveChangesAsync();
            }

            private static async Task AddValuesAsync(
                ApplicationDbContext context,
                int resourceId,
                Dictionary<string, PropertyModel> properties,
                Dictionary<string, string?> values,
                string language)
            {
                foreach (var (key, valueText) in values)
                {
                    if (string.IsNullOrWhiteSpace(valueText) || !properties.TryGetValue(key, out var property))
                    {
                        continue;
                    }

                    if (await context.Values.AnyAsync(v =>
                        v.ResourceId == resourceId &&
                        v.PropertyId == property.Id &&
                        v.ValueText == valueText))
                    {
                        continue;
                    }

                    await context.Values.AddAsync(new ValueModel
                    {
                        ResourceId = resourceId,
                        PropertyId = property.Id,
                        ValueText = valueText,
                        Type = "literal",
                        Language = language,
                        CreatedBy = SeedUser
                    });
                }

                await context.SaveChangesAsync();
            }

            private static SeedItem Item(
                string template,
                string owner,
                string title,
                string creator,
                string publisher,
                string date,
                string language,
                string identifier,
                string genre,
                string callNumber,
                string shelf,
                string availability)
            {
                return new SeedItem(template, owner, title, creator, publisher, date, language, identifier, genre, callNumber, shelf, availability);
            }

            private sealed record SeedItem(
                string Template,
                string Owner,
                string Title,
                string Creator,
                string Publisher,
                string Date,
                string Language,
                string Identifier,
                string Genre,
                string CallNumber,
                string Shelf,
                string Availability);
        }
    }
}
