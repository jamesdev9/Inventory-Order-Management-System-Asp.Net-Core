using Indotalent.Applications.CustomerContacts;
using Indotalent.Applications.Customers;
using Indotalent.Applications.NumberSequences;
using Indotalent.Models.Entities;

namespace Indotalent.Data.Demo
{
    public static class DemoCustomerContact
    {
        public static async Task GenerateAsync(IServiceProvider services)
        {
            var customerContactService = services.GetRequiredService<CustomerContactService>();
            var numberSequenceService = services.GetRequiredService<NumberSequenceService>();
            var customerService = services.GetRequiredService<CustomerService>();

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
                "Customer Success Manager",
                "Marketing Coordinator",
                "Quality Assurance Tester",
                "HR Specialist",
                "Event Coordinator",
                "Account Executive",
                "Network Administrator",
                "Sales Manager",
                "Legal Assistant"
            };

            var customers = customerService.GetAll().Select(x => x.Id).ToArray();

            Random random = new Random();

            foreach (var item in customers)
            {
                for (int i = 0; i < 3; i++)
                {
                    var first = DbInitializer.GetRandomString(firsts, random);
                    var last = DbInitializer.GetRandomString(lasts, random);

                    await customerContactService.AddAsync(new CustomerContact
                    {
                        Name = $"{first} {last}",
                        Number = numberSequenceService.GenerateNumber(nameof(CustomerContact), "", "CC"),
                        CustomerId = item,
                        JobTitle = DbInitializer.GetRandomString(jobTitles, random),
                        EmailAddress = $"{first.ToLower()}.{last.ToLower()}@gmail.com"
                    });

                }
            }
        }
    }
}
