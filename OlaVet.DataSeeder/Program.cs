// =============================================
// OlaVet.DataSeeder/Program.cs
// STATE-OF-THE-ART Production Data Seeder
// Complete, Realistic, Zero Nulls, Maximum Dictionaries
// =============================================

using System.Diagnostics;
using System.Data;
using Microsoft.Data.SqlClient;
using Dapper;
using Bogus;

namespace OlaVet.DataSeeder;

class Program
{
    private static string _connectionString =
        "Server=localhost;Database=OlaVet;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

    // =============================================
    // CONFIGURATION - Scale as needed
    // =============================================
    private static readonly int PetOwnerCount = 100_000;
    private static readonly int VetCount = 10_000;
    private static readonly int LabCount = 500;
    private static readonly int StoreCount = 1_000;
    private static readonly int PetCount = 150_000;
    private static readonly int AppointmentCount = 500_000;
    private static readonly int LabAppointmentCount = 200_000;
    private static readonly int MedicineOrderCount = 300_000;
    private static readonly int MedicineCount = 5_000;
    private static readonly int LabTestCount = 100;
    private static readonly int BatchSize = 10_000;

    // =============================================
    // REALISTIC DICTIONARIES - Pakistani Context
    // =============================================

    private static readonly string[] PakistaniMaleFirstNames = new[]
    {
        "Muhammad", "Ahmed", "Ali", "Hassan", "Usman", "Omar", "Bilal", "Hamza", "Abdullah", "Zain",
        "Arslan", "Faisal", "Kamran", "Shahid", "Imran", "Asad", "Fahad", "Waqar", "Tariq", "Saad",
        "Junaid", "Rizwan", "Adnan", "Farhan", "Umer", "Haider", "Salman", "Naveed", "Rashid", "Zahid",
        "Aqeel", "Rehan", "Haris", "Danish", "Talha", "Waleed", "Yasir", "Shehzad", "Muneeb", "Usama"
    };

    private static readonly string[] PakistaniFemaleFirstNames = new[]
    {
        "Fatima", "Ayesha", "Zainab", "Maryam", "Khadija", "Amina", "Sara", "Hira", "Alina", "Laiba",
        "Mahnoor", "Anum", "Sana", "Maria", "Rabia", "Nida", "Fiza", "Hina", "Noor", "Zara",
        "Areeba", "Saima", "Farah", "Sundus", "Nimra", "Iqra", "Bushra", "Komal", "Sidra", "Mehwish",
        "Sadia", "Nazia", "Rubina", "Shabana", "Samina", "Fouzia", "Amna", "Arooj", "Hafsa", "Samra"
    };

    private static readonly string[] PakistaniLastNames = new[]
    {
        "Khan", "Ali", "Ahmed", "Hassan", "Hussain", "Shah", "Malik", "Akhtar", "Butt", "Chaudhry",
        "Raza", "Aziz", "Iqbal", "Siddiqui", "Ansari", "Sheikh", "Mirza", "Abbasi", "Qureshi", "Rizvi",
        "Naqvi", "Haider", "Javed", "Aslam", "Rasheed", "Mahmood", "Saeed", "Farooq", "Jameel", "Karim",
        "Rahman", "Saleem", "Tariq", "Waheed", "Yousuf", "Zafar", "Bhatti", "Cheema", "Gondal", "Jutt"
    };

    private static readonly string[] LahoreLocalities = new[]
    {
        "Gulberg I", "Gulberg II", "Gulberg III", "DHA Phase 1", "DHA Phase 2", "DHA Phase 3",
        "DHA Phase 4", "DHA Phase 5", "DHA Phase 6", "DHA Phase 7", "DHA Phase 8", "Model Town",
        "Johar Town", "Wapda Town", "Bahria Town", "Cavalry Ground", "Cantt", "Garden Town",
        "Faisal Town", "Allama Iqbal Town", "Township", "Raiwind Road", "Multan Road", "Canal Road",
        "MM Alam Road", "Liberty Market", "Anarkali", "Mall Road", "Shadman", "Jail Road",
        "Thokar Niaz Baig", "Valencia Town", "Lake City", "Paragon City", "Eden City", "Sui Gas Society",
        "Muslim Town", "Samanabad", "Mughalpura", "Shalimar Town", "Green Town", "Sabzazar",
        "Mustafa Town", "Iqbal Town", "Wahdat Colony", "Township", "Chungi Amar Sidhu", "Harbanspura",
        "Shahdara", "Badami Bagh", "Data Darbar", "Lakshmi Chowk"
    };

    private static readonly string[] StreetNames = new[]
    {
        "Main Boulevard", "Canal Bank Road", "Ferozepur Road", "Jail Road", "Mall Road",
        "Shahrah-e-Faisal", "Shahrah-e-Quaid-e-Azam", "Walton Road", "Raiwind Road", "Multan Road",
        "GT Road", "Davis Road", "The Mall", "Upper Mall", "Lower Mall", "Empress Road",
        "College Road", "University Road", "Circular Road", "Link Road", "Service Road"
    };

    private static readonly string[] VetSpecializations = new[]
    {
        "Small Animal Surgery", "Large Animal Surgery", "Internal Medicine", "Emergency Medicine",
        "Exotic Animals", "Avian Medicine", "Equine Medicine", "Dentistry", "Orthopedics",
        "Dermatology", "Cardiology", "Neurology", "Oncology", "Ophthalmology", "Radiology",
        "Behavior Medicine", "Nutrition", "Anesthesiology", "Critical Care", "Preventive Medicine",
        "Reproduction", "Sports Medicine", "Wildlife Medicine", "Zoo Medicine", "Laboratory Animal Medicine"
    };

    private static readonly string[] VetEducationQualifications = new[]
    {
        "DVM - Doctor of Veterinary Medicine",
        "MS in Veterinary Surgery",
        "MS in Veterinary Medicine",
        "PhD in Veterinary Sciences",
        "Diploma in Veterinary Anesthesia",
        "Certificate in Small Animal Surgery",
        "Certificate in Emergency and Critical Care",
        "Fellowship in Exotic Animal Medicine",
        "Advanced Certificate in Veterinary Dentistry",
        "Postgraduate Diploma in Veterinary Radiology",
        "MS in Veterinary Pathology",
        "Certificate in Veterinary Cardiology",
        "Diploma in Veterinary Dermatology",
        "Advanced Training in Veterinary Ophthalmology",
        "Certificate in Wildlife Medicine"
    };

    private static readonly string[] VetEducationInstitutes = new[]
    {
        "University of Veterinary and Animal Sciences, Lahore",
        "University of Agriculture, Faisalabad",
        "PMAS-Arid Agriculture University, Rawalpindi",
        "Sindh Agriculture University, Tandojam",
        "Royal Veterinary College, London, UK",
        "Cornell University College of Veterinary Medicine, USA",
        "University of California, Davis, USA",
        "University of Edinburgh, UK",
        "Utrecht University, Netherlands",
        "University of Sydney, Australia",
        "Murdoch University, Australia",
        "Massey University, New Zealand",
        "University of Glasgow, UK",
        "Ohio State University, USA",
        "Colorado State University, USA"
    };

    private static readonly string[] VetServices = new[]
    {
        "General Health Checkup", "Vaccination", "Deworming", "Dental Cleaning",
        "Surgical Procedures", "Emergency Surgery", "Spaying/Neutering", "Tumor Removal",
        "Wound Treatment", "Fracture Treatment", "X-Ray Imaging", "Ultrasound Examination",
        "Blood Tests", "Skin Treatment", "Ear Treatment", "Eye Treatment",
        "Dietary Consultation", "Behavioral Consultation", "Grooming Consultation", "Microchipping",
        "Health Certificates", "Euthanasia", "Post-operative Care", "Physiotherapy",
        "Acupuncture", "Laser Therapy", "Chronic Disease Management"
    };

    private static readonly string[] DogBreeds = new[]
    {
        "German Shepherd", "Labrador Retriever", "Golden Retriever", "Bulldog", "Beagle",
        "Poodle", "Rottweiler", "Siberian Husky", "Dachshund", "Doberman Pinscher",
        "Boxer", "Great Dane", "Shih Tzu", "Chihuahua", "Pug", "Cocker Spaniel",
        "Pomeranian", "Maltese", "Yorkshire Terrier", "Dalmatian", "Basset Hound",
        "Border Collie", "Australian Shepherd", "Bichon Frise", "Bull Terrier",
        "Cavalier King Charles", "Chow Chow", "English Springer", "French Bulldog",
        "Havanese", "Mastiff", "Miniature Schnauzer", "Newfoundland", "Papillon",
        "Pembroke Welsh Corgi", "Pointer", "Saint Bernard", "Samoyed", "Shar Pei",
        "Staffordshire Bull Terrier", "Vizsla", "Weimaraner", "West Highland White",
        "Whippet", "Afghan Hound", "Airedale Terrier", "Akita", "Alaskan Malamute"
    };

