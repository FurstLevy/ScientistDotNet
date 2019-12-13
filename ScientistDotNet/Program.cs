using GitHub;
using System;
using System.Threading.Tasks;

namespace ScientistDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var productionData = new Customer {Age = 15};

            // Configure how we want results published
            Scientist.ResultPublisher = new ConsoleResultPublisher();

            // Run the expereiment
            var status = Scientist.Science<CustomerStatus>("customer status", experiment =>
            {
                // Current production method
                experiment.Use(() => CustomerStatusCalculatorExisting.CalculateStatus(productionData));

                // One or more candidates to compare result to
                experiment.Try("Awesome new simplified algorithm", () =>
                    CustomerStatusCalculatorNew.CalculateStatus(productionData));
            });
        }

        private static bool IsCollaborator(string user)
        {
            return user.Equals("Scientist");
        }

        private static bool HasAccess(string user)
        {
            return user.ToUpper().Equals("SCIENTIST");
        }
    }

    class Customer
    {
        public int Age { get; set; }
    }

    enum CustomerStatus
    {
        Unknown,
        Standard,
        Gold
    }

    static class CustomerStatusCalculatorExisting
    {
        public static CustomerStatus CalculateStatus(Customer customer)
        {
            if (customer.Age >= 16 && customer.Age <= 21)
            {
                return CustomerStatus.Gold;
            }

            if (customer.Age > 21)
            {
                return CustomerStatus.Standard;
            }

            return CustomerStatus.Unknown;
        }
    }


    static class CustomerStatusCalculatorNew
    {
        /// <summary>
        /// New implementation assumes that all production data has no customers under 16 years old
        /// </summary>        
        public static CustomerStatus CalculateStatus(Customer customer)
        {

            if (customer.Age <= 21)
            {
                return CustomerStatus.Gold;
            }

            return CustomerStatus.Standard;
        }
    }

    public class ConsoleResultPublisher : IResultPublisher
    {
        public Task Publish<T, TClean>(Result<T, TClean> result)
        {
            if (result.Mismatched)
            {
                Console.WriteLine(
                    $"Experiment name: '{result.ExperimentName}' resulted in mismatched results");
                Console.WriteLine($"Existing code result: {result.Control.Value}");

                foreach (var candidate in result.Candidates)
                {
                    Console.WriteLine($"Candidate name: {candidate.Name}");
                    Console.WriteLine($"Candidate value: {candidate.Value}");
                    Console.WriteLine($"Candidate execution duration: {candidate.Duration}");
                }
            }

            return Task.FromResult(0);
        }
    }
}
