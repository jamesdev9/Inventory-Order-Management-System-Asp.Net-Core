using Indotalent.Applications.NumberSequences;
using Indotalent.Applications.VendorContacts;
using Indotalent.Applications.Vendors;
using Indotalent.Models.Entities;

namespace Indotalent.Data.Demo
{
    public static class DemoVendorContact
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var vendorContactService = services.GetRequiredService<VendorContactService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var vendorService = services.GetRequiredService<VendorService>();

            var firsts = new string[]
            {
                "Adam",
                "Sarah",
                "Michael",
                "Emily",
                "David",
                "Jessica",
                "Kevin",
                "Samantha",
                "Jason",
                "Olivia",
                "Matthew",
                "Ashley",
                "Christopher",
                "Jennifer",
                "Nicholas",
                "Amanda",
                "Alexander",
                "Stephanie",
                "Jonathan",
                "Lauren"
            };

            var lasts = new string[]
            {
                "Johnson",
                "Williams",
                "Brown",
                "Jones",
                "Miller",
                "Davis",
                "Garcia",
                "Rodriguez",
                "Wilson",
                "Martinez",
                "Anderson",
                "Taylor",
                "Thomas",
                "Hernandez",
                "Moore",
                "Martin",
                "Jackson",
                "Thompson",
                "White",
                "Lopez"
            };

            var jobTitles = new string[]
            {
                "Chief Executive Officer",
                "Data Scientist",
                "Product Manager",
                "Business Development Executive",
                "IT Consultant",
                "Social Media Specialist",
                "Research Analyst",
                "Content Writer",
                "Operations Manager",
                "Financial Planner",
                "Software Developer",
                "Vendor Success Manager",
                "Marketing Coordinator",
                "Quality Assurance Tester",
                "HR Specialist",
                "Event Coordinator",
                "Account Executive",
                "Network Administrator",
                "Sales Manager",
                "Legal Assistant"
            };

            var vendors = vendorService.GetAll().Select(x => x.Id).ToArray();

            Random random = new Random();

            foreach (var item in vendors)
            {
                for (int i = 0; i < 3; i++)
                {
                    var first = DbInitializer.GetRandomString(firsts, random);
                    var last = DbInitializer.GetRandomString(lasts, random);

                    await vendorContactService.AddAsync(new VendorContact
                    {
                        Name = $"{first} {last}",
                        Number = numberSequenceService.GenerateNumber(nameof(VendorContact), "", "VC"),
                        VendorId = item,
                        JobTitle = DbInitializer.GetRandomString(jobTitles, random),
                        EmailAddress = $"{first.ToLower()}.{last.ToLower()}@gmail.com"
                    });

                }
            }
        }
    }
}