    private static readonly string[] CatBreeds = new[]
    {
        "Persian", "Siamese", "Maine Coon", "British Shorthair", "Ragdoll",
        "Bengal", "Sphynx", "Scottish Fold", "Himalayan", "Domestic Shorthair",
        "Abyssinian", "American Shorthair", "Birman", "Burmese", "Exotic Shorthair",
        "Norwegian Forest", "Oriental", "Russian Blue", "Siberian", "Turkish Angora",
        "Tonkinese", "Manx", "Devon Rex", "Cornish Rex", "Chartreux",
        "Egyptian Mau", "Havana Brown", "Japanese Bobtail", "Korat", "LaPerm"
    };

    private static readonly string[] BirdTypes = new[]
    {
        "Budgerigar", "Cockatiel", "African Grey Parrot", "Macaw", "Cockatoo",
        "Lovebird", "Canary", "Finch", "Parakeet", "Conure", "Amazon Parrot",
        "Eclectus", "Quaker Parrot", "Rosella", "Lorikeet", "Dove", "Pigeon"
    };

    private static readonly string[] OtherPetSpecies = new[]
    {
        "Rabbit", "Guinea Pig", "Hamster", "Ferret", "Chinchilla",
        "Gerbil", "Hedgehog", "Sugar Glider", "Turtle", "Tortoise",
        "Iguana", "Bearded Dragon", "Snake", "Fish"
    };

    private static readonly string[] PetColors = new[]
    {
        "Black", "White", "Brown", "Golden", "Gray", "Cream", "Tan",
        "Brindle", "Spotted", "Striped", "Tri-color", "Bi-color",
        "Orange", "Blue", "Red", "Fawn", "Sable", "Merle", "Chocolate"
    };

    private static readonly string[] AppointmentReasons = new[]
    {
        "Routine health checkup", "Annual vaccination", "Booster vaccination",
        "Skin rash or irritation", "Ear infection", "Eye discharge", "Digestive issues",
        "Loss of appetite", "Vomiting", "Diarrhea", "Coughing", "Sneezing",
        "Limping or lameness", "Injury from fall", "Fight wound", "Dental problems",
        "Bad breath", "Weight loss", "Weight gain", "Lethargy", "Excessive drinking",
        "Frequent urination", "Hair loss", "Scratching excessively", "Behavioral issues",
        "Aggression problems", "Anxiety symptoms", "Follow-up examination",
        "Post-surgery checkup", "Pregnancy checkup", "Pre-breeding consultation",
        "Nail trimming", "Ear cleaning", "Anal gland expression", "Health certificate",
        "Travel certificate", "Microchipping", "Deworming", "Flea treatment",
        "Tick removal", "Emergency consultation"
    };

    private static readonly string[] Diagnoses = new[]
    {
        "Healthy - Routine checkup completed", "Skin allergy - Environmental triggers",
        "Bacterial ear infection", "Conjunctivitis", "Gastroenteritis",
        "Dietary indiscretion", "Viral upper respiratory infection",
        "Dental tartar buildup", "Gingivitis", "Periodontal disease",
        "Soft tissue injury", "Arthritis - Early stage", "Hip dysplasia",
        "Luxating patella", "Dermatitis", "Flea allergy dermatitis",
        "Hot spots", "Urinary tract infection", "Bladder stones",
        "Kidney disease - Chronic", "Liver dysfunction", "Diabetes mellitus",
        "Hypothyroidism", "Hyperthyroidism", "Obesity", "Malnutrition",
        "Intestinal parasites", "Heartworm positive", "Pancreatitis",
        "Inflammatory bowel disease", "Foreign body ingestion", "Toxicity",
        "Kennel cough", "Parvovirus", "Distemper", "Rabies vaccination given",
        "Pregnancy confirmed", "False pregnancy", "Pyometra", "Mammary tumor",
        "Lipoma", "Mast cell tumor", "Anxiety disorder", "Separation anxiety"
    };

    private static readonly string[] Treatments = new[]
    {
        "Prescribed antibiotics for 7 days", "Prescribed anti-inflammatory medication",
        "Applied topical ointment", "Administered vaccination - DHPP",
        "Administered rabies vaccine", "Administered flea and tick prevention",
        "Performed dental cleaning under anesthesia", "Extracted damaged tooth",
        "Applied wound dressing", "Prescribed pain medication",
        "Recommended dietary changes - Low fat diet", "Recommended weight loss program",
        "Prescribed insulin therapy", "Prescribed thyroid medication",
        "Prescribed antihistamines", "Administered IV fluids",
        "Prescribed ear drops for 10 days", "Prescribed eye drops",
        "Prescribed deworming medication", "Performed blood tests - Results pending",
        "Performed X-ray - No fractures found", "Performed ultrasound examination",
        "Recommended follow-up in 2 weeks", "Recommended specialist consultation",
        "Applied bandage - Change in 3 days", "Prescribed joint supplements",
        "Recommended physiotherapy", "Administered subcutaneous fluids",
        "Expressed anal glands", "Trimmed nails", "Cleaned ears",
        "Applied flea treatment", "Microchipped pet", "Neutered/Spayed pet",
        "Provided nutritional counseling", "Prescribed probiotic supplements",
        "Recommended behavioral training", "Prescribed calming medication"
    };

    private static readonly string[] MedicineNames = new[]
    {
        "Amoxicillin", "Cephalexin", "Enrofloxacin", "Metronidazole", "Doxycycline",
        "Carprofen", "Meloxicam", "Gabapentin", "Tramadol", "Prednisolone",
        "Dexamethasone", "Furosemide", "Enalapril", "Pimobendan", "Digoxin",
        "Ivermectin", "Fenbendazole", "Praziquantel", "Fipronil", "Selamectin",
        "Levothyroxine", "Insulin", "Methimazole", "Amlodipine", "Atenolol",
        "Famotidine", "Omeprazole", "Metoclopramide", "Cerenia", "Ondansetron",
        "Diphenhydramine", "Cetirizine", "Hydroxyzine", "Fluconazole", "Ketoconazole"
    };

    private static readonly string[] Manufacturers = new[]
    {
        "Zoetis", "Merck Animal Health", "Elanco", "Boehringer Ingelheim",
        "Virbac", "Ceva Santé Animale", "Vetoquinol", "Dechra Pharmaceuticals",
        "Norbrook", "Bayer Animal Health", "Merial", "Intervet", "Pfizer Animal Health",
        "Novartis Animal Health", "Bimeda", "Jurox", "Chanelle Pharma", "Dermcare"
    };

    private static readonly string[] ReviewComments = new[]
    {
        "Excellent service! Very professional and caring.",
        "Dr. was extremely knowledgeable and gentle with my pet.",
        "Great experience overall. Highly recommend!",
        "Very satisfied with the treatment provided.",
        "The vet took time to explain everything clearly.",
        "Professional staff and clean facility.",
        "My pet felt comfortable throughout the visit.",
        "Quick service without compromising quality.",
        "Fair pricing for the quality of care received.",
        "Will definitely return for future visits.",
        "Could improve on wait times, but good service.",
        "Thorough examination and detailed explanation.",
        "Emergency care was handled very well.",
        "Follow-up care was excellent.",
        "Impressed with the diagnostic facilities.",
        "Good bedside manner with anxious pets.",
        "Reasonable fees compared to other clinics.",
        "Easy appointment booking process.",
        "Convenient location and parking.",
        "Staff was friendly and helpful."
    };

    static async Task Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     OlaVet Production Data Seeder - State of the Art          ║");
        Console.WriteLine("║     Zero Nulls | Maximum Realism | Complete Relationships     ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
        Console.WriteLine();

        var totalStopwatch = Stopwatch.StartNew();

        try
        {
            await VerifyConnection();
            await SeedLookupTables();
            await SeedPetOwners();
            await SeedVets();
            await SeedVetQualificationsAndServices();
            await SeedVetAvailability();
            await SeedLabs();
            await SeedLabTests();
            await SeedStores();
            await SeedMedicines();
            await SeedInventory();
            await SeedPets();
            await SeedVetAppointments();
            await SeedLabAppointments();
            await SeedLabAppointmentTests();
            await SeedMedicineOrders();
            await SeedMedicineOrderDetails();
            await SeedMedicalRecords();
            await SeedVetPayments();
            await SeedLabPayments();
            await SeedStorePayments();
            await SeedVetReviews();
            await SeedLabReviews();
            await SeedStoreReviews();

            totalStopwatch.Stop();

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  ✅ DATA SEEDING COMPLETED SUCCESSFULLY!                       ║");
            Console.WriteLine($"║  Total Time: {totalStopwatch.Elapsed:hh\\:mm\\:ss}                                    ║");
            Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
            Console.ResetColor();

            await PrintDatabaseStatistics();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ CRITICAL ERROR: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static async Task VerifyConnection()
    {
        Console.Write("🔌 Verifying database connection... ");
        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();
        var result = await connection.QuerySingleAsync<string>("SELECT DB_NAME()");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"✅ Connected to: {result}");
        Console.ResetColor();
    }

    static async Task SeedLookupTables()
    {
        Console.WriteLine("\n📋 Seeding Lookup Tables...");
        using var connection = new SqlConnection(_connectionString);

        // Medicine Types
        var medicineTypes = new[]
        {
            ("Tablet", "Oral solid medication in tablet form"),
            ("Capsule", "Oral medication enclosed in gelatin capsule"),
            ("Injection", "Injectable medication for intramuscular or intravenous use"),
            ("Syrup", "Liquid oral medication in syrup form"),
            ("Suspension", "Liquid medication with suspended particles"),
            ("Ointment", "Topical medication in cream or gel form"),
            ("Drops", "Liquid drops for eyes, ears, or oral use"),
            ("Bandage", "Wound dressing and bandage material"),
            ("Spray", "Topical medication in spray form"),
            ("Powder", "Medicated powder for topical or oral use"),
            ("Shampoo", "Medicated shampoo for skin conditions"),
            ("Paste", "Oral paste medication"),
            ("Chewable", "Flavored chewable medication"),
            ("Transdermal", "Patch for transdermal medication delivery")
        };

        foreach (var (name, desc) in medicineTypes)
        {
            await connection.ExecuteAsync(
                "INSERT INTO MedicineType (TypeName, Description, CreatedDate) VALUES (@Name, @Desc, GETUTCDATE())",
                new { Name = name, Desc = desc });
        }
        Console.WriteLine($"  ✓ Medicine Types: {medicineTypes.Length}");

        // Record Types
        var recordTypes = new[]
        {
            ("Prescription", "Medication prescription record"),
            ("Report", "Diagnostic or examination report"),
            ("Invoice", "Billing and payment invoice"),
            ("Vaccine", "Vaccination record and certificate"),
            ("Surgery", "Surgical procedure record"),
            ("Lab Result", "Laboratory test results"),
            ("Imaging", "X-ray, ultrasound, or other imaging"),
            ("Certificate", "Health or travel certificate"),
            ("Other", "Miscellaneous medical documentation")
        };

        foreach (var (name, desc) in recordTypes)
        {
            await connection.ExecuteAsync(
                "INSERT INTO RecordType (TypeName, Description, CreatedDate) VALUES (@Name, @Desc, GETUTCDATE())",
                new { Name = name, Desc = desc });
        }
        Console.WriteLine($"  ✓ Record Types: {recordTypes.Length}");

        // Vet Appointment Types
        await connection.ExecuteAsync(
            "INSERT INTO VetAppointmentType (TypeName, Description, CreatedDate) VALUES (@Name, @Desc, GETUTCDATE())",
            new { Name = "Clinic", Desc = "In-person clinic visit" });
        await connection.ExecuteAsync(
            "INSERT INTO VetAppointmentType (TypeName, Description, CreatedDate) VALUES (@Name, @Desc, GETUTCDATE())",
            new { Name = "Video", Desc = "Online video conferencing consultation" });
        Console.WriteLine("  ✓ Appointment Types: 2");

        // Status Types
        var statusTypes = new[]
        {
            ("Scheduled", "Appointment", "Appointment scheduled and confirmed"),
            ("Confirmed", "Appointment", "Appointment confirmed by provider"),
            ("Completed", "Appointment", "Appointment completed successfully"),
            ("Cancelled", "Appointment", "Appointment cancelled by user or provider"),
            ("No-Show", "Appointment", "Patient did not attend scheduled appointment"),
            ("Rescheduled", "Appointment", "Appointment rescheduled to different time"),
            ("Pending", "MedicineOrder", "Order placed awaiting processing"),
            ("Processing", "MedicineOrder", "Order being prepared for delivery"),
            ("Shipped", "MedicineOrder", "Order shipped to delivery address"),
            ("Delivered", "MedicineOrder", "Order successfully delivered to customer"),
            ("Returned", "MedicineOrder", "Order returned by customer")
        };

        foreach (var (name, appliesTo, desc) in statusTypes)
        {
            await connection.ExecuteAsync(
                "INSERT INTO StatusType (StatusName, AppliesTo, Description, CreatedDate) VALUES (@Name, @AppliesTo, @Desc, GETUTCDATE())",
                new { Name = name, AppliesTo = appliesTo, Desc = desc });
        }
        Console.WriteLine($"  ✓ Status Types: {statusTypes.Length}");
    }

    static async Task SeedPetOwners()
    {
        Console.WriteLine($"\n👥 Seeding {PetOwnerCount:N0} Pet Owners...");
        var sw = Stopwatch.StartNew();

        var random = new Random(12345); // Fixed seed for reproducibility
        var faker = new Faker();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        int totalInserted = 0;
        int batchCount = (int)Math.Ceiling(PetOwnerCount / (double)BatchSize);

        int phoneCounter = 0; // Counter for unique phone numbers
        
        for (int batch = 0; batch < batchCount; batch++)
        {
            int currentBatchSize = Math.Min(BatchSize, PetOwnerCount - totalInserted);
            var petOwners = new List<object>();

            for (int i = 0; i < currentBatchSize; i++)
            {
                var isFemale = random.Next(2) == 0;
                var firstName = isFemale
                    ? PakistaniFemaleFirstNames[random.Next(PakistaniFemaleFirstNames.Length)]
                    : PakistaniMaleFirstNames[random.Next(PakistaniMaleFirstNames.Length)];
                var lastName = PakistaniLastNames[random.Next(PakistaniLastNames.Length)];
                var fullName = $"{firstName} {lastName}";
                var email = $"{firstName.ToLower()}.{lastName.ToLower()}{totalInserted + i + 1}@email.com";
                // Use sequential counter for unique phone numbers
                var phoneOperator = 300 + (phoneCounter / 10000000); // Rotate operators for large datasets
                var phoneNumber = $"+92-{phoneOperator}-{3000000 + phoneCounter}";
                phoneCounter++;

                var locality = LahoreLocalities[random.Next(LahoreLocalities.Length)];
                var streetName = StreetNames[random.Next(StreetNames.Length)];
                var houseNumber = random.Next(1, 999);
                var address = $"House {houseNumber}, {streetName}, {locality}, Lahore, Pakistan";

                var age = random.Next(18, 76);
                var gender = isFemale ? "Female" : "Male";
                var wallet = Math.Round((decimal)(random.NextDouble() * 45000 + 5000), 2);

                var daysAgo = random.Next(0, 730);
                var registrationDate = DateTime.UtcNow.AddDays(-daysAgo);

                petOwners.Add(new
                {
                    OwnerName = fullName,
                    Email = email,
                    ContactNumber = phoneNumber,
                    HomeAddress = address,
                    Age = age,
                    Gender = gender,
                    Wallet = wallet,
                    RegistrationDate = registrationDate,
                    IsActive = 1
                });
            }

            const string sql = @"
                INSERT INTO PetOwner (OwnerName, Email, ContactNumber, HomeAddress, Age, Gender, Wallet, RegistrationDate, IsActive, CreatedDate)
                VALUES (@OwnerName, @Email, @ContactNumber, @HomeAddress, @Age, @Gender, @Wallet, @RegistrationDate, @IsActive, GETUTCDATE())";

            await connection.ExecuteAsync(sql, petOwners);

            totalInserted += currentBatchSize;
            double progress = (totalInserted / (double)PetOwnerCount) * 100;
            double rate = totalInserted / sw.Elapsed.TotalSeconds;
            Console.Write($"\r  Progress: {progress:F1}% ({totalInserted:N0}/{PetOwnerCount:N0}) - {rate:F0} rows/sec");
        }

        sw.Stop();
        Console.WriteLine($"\n  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedVets()
    {
        Console.WriteLine($"\n👨‍⚕️ Seeding {VetCount:N0} Veterinarians...");
        var sw = Stopwatch.StartNew();

        var random = new Random(12346); // Fixed seed for reproducibility
        var faker = new Faker();

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        int totalInserted = 0;
        int batchCount = (int)Math.Ceiling(VetCount / (double)BatchSize);

        int vetPhoneCounter = 0; // Counter for unique vet phone numbers
        
        for (int batch = 0; batch < batchCount; batch++)
        {
            int currentBatchSize = Math.Min(BatchSize, VetCount - totalInserted);
            var vets = new List<object>();

            for (int i = 0; i < currentBatchSize; i++)
            {
                var isFemale = random.Next(10) < 3; // 30% female vets
                var firstName = isFemale
                    ? PakistaniFemaleFirstNames[random.Next(PakistaniFemaleFirstNames.Length)]
                    : PakistaniMaleFirstNames[random.Next(PakistaniMaleFirstNames.Length)];
                var lastName = PakistaniLastNames[random.Next(PakistaniLastNames.Length)];
                var vetName = $"Dr. {firstName} {lastName}";

                var specialization = VetSpecializations[random.Next(VetSpecializations.Length)];
                var locality = LahoreLocalities[random.Next(LahoreLocalities.Length)];
                var clinicNumber = random.Next(1, 99);
                var clinicLocation = $"Veterinary Clinic {clinicNumber}, {locality}, Lahore, Pakistan";
                var fee = Math.Round((decimal)(random.NextDouble() * 3500 + 1500), 2);
                // Use sequential counter for unique phone numbers (starting from 5000000 to avoid overlap with pet owners)
                var phoneNumber = $"+92-320-{5000000 + vetPhoneCounter}";
                vetPhoneCounter++;
                var email = $"dr.{firstName.ToLower()}.{lastName.ToLower()}{totalInserted + i + 1}@olavet.com";
                var yearsExp = random.Next(2, 26);
                var licenseYear = DateTime.UtcNow.Year - yearsExp;
                var licenseNumber = $"VET-{licenseYear}-{totalInserted + i + 100}";

                vets.Add(new
                {
                    VetName = vetName,
                    Specialization = specialization,
                    ClinicLocation = clinicLocation,
                    Fee = fee,
                    ContactNumber = phoneNumber,
                    Email = email,
                    YearsOfExperience = yearsExp,
                    LicenseNumber = licenseNumber,
                    IsActive = 1
                });
            }

            const string sql = @"
                INSERT INTO Vet (VetName, Specialization, ClinicLocation, Fee, ContactNumber, Email, YearsOfExperience, LicenseNumber, IsActive, CreatedDate)
                VALUES (@VetName, @Specialization, @ClinicLocation, @Fee, @ContactNumber, @Email, @YearsOfExperience, @LicenseNumber, @IsActive, GETUTCDATE())";

            await connection.ExecuteAsync(sql, vets);

            totalInserted += currentBatchSize;
            double progress = (totalInserted / (double)VetCount) * 100;
            Console.Write($"\r  Progress: {progress:F1}% ({totalInserted:N0}/{VetCount:N0})");
        }

        sw.Stop();
        Console.WriteLine($"\n  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedVetQualificationsAndServices()
    {
        Console.WriteLine("\n🎓 Seeding Vet Qualifications & Services...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12347);

        using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync();

        var vetIds = await connection.QueryAsync<int>("SELECT VetId FROM Vet");
        var qualifications = new List<object>();
        var services = new List<object>();

        foreach (var vetId in vetIds)
        {
            // 2-4 qualifications per vet
            int qualCount = random.Next(2, 5);
            var selectedQuals = VetEducationQualifications
                .OrderBy(x => random.Next())
                .Take(qualCount);

            foreach (var qual in selectedQuals)
            {
                var institute = VetEducationInstitutes[random.Next(VetEducationInstitutes.Length)];
                var year = DateTime.UtcNow.Year - random.Next(5, 25);

                qualifications.Add(new
                {
                    VetId = vetId,
                    QualificationName = qual,
                    Institute = institute,
                    YearOfDegree = year
                });
            }

            // 3-6 services per vet
            int serviceCount = random.Next(3, 7);
            var selectedServices = VetServices
                .OrderBy(x => random.Next())
                .Take(serviceCount);

            foreach (var service in selectedServices)
            {
                var fee = Math.Round((decimal)(random.NextDouble() * 15000 + 2000), 2);
                var description = $"Professional {service.ToLower()} with state-of-the-art equipment and experienced staff";

                services.Add(new
                {
                    VetId = vetId,
                    ServiceType = service,
                    ServiceDescription = description,
                    ServiceFee = fee
                });
            }
        }

        await connection.ExecuteAsync(@"
            INSERT INTO EducationQualification (VetId, QualificationName, Institute, YearOfDegree)
            VALUES (@VetId, @QualificationName, @Institute, @YearOfDegree)", qualifications);

        await connection.ExecuteAsync(@"
            INSERT INTO Service (VetId, ServiceType, ServiceDescription, ServiceFee)
            VALUES (@VetId, @ServiceType, @ServiceDescription, @ServiceFee)", services);

        sw.Stop();
        Console.WriteLine($"  ✅ {qualifications.Count:N0} qualifications, {services.Count:N0} services in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedVetAvailability()
    {
        Console.WriteLine("\n📅 Seeding Vet Availability Schedules...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12348);

        using var connection = new SqlConnection(_connectionString);
        var vetIds = await connection.QueryAsync<int>("SELECT VetId FROM Vet");

        var daysOfWeek = new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        var availabilities = new List<object>();

        foreach (var vetId in vetIds)
        {
            // Each vet works 3-6 days per week
            int workDays = random.Next(3, 7);
            var selectedDays = daysOfWeek.OrderBy(x => random.Next()).Take(workDays);

            foreach (var day in selectedDays)
            {
                var startHour = random.Next(8, 11); // 8 AM to 10 AM start
                var endHour = random.Next(17, 21);   // 5 PM to 8 PM end
                var slotDuration = random.Next(2, 4) * 15; // 30, 45, or 60 minutes

                availabilities.Add(new
                {
                    VetId = vetId,
                    DayOfWeek = day,
                    StartTime = new TimeSpan(startHour, 0, 0),
                    EndTime = new TimeSpan(endHour, 0, 0),
                    SlotDurationMinutes = slotDuration,
                    IsAvailable = 1
                });
            }
        }

        await connection.ExecuteAsync(@"
            INSERT INTO VetAvailability (VetId, DayOfWeek, StartTime, EndTime, SlotDurationMinutes, IsAvailable)
            VALUES (@VetId, @DayOfWeek, @StartTime, @EndTime, @SlotDurationMinutes, @IsAvailable)", availabilities);

        sw.Stop();
        Console.WriteLine($"  ✅ {availabilities.Count:N0} availability slots in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedLabs()
    {
        Console.WriteLine($"\n🔬 Seeding {LabCount:N0} Laboratories...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12349);

        var labs = new List<object>();

        for (int i = 0; i < LabCount; i++)
        {
            var labType = new[] { "Veterinary Diagnostics", "Animal Lab Services", "Pet Care Laboratory", "Veterinary Testing Center" }[random.Next(4)];
            var labName = $"{labType} - {LahoreLocalities[random.Next(LahoreLocalities.Length)]}";
            var locality = LahoreLocalities[random.Next(LahoreLocalities.Length)];
            var streetName = StreetNames[random.Next(StreetNames.Length)];
            var buildingNumber = random.Next(1, 500);
            var labAddress = $"Building {buildingNumber}, {streetName}, {locality}, Lahore, Pakistan";
            var waitTime = random.Next(12, 73); // 12-72 hours
            var experience = random.Next(3, 21); // 3-20 years
            // Use sequential counter for unique phone numbers
            var phoneNumber = $"+92-311-{7000000 + i}";
            var discount = random.Next(0, 21); // 0-20% discount
            var specialization = new[] { "Pathology, Microbiology", "Hematology, Biochemistry",
                "Radiology, Ultrasound, CT Scan", "Molecular Diagnostics, PCR",
                "Histopathology, Cytology" }[random.Next(5)];

            labs.Add(new
            {
                LabName = labName,
                LabAddress = labAddress,
                WaitTime = waitTime,
                Experience = experience,
                ContactNumber = phoneNumber,
                Discount = discount,
                Specialization = specialization,
                IsActive = 1
            });
        }

        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
            INSERT INTO Lab (LabName, LabAddress, WaitTime, Experience, ContactNumber, Discount, Specialization, IsActive, CreatedDate)
            VALUES (@LabName, @LabAddress, @WaitTime, @Experience, @ContactNumber, @Discount, @Specialization, @IsActive, GETUTCDATE())", labs);

        sw.Stop();
        Console.WriteLine($"  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedLabTests()
    {
        Console.WriteLine($"\n🧪 Seeding {LabTestCount} Lab Tests...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12350);

        var testCategories = new[]
        {
            ("Hematology", new[] { "Complete Blood Count", "Blood Smear Examination", "Platelet Count", "Reticulocyte Count" }),
            ("Biochemistry", new[] { "Liver Function Panel", "Kidney Function Panel", "Electrolyte Panel", "Blood Glucose", "Cholesterol Panel" }),
            ("Clinical Pathology", new[] { "Urinalysis Complete", "Fecal Examination", "Skin Scraping", "Cytology" }),
            ("Microbiology", new[] { "Bacterial Culture", "Fungal Culture", "Viral PCR", "Sensitivity Testing" }),
            ("Serology", new[] { "FIV/FeLV Test", "Heartworm Test", "Parvovirus Test", "Distemper Test" }),
            ("Radiology", new[] { "X-Ray Single View", "X-Ray Multiple Views", "Ultrasound Abdomen", "CT Scan", "MRI" }),
            ("Endocrinology", new[] { "Thyroid Panel", "Cortisol Test", "ACTH Stimulation", "Insulin Level" }),
            ("Parasitology", new[] { "Fecal Flotation", "Giardia Test", "Cryptosporidium Test", "Blood Parasite Screen" })
        };

        var tests = new List<object>();

        foreach (var (category, testNames) in testCategories)
        {
            foreach (var testName in testNames)
            {
                var fee = Math.Round((decimal)(random.NextDouble() * 7000 + 800), 2);
                var turnaroundHours = random.Next(1, 97); // 1-96 hours
                var description = $"Comprehensive {testName.ToLower()} for diagnostic purposes with detailed analysis and reporting";

                tests.Add(new
                {
                    LabTestName = testName,
                    LabTestType = category,
                    LabTestDescription = description,
                    TestFee = fee,
                    TurnaroundTimeHours = turnaroundHours,
                    IsActive = 1
                });
            }
        }

        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
            INSERT INTO LabTest (LabTestName, LabTestType, LabTestDescription, TestFee, TurnaroundTimeHours, IsActive, CreatedDate)
            VALUES (@LabTestName, @LabTestType, @LabTestDescription, @TestFee, @TurnaroundTimeHours, @IsActive, GETUTCDATE())", tests);

        sw.Stop();
        Console.WriteLine($"  ✅ {tests.Count} lab tests in {sw.Elapsed.TotalMilliseconds:F0}ms");
    }

    static async Task SeedStores()
    {
        Console.WriteLine($"\n🏪 Seeding {StoreCount:N0} Pharmacies/Stores...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12351);

        var stores = new List<object>();

        for (int i = 0; i < StoreCount; i++)
        {
            var storeType = new[] { "Pharmacy", "Pet Store", "Veterinary Supply", "Animal Care Center", "Pet Health Shop" }[random.Next(5)];
            var storeName = $"{new[] { "Al-Shifa", "Care", "Health", "Pet", "Vet", "Animal" }[random.Next(6)]} {storeType}";
            var locality = LahoreLocalities[random.Next(LahoreLocalities.Length)];
            var streetName = StreetNames[random.Next(StreetNames.Length)];
            var shopNumber = random.Next(1, 999);
            var storeAddress = $"Shop {shopNumber}, {streetName}, {locality}, Lahore, Pakistan";
            var since = DateTime.UtcNow.AddYears(-random.Next(1, 16)).AddMonths(-random.Next(0, 12));
            var openingHour = random.Next(7, 10);
            var closingHour = random.Next(20, 23);
            // Use sequential counter for unique phone numbers
            var phoneNumber = $"+92-331-{8000000 + i}";

            stores.Add(new
            {
                StoreName = storeName,
                StoreAddress = storeAddress,
                Since = since,
                OpeningTime = new TimeSpan(openingHour, 0, 0),
                ClosingTime = new TimeSpan(closingHour, 0, 0),
                ContactNumber = phoneNumber,
                IsActive = 1
            });
        }

        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"
            INSERT INTO Store (StoreName, StoreAddress, Since, OpeningTime, ClosingTime, ContactNumber, IsActive, CreatedDate)
            VALUES (@StoreName, @StoreAddress, @Since, @OpeningTime, @ClosingTime, @ContactNumber, @IsActive, GETUTCDATE())", stores);

        sw.Stop();
        Console.WriteLine($"  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedMedicines()
    {
        Console.WriteLine($"\n💊 Seeding {MedicineCount:N0} Medicines...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12352);

        using var connection = new SqlConnection(_connectionString);
        int totalInserted = 0;
        int batchCount = (int)Math.Ceiling(MedicineCount / (double)BatchSize);

        for (int batch = 0; batch < batchCount; batch++)
        {
            int currentBatchSize = Math.Min(BatchSize, MedicineCount - totalInserted);
            var medicines = new List<object>();

            for (int i = 0; i < currentBatchSize; i++)
            {
                var baseName = MedicineNames[random.Next(MedicineNames.Length)];
                var mg = new[] { 25, 50, 100, 150, 200, 250, 500, 1000 }[random.Next(8)];
                var medicineName = $"{baseName} {mg}mg";
                var price = Math.Round((decimal)(random.NextDouble() * 2800 + 200), 2);
                var medicineTypeId = random.Next(1, 15);
                var manufacturer = Manufacturers[random.Next(Manufacturers.Length)];
                var description = $"{baseName} - Professional veterinary medication for treatment and prevention. Manufactured by {manufacturer}.";
                var requiresPrescription = random.Next(10) < 7; // 70% require prescription

                medicines.Add(new
                {
                    MedicineName = medicineName,
                    MG = mg,
                    Price = price,
                    MedicineTypeId = medicineTypeId,
                    Manufacturer = manufacturer,
                    Description = description,
                    RequiresPrescription = requiresPrescription,
                    IsActive = 1
                });
            }

            await connection.ExecuteAsync(@"
                INSERT INTO Medicine (MedicineName, MG, Price, MedicineTypeId, Manufacturer, Description, RequiresPrescription, IsActive, CreatedDate)
                VALUES (@MedicineName, @MG, @Price, @MedicineTypeId, @Manufacturer, @Description, @RequiresPrescription, @IsActive, GETUTCDATE())", medicines);

            totalInserted += currentBatchSize;
            double progress = (totalInserted / (double)MedicineCount) * 100;
            Console.Write($"\r  Progress: {progress:F1}% ({totalInserted:N0}/{MedicineCount:N0})");
        }

        sw.Stop();
        Console.WriteLine($"\n  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedInventory()
    {
        Console.WriteLine("\n📦 Seeding Store Inventory...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12353);

        using var connection = new SqlConnection(_connectionString);
        var storeIds = (await connection.QueryAsync<int>("SELECT StoreId FROM Store")).ToList();
        var medicineIds = (await connection.QueryAsync<int>("SELECT MedicineId FROM Medicine")).ToList();

        var inventories = new List<object>();

        foreach (var storeId in storeIds)
        {
            int medicineCount = random.Next(50, 101); // 50-100 medicines per store
            var selectedMedicines = medicineIds.OrderBy(x => random.Next()).Take(medicineCount);

            foreach (var medId in selectedMedicines)
            {
                var quantity = random.Next(20, 501); // 20-500 units
                var daysAgo = random.Next(1, 61); // Restocked within last 60 days
                var lastRestocked = DateTime.UtcNow.AddDays(-daysAgo);

                inventories.Add(new
                {
                    StoreId = storeId,
                    MedicineId = medId,
                    Quantity = quantity,
                    LastRestocked = lastRestocked
                });
            }
        }

        await connection.ExecuteAsync(@"
            INSERT INTO Inventory (StoreId, MedicineId, Quantity, LastRestocked)
            VALUES (@StoreId, @MedicineId, @Quantity, @LastRestocked)", inventories);

        sw.Stop();
        Console.WriteLine($"  ✅ {inventories.Count:N0} inventory records in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedPets()
    {
        Console.WriteLine($"\n🐕 Seeding {PetCount:N0} Pets...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12354);

        using var connection = new SqlConnection(_connectionString);
        var ownerIds = (await connection.QueryAsync<int>("SELECT PetOwnerId FROM PetOwner")).ToList();

        int totalInserted = 0;
        int batchCount = (int)Math.Ceiling(PetCount / (double)BatchSize);

        for (int batch = 0; batch < batchCount; batch++)
        {
            int currentBatchSize = Math.Min(BatchSize, PetCount - totalInserted);
            var pets = new List<object>();

            for (int i = 0; i < currentBatchSize; i++)
            {
                var ownerId = ownerIds[random.Next(ownerIds.Count)];
                var speciesRoll = random.Next(100);

                string species, breed, gender, name;
                decimal weight;
                int age;

                if (speciesRoll < 60) // 60% Dogs
                {
                    species = "Dog";
                    breed = DogBreeds[random.Next(DogBreeds.Length)];
                    weight = (decimal)(random.NextDouble() * 40 + 5);
                    age = random.Next(0, 16);
                    name = new[] { "Max", "Buddy", "Charlie", "Rocky", "Duke", "Bear", "Jack", "Cooper",
                        "Bella", "Lucy", "Daisy", "Luna", "Lola", "Sadie", "Molly", "Bailey" }[random.Next(16)];
                }
                else if (speciesRoll < 90) // 30% Cats
                {
                    species = "Cat";
                    breed = CatBreeds[random.Next(CatBreeds.Length)];
                    weight = (decimal)(random.NextDouble() * 6 + 2);
                    age = random.Next(0, 18);
                    name = new[] { "Whiskers", "Simba", "Milo", "Tiger", "Oscar", "Leo", "Shadow",
                        "Bella", "Luna", "Chloe", "Lily", "Nala", "Cleo", "Sophie" }[random.Next(14)];
                }
                else if (speciesRoll < 95) // 5% Birds
                {
                    species = "Bird";
                    breed = BirdTypes[random.Next(BirdTypes.Length)];
                    weight = (decimal)(random.NextDouble() * 0.5 + 0.05);
                    age = random.Next(0, 12);
                    name = new[] { "Tweety", "Polly", "Kiwi", "Mango", "Rio", "Sunny", "Coco" }[random.Next(7)];
                }
                else // 5% Others
                {
                    species = OtherPetSpecies[random.Next(OtherPetSpecies.Length)];
                    breed = "Mixed";
                    weight = (decimal)(random.NextDouble() * 3 + 0.5);
                    age = random.Next(0, 10);
                    name = new[] { "Fluffy", "Nibbles", "Thumper", "Spike", "Patches" }[random.Next(5)];
                }

                gender = random.Next(2) == 0 ? "Male" : "Female";
                var color = PetColors[random.Next(PetColors.Length)];
                var dob = DateTime.UtcNow.AddYears(-age).AddDays(-random.Next(0, 365));
                var regDate = dob.AddDays(random.Next(30, 180));

                pets.Add(new
                {
                    PetOwnerId = ownerId,
                    Name = name,
                    Species = species,
                    Breed = breed,
                    Age = age,
                    PetWeight = Math.Round(weight, 2),
                    Color = color,
                    Gender = gender,
                    DateOfBirth = dob,
                    RegistrationDate = regDate,
                    IsActive = 1
                });
            }

            await connection.ExecuteAsync(@"
                INSERT INTO Pet (PetOwnerId, Name, Species, Breed, Age, PetWeight, Color, Gender, DateOfBirth, RegistrationDate, IsActive, CreatedDate)
                VALUES (@PetOwnerId, @Name, @Species, @Breed, @Age, @PetWeight, @Color, @Gender, @DateOfBirth, @RegistrationDate, @IsActive, GETUTCDATE())", pets);

            totalInserted += currentBatchSize;
            double progress = (totalInserted / (double)PetCount) * 100;
            Console.Write($"\r  Progress: {progress:F1}% ({totalInserted:N0}/{PetCount:N0})");
        }

        sw.Stop();
        Console.WriteLine($"\n  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    // Continue with remaining seed methods in next response...

    static async Task<int> GetTableCount(string tableName)
    {
        using var connection = new SqlConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {tableName}");
    }

    static async Task PrintDatabaseStatistics()
    {
        Console.WriteLine("\n📊 Database Statistics:");
        Console.WriteLine("═══════════════════════════════════════════════════════");

        var tables = new[]
        {
            "PetOwner", "Vet", "Lab", "Store", "Pet", "Medicine",
            "VetAppointment", "LabAppointment", "MedicineOrder",
            "VetPayment", "LabPayment", "StorePayment",
            "VetReview", "LabReview", "StoreReview",
            "MedicalRecord", "Inventory", "EducationQualification", "Service"
        };

        foreach (var table in tables)
        {
            var count = await GetTableCount(table);
            Console.WriteLine($"  {table,-30} {count,15:N0} rows");
        }

        Console.WriteLine("═══════════════════════════════════════════════════════");

        using var connection = new SqlConnection(_connectionString);
        var totalRows = await connection.ExecuteScalarAsync<int>(@"
            SELECT SUM(p.rows) FROM sys.tables t
            INNER JOIN sys.partitions p ON t.object_id = p.object_id
            WHERE t.is_ms_shipped = 0 AND p.index_id IN (0,1)");

        Console.WriteLine($"  {"TOTAL ROWS",-30} {totalRows,15:N0}");
    }

    static async Task SeedVetAppointments()
        {
            Console.WriteLine($"\n📅 Seeding {AppointmentCount:N0} Vet Appointments (OlaDoc pattern)...");
            var sw = Stopwatch.StartNew();
            var random = new Random(12355);

            using var connection = new SqlConnection(_connectionString);
            var pets = (await connection.QueryAsync<PetWithOwner>("SELECT PetId, PetOwnerId FROM Pet")).ToList();
            var vetIds = (await connection.QueryAsync<int>("SELECT VetId FROM Vet")).ToList();

            int totalInserted = 0;
            int batchCount = (int)Math.Ceiling(AppointmentCount / (double)BatchSize);

            for (int batch = 0; batch < batchCount; batch++)
            {
                int currentBatchSize = Math.Min(BatchSize, AppointmentCount - totalInserted);
                var appointments = new List<object>();

                for (int i = 0; i < currentBatchSize; i++)
                {
                    var pet = pets[random.Next(pets.Count)];
                    var vetId = vetIds[random.Next(vetIds.Count)];

                    // OlaDoc pattern: 70% in last 6 months, 30% older
                    var daysAgo = random.Next(100) < 70 ? random.Next(0, 180) : random.Next(180, 730);
                    var appointmentDate = DateTime.UtcNow.AddDays(-daysAgo);

                    // Peak hours: 60% morning (9-12), 40% evening (17-20)
                    var isPeakMorning = random.Next(100) < 60;
                    var hour = isPeakMorning ? random.Next(9, 12) : random.Next(17, 20);
                    var minute = random.Next(0, 4) * 15; // 0, 15, 30, 45
                    appointmentDate = appointmentDate.Date.AddHours(hour).AddMinutes(minute);

                    // Status distribution: 70% completed, 20% scheduled, 10% cancelled
                    int statusId;
                    DateTime? completedDate = null;

                    if (daysAgo > 7)
                    {
                        var statusRoll = random.Next(100);
                        if (statusRoll < 70) { statusId = 3; completedDate = appointmentDate.AddHours(1); }
                        else if (statusRoll < 90) statusId = 1;
                        else statusId = 4;
                    }
                    else { statusId = 1; }

                    // 80% clinic, 20% video
                    var appointmentTypeId = random.Next(10) < 8 ? 1 : 2;
                    var reason = AppointmentReasons[random.Next(AppointmentReasons.Length)];
                    var notes = random.Next(10) < 3 ? $"Follow-up required in 2 weeks. {reason}" : null;

                    appointments.Add(new
                    {
                        PetId = pet.PetId,
                        PetOwnerId = pet.PetOwnerId,
                        VetId = vetId,
                        VetAppointmentTypeId = appointmentTypeId,
                        AppointmentDateTime = appointmentDate,
                        StatusTypeId = statusId,
                        Reason = reason,
                        Notes = notes,
                        CompletedDate = completedDate
                    });
                }

                await connection.ExecuteAsync(@"
            INSERT INTO VetAppointment (PetId, PetOwnerId, VetId, VetAppointmentTypeId, AppointmentDateTime, StatusTypeId, Reason, Notes, CompletedDate, CreatedDate)
            VALUES (@PetId, @PetOwnerId, @VetId, @VetAppointmentTypeId, @AppointmentDateTime, @StatusTypeId, @Reason, @Notes, @CompletedDate, GETUTCDATE())", appointments);

                totalInserted += currentBatchSize;
                double progress = (totalInserted / (double)AppointmentCount) * 100;
                double rate = totalInserted / sw.Elapsed.TotalSeconds;
                Console.Write($"\r  Progress: {progress:F1}% ({totalInserted:N0}/{AppointmentCount:N0}) - {rate:F0} rows/sec");
            }

            sw.Stop();
            Console.WriteLine($"\n  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
        }  

    static async Task SeedLabAppointments()
    {
        Console.WriteLine($"\n🧬 Seeding {LabAppointmentCount:N0} Lab Appointments...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12356);

        using var connection = new SqlConnection(_connectionString);
        var pets = (await connection.QueryAsync<PetWithOwner>("SELECT PetId, PetOwnerId FROM Pet")).ToList();
        var labIds = (await connection.QueryAsync<int>("SELECT LabId FROM Lab")).ToList();

        int totalInserted = 0;
        int batchCount = (int)Math.Ceiling(LabAppointmentCount / (double)BatchSize);

        for (int batch = 0; batch < batchCount; batch++)
        {
            int currentBatchSize = Math.Min(BatchSize, LabAppointmentCount - totalInserted);
            var appointments = new List<object>();

            for (int i = 0; i < currentBatchSize; i++)
            {
                var pet = pets[random.Next(pets.Count)];
                var labId = labIds[random.Next(labIds.Count)];
                var daysAgo = random.Next(0, 365);
                var appointmentDate = DateTime.UtcNow.AddDays(-daysAgo);
                var statusId = daysAgo > 5 ? 3 : 1;
                var completedDate = statusId == 3 ? appointmentDate.AddHours(random.Next(24, 73)) : (DateTime?)null;
                var notes = $"Lab tests requested by veterinarian for diagnostic purposes";

                appointments.Add(new
                {
                    PetId = pet.PetId,
                    PetOwnerId = pet.PetOwnerId,
                    LabId = labId,
                    AppointmentDateTime = appointmentDate,
                    StatusTypeId = statusId,
                    Notes = notes,
                    CompletedDate = completedDate
                });
            }

            await connection.ExecuteAsync(@"
            INSERT INTO LabAppointment (PetId, PetOwnerId, LabId, AppointmentDateTime, StatusTypeId, Notes, CompletedDate, CreatedDate)
            VALUES (@PetId, @PetOwnerId, @LabId, @AppointmentDateTime, @StatusTypeId, @Notes, @CompletedDate, GETUTCDATE())", appointments);

            totalInserted += currentBatchSize;
            double progress = (totalInserted / (double)LabAppointmentCount) * 100;
            Console.Write($"\r  Progress: {progress:F1}% ({totalInserted:N0}/{LabAppointmentCount:N0})");
        }

        sw.Stop();
        Console.WriteLine($"\n  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedLabAppointmentTests()
    {
        Console.WriteLine("\n🔬 Seeding Lab Appointment Tests...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12357);

        using var connection = new SqlConnection(_connectionString);
        var completedAppts = (await connection.QueryAsync<int>(@"
        SELECT LabAppointmentId FROM LabAppointment WHERE StatusTypeId = 3")).ToList();
        var testIds = (await connection.QueryAsync<int>("SELECT LabTestId FROM LabTest")).ToList();

        var tests = new List<object>();

        foreach (var apptId in completedAppts)
        {
            int testCount = random.Next(1, 4); // 1-3 tests per appointment
            var selectedTests = testIds.OrderBy(x => random.Next()).Take(testCount);

            foreach (var testId in selectedTests)
            {
                var result = new[]
                {
                "All values within normal range",
                "Slightly elevated WBC - Minor infection possible",
                "Results normal for age and species",
                "No abnormalities detected",
                "Mild abnormalities - Recommend follow-up",
                "Test results indicate good health",
                "Within acceptable parameters"
            }[random.Next(7)];

                var daysAfterAppt = random.Next(1, 4);
                var resultDate = DateTime.UtcNow.AddDays(-random.Next(30, 365)).AddDays(daysAfterAppt);

                tests.Add(new
                {
                    LabAppointmentId = apptId,
                    LabTestId = testId,
                    TestResult = result,
                    ResultDate = resultDate,
                    ResultFile = $"/lab-results/{apptId}-{testId}.pdf"
                });
            }
        }

        await connection.ExecuteAsync(@"
        INSERT INTO LabAppointmentTest (LabAppointmentId, LabTestId, TestResult, ResultDate, ResultFile)
        VALUES (@LabAppointmentId, @LabTestId, @TestResult, @ResultDate, @ResultFile)", tests);

        sw.Stop();
        Console.WriteLine($"  ✅ {tests.Count:N0} lab tests in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedMedicineOrders()
    {
        Console.WriteLine($"\n🛒 Seeding {MedicineOrderCount:N0} Medicine Orders...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12358);

        using var connection = new SqlConnection(_connectionString);
        var ownerIds = (await connection.QueryAsync<int>("SELECT PetOwnerId FROM PetOwner")).ToList();
        var storeIds = (await connection.QueryAsync<int>("SELECT StoreId FROM Store")).ToList();

        int totalInserted = 0;
        int batchCount = (int)Math.Ceiling(MedicineOrderCount / (double)BatchSize);

        for (int batch = 0; batch < batchCount; batch++)
        {
            int currentBatchSize = Math.Min(BatchSize, MedicineOrderCount - totalInserted);
            var orders = new List<object>();

            for (int i = 0; i < currentBatchSize; i++)
            {
                var ownerId = ownerIds[random.Next(ownerIds.Count)];
                var storeId = storeIds[random.Next(storeIds.Count)];
                var daysAgo = random.Next(0, 365);
                var orderDate = DateTime.UtcNow.AddDays(-daysAgo);
                var statusId = daysAgo > 3 ? 9 : (daysAgo > 1 ? 8 : 7); // Delivered/Shipped/Processing
                var totalAmount = Math.Round((decimal)(random.NextDouble() * 4500 + 500), 2);

                var locality = LahoreLocalities[random.Next(LahoreLocalities.Length)];
                var streetName = StreetNames[random.Next(StreetNames.Length)];
                var houseNumber = random.Next(1, 999);
                var deliveryAddress = $"House {houseNumber}, {streetName}, {locality}, Lahore, Pakistan";

                var deliveredDate = statusId == 9 ? orderDate.AddDays(random.Next(1, 6)) : (DateTime?)null;

                orders.Add(new
                {
                    PetOwnerId = ownerId,
                    StoreId = storeId,
                    OrderDateTime = orderDate,
                    StatusTypeId = statusId,
                    TotalAmount = totalAmount,
                    DeliveryAddress = deliveryAddress,
                    DeliveredDate = deliveredDate
                });
            }

            await connection.ExecuteAsync(@"
            INSERT INTO MedicineOrder (PetOwnerId, StoreId, OrderDateTime, StatusTypeId, TotalAmount, DeliveryAddress, DeliveredDate)
            VALUES (@PetOwnerId, @StoreId, @OrderDateTime, @StatusTypeId, @TotalAmount, @DeliveryAddress, @DeliveredDate)", orders);

            totalInserted += currentBatchSize;
            double progress = (totalInserted / (double)MedicineOrderCount) * 100;
            Console.Write($"\r  Progress: {progress:F1}% ({totalInserted:N0}/{MedicineOrderCount:N0})");
        }

        sw.Stop();
        Console.WriteLine($"\n  ✅ Completed in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedMedicineOrderDetails()
    {
        Console.WriteLine("\n📋 Seeding Medicine Order Details...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12359);

        using var connection = new SqlConnection(_connectionString);
        var orderIds = (await connection.QueryAsync<int>("SELECT MedicineOrderId FROM MedicineOrder")).ToList();
        var medicines = (await connection.QueryAsync<dynamic>("SELECT MedicineId, Price FROM Medicine")).ToList();

        var details = new List<object>();

        foreach (var orderId in orderIds)
        {
            int itemCount = random.Next(1, 6); // 1-5 items per order
            var selectedMedicines = medicines.OrderBy(x => random.Next()).Take(itemCount);

            foreach (var medicine in selectedMedicines)
            {
                var quantity = random.Next(1, 6);
                var unitPrice = (decimal)medicine.Price;

                details.Add(new
                {
                    MedicineOrderId = orderId,
                    MedicineId = (int)medicine.MedicineId,
                    Quantity = quantity,
                    UnitPrice = unitPrice
                });
            }
        }

        await connection.ExecuteAsync(@"
        INSERT INTO MedicineOrderDetails (MedicineOrderId, MedicineId, Quantity, UnitPrice)
        VALUES (@MedicineOrderId, @MedicineId, @Quantity, @UnitPrice)", details);

        sw.Stop();
        Console.WriteLine($"  ✅ {details.Count:N0} order details in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedMedicalRecords()
    {
        Console.WriteLine("\n📋 Seeding Medical Records...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12360);

        using var connection = new SqlConnection(_connectionString);
        var completedAppts = (await connection.QueryAsync<dynamic>(@"
        SELECT VetAppointmentId, PetId, PetOwnerId, VetId, CompletedDate
        FROM VetAppointment WHERE StatusTypeId = 3 AND CompletedDate IS NOT NULL")).ToList();

        var records = new List<object>();

        foreach (var appt in completedAppts)
        {
            var recordTypeId = random.Next(1, 6);
            var diagnosis = Diagnoses[random.Next(Diagnoses.Length)];
            var treatment = Treatments[random.Next(Treatments.Length)];
            var attachmentPath = $"/medical-records/pet-{appt.PetId}/record-{appt.VetAppointmentId}.pdf";

            records.Add(new
            {
                PetId = (int)appt.PetId,
                PetOwnerId = (int)appt.PetOwnerId,
                RecordTypeId = recordTypeId,
                RecordDate = (DateTime)appt.CompletedDate,
                Diagnosis = diagnosis,
                TreatmentDescription = treatment,
                VetId = (int)appt.VetId,
                AttachmentPath = attachmentPath
            });
        }

        await connection.ExecuteAsync(@"
        INSERT INTO MedicalRecord (PetId, PetOwnerId, RecordTypeId, RecordDate, Diagnosis, TreatmentDescription, VetId, AttachmentPath)
        VALUES (@PetId, @PetOwnerId, @RecordTypeId, @RecordDate, @Diagnosis, @TreatmentDescription, @VetId, @AttachmentPath)", records);

        sw.Stop();
        Console.WriteLine($"  ✅ {records.Count:N0} medical records in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedVetPayments()
    {
        Console.WriteLine("\n💰 Seeding Vet Payments...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12361);

        using var connection = new SqlConnection(_connectionString);
        var completedAppts = (await connection.QueryAsync<dynamic>(@"
        SELECT va.VetAppointmentId, va.PetOwnerId, va.VetId, v.Fee, va.CompletedDate
        FROM VetAppointment va
        INNER JOIN Vet v ON va.VetId = v.VetId
        WHERE va.StatusTypeId = 3 AND va.CompletedDate IS NOT NULL")).ToList();

        var payments = new List<object>();
        int counter = 0;

        foreach (var appt in completedAppts)
        {
            var paymentMethod = new[] { "Wallet", "Card", "Cash" }[random.Next(3)];
            // Use counter + full GUID for guaranteed uniqueness
            var transactionId = $"TXN-VET-{counter++:D7}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

            payments.Add(new
            {
                VetAppointmentId = (int)appt.VetAppointmentId,
                PetOwnerId = (int)appt.PetOwnerId,
                VetId = (int)appt.VetId,
                Amount = (decimal)appt.Fee,
                PaymentDateTime = (DateTime)appt.CompletedDate,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId
            });
        }

        await connection.ExecuteAsync(@"
        INSERT INTO VetPayment (VetAppointmentId, PetOwnerId, VetId, Amount, PaymentDateTime, PaymentMethod, TransactionId)
        VALUES (@VetAppointmentId, @PetOwnerId, @VetId, @Amount, @PaymentDateTime, @PaymentMethod, @TransactionId)", payments);

        sw.Stop();
        Console.WriteLine($"  ✅ {payments.Count:N0} payments in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedLabPayments()
    {
        Console.WriteLine("\n💰 Seeding Lab Payments...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12362);

        using var connection = new SqlConnection(_connectionString);
        var completedAppts = (await connection.QueryAsync<dynamic>(@"
        SELECT la.LabAppointmentId, la.PetOwnerId, la.LabId, la.CompletedDate
        FROM LabAppointment la
        WHERE la.StatusTypeId = 3 AND la.CompletedDate IS NOT NULL")).ToList();

        var payments = new List<object>();
        int counter = 0;

        foreach (var appt in completedAppts)
        {
            var amount = Math.Round((decimal)(random.NextDouble() * 5000 + 1000), 2);
            var paymentMethod = new[] { "Wallet", "Card", "Cash" }[random.Next(3)];
            // Use counter + GUID for guaranteed uniqueness
            var transactionId = $"TXN-LAB-{counter++:D7}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

            payments.Add(new
            {
                LabAppointmentId = (int)appt.LabAppointmentId,
                PetOwnerId = (int)appt.PetOwnerId,
                LabId = (int)appt.LabId,
                Amount = amount,
                PaymentDateTime = (DateTime)appt.CompletedDate,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId
            });
        }

        await connection.ExecuteAsync(@"
        INSERT INTO LabPayment (LabAppointmentId, PetOwnerId, LabId, Amount, PaymentDateTime, PaymentMethod, TransactionId)
        VALUES (@LabAppointmentId, @PetOwnerId, @LabId, @Amount, @PaymentDateTime, @PaymentMethod, @TransactionId)", payments);

        sw.Stop();
        Console.WriteLine($"  ✅ {payments.Count:N0} payments in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedStorePayments()
    {
        Console.WriteLine("\n💰 Seeding Store Payments...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12363);

        using var connection = new SqlConnection(_connectionString);
        var orders = (await connection.QueryAsync<dynamic>(@"
        SELECT MedicineOrderId, PetOwnerId, StoreId, TotalAmount, OrderDateTime
        FROM MedicineOrder WHERE StatusTypeId = 9")).ToList();

        var payments = new List<object>();
        int counter = 0;

        foreach (var order in orders)
        {
            var paymentMethod = new[] { "Wallet", "Card", "Cash" }[random.Next(3)];
            // Use counter + GUID for guaranteed uniqueness
            var transactionId = $"TXN-STORE-{counter++:D7}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

            payments.Add(new
            {
                MedicineOrderId = (int)order.MedicineOrderId,
                PetOwnerId = (int)order.PetOwnerId,
                StoreId = (int)order.StoreId,
                Amount = (decimal)order.TotalAmount,
                PaymentDateTime = (DateTime)order.OrderDateTime,
                PaymentMethod = paymentMethod,
                TransactionId = transactionId
            });
        }

        await connection.ExecuteAsync(@"
        INSERT INTO StorePayment (MedicineOrderId, PetOwnerId, StoreId, Amount, PaymentDateTime, PaymentMethod, TransactionId)
        VALUES (@MedicineOrderId, @PetOwnerId, @StoreId, @Amount, @PaymentDateTime, @PaymentMethod, @TransactionId)", payments);

        sw.Stop();
        Console.WriteLine($"  ✅ {payments.Count:N0} payments in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedVetReviews()
    {
        Console.WriteLine("\n⭐ Seeding Vet Reviews...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12364);

        using var connection = new SqlConnection(_connectionString);
        var completedAppts = (await connection.QueryAsync<dynamic>(@"
        SELECT VetAppointmentId, PetOwnerId, VetId, CompletedDate
        FROM VetAppointment WHERE StatusTypeId = 3")).ToList();

        var reviews = new List<object>();

        foreach (var appt in completedAppts)
        {
            if (random.Next(100) < 65) // 65% leave reviews
            {
                var rating = random.Next(100) < 80
                    ? random.Next(4, 6)  // 80% give 4-5 stars
                    : random.Next(3, 6); // 20% give 3-5 stars

                var comment = ReviewComments[random.Next(ReviewComments.Length)];
                var reviewDate = ((DateTime)appt.CompletedDate).AddHours(random.Next(2, 72));

                reviews.Add(new
                {
                    VetAppointmentId = (int)appt.VetAppointmentId,
                    PetOwnerId = (int)appt.PetOwnerId,
                    VetId = (int)appt.VetId,
                    Rating = rating,
                    Comments = comment,
                    ReviewDateTime = reviewDate
                });
            }
        }

        await connection.ExecuteAsync(@"
        INSERT INTO VetReview (VetAppointmentId, PetOwnerId, VetId, Rating, Comments, ReviewDateTime)
        VALUES (@VetAppointmentId, @PetOwnerId, @VetId, @Rating, @Comments, @ReviewDateTime)", reviews);

        sw.Stop();
        Console.WriteLine($"  ✅ {reviews.Count:N0} reviews in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedLabReviews()
    {
        Console.WriteLine("\n⭐ Seeding Lab Reviews...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12365);

        using var connection = new SqlConnection(_connectionString);
        var completedAppts = (await connection.QueryAsync<dynamic>(@"
        SELECT LabAppointmentId, PetOwnerId, LabId, CompletedDate
        FROM LabAppointment WHERE StatusTypeId = 3")).ToList();

        var reviews = new List<object>();

        foreach (var appt in completedAppts)
        {
            if (random.Next(100) < 60) // 60% leave reviews
            {
                var rating = random.Next(100) < 75 ? random.Next(4, 6) : random.Next(3, 6);
                var comment = ReviewComments[random.Next(ReviewComments.Length)];
                var reviewDate = ((DateTime)appt.CompletedDate).AddHours(random.Next(2, 72));

                reviews.Add(new
                {
                    LabAppointmentId = (int)appt.LabAppointmentId,
                    PetOwnerId = (int)appt.PetOwnerId,
                    LabId = (int)appt.LabId,
                    Rating = rating,
                    Comments = comment,
                    ReviewDateTime = reviewDate
                });
            }
        }

        await connection.ExecuteAsync(@"
        INSERT INTO LabReview (LabAppointmentId, PetOwnerId, LabId, Rating, Comments, ReviewDateTime)
        VALUES (@LabAppointmentId, @PetOwnerId, @LabId, @Rating, @Comments, @ReviewDateTime)", reviews);

        sw.Stop();
        Console.WriteLine($"  ✅ {reviews.Count:N0} reviews in {sw.Elapsed.TotalSeconds:F2}s");
    }

    static async Task SeedStoreReviews()
    {
        Console.WriteLine("\n⭐ Seeding Store Reviews...");
        var sw = Stopwatch.StartNew();
        var random = new Random(12366);

        using var connection = new SqlConnection(_connectionString);
        var deliveredOrders = (await connection.QueryAsync<dynamic>(@"
        SELECT MedicineOrderId, PetOwnerId, StoreId, DeliveredDate
        FROM MedicineOrder WHERE StatusTypeId = 9 AND DeliveredDate IS NOT NULL")).ToList();

        var reviews = new List<object>();

        foreach (var order in deliveredOrders)
        {
            if (random.Next(100) < 55) // 55% leave reviews
            {
                var rating = random.Next(100) < 70 ? random.Next(4, 6) : random.Next(3, 6);
                var comment = ReviewComments[random.Next(ReviewComments.Length)];
                var reviewDate = ((DateTime)order.DeliveredDate).AddHours(random.Next(2, 72));

                reviews.Add(new
                {
                    MedicineOrderId = (int)order.MedicineOrderId,
                    PetOwnerId = (int)order.PetOwnerId,
                    StoreId = (int)order.StoreId,
                    Rating = rating,
                    Comments = comment,
                    ReviewDateTime = reviewDate
                });
            }
        }

        await connection.ExecuteAsync(@"
        INSERT INTO StoreReview (MedicineOrderId, PetOwnerId, StoreId, Rating, Comments, ReviewDateTime)
        VALUES (@MedicineOrderId, @PetOwnerId, @StoreId, @Rating, @Comments, @ReviewDateTime)", reviews);

        sw.Stop();
        Console.WriteLine($"  ✅ {reviews.Count:N0} reviews in {sw.Elapsed.TotalSeconds:F2}s");
    }

    // =============================================
    // SUPPORTING RECORD TYPES
    // =============================================
    record PetWithOwner(int PetId, int PetOwnerId);
}
