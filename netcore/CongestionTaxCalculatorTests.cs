using System;
using Xunit;
using congestion.calculator;

namespace CongestionTaxCalculatorTests
{
    public class CongestionTaxCalculatorTests
    {
        [Fact]
        public void CalculateTaxForSinglePass_Car()
        {
            // Setup
            CongestionTaxCalculator calculator = new CongestionTaxCalculator();
            Vehicle car = new Car();
            DateTime[] passes = { new DateTime(2013, 2, 7, 6, 15, 0) };

            // Execute
            int tax = calculator.GetTax(car, passes);

            // Verify
            Assert.Equal(8, tax);
        }

        [Fact]
        public void CalculateTaxForMultiplePasses_Car()
        {
            // Setup
            CongestionTaxCalculator calculator = new CongestionTaxCalculator();
            Vehicle car = new Car();
            DateTime[] passes = {
                // Passes within 60 minutes
                new DateTime(2013, 2, 7, 6, 0, 0), // Fee 8 SEK
                new DateTime(2013, 2, 7, 17, 55, 0), // Fee 13 SEK
                new DateTime(2013, 2, 7, 18, 9, 0), // Fee 8 SEK
                new DateTime(2013, 2, 7, 18, 31, 0), // Fee 0 SEK
            };

            // Execute
            int tax = calculator.GetTax(car, passes);

            // Verify
            // Should be charged by mornin fee (8 SEK) and evening fee only (13 SEK)
            // In total 21 SEK
            Assert.Equal(21, tax);
        }

        [Fact]
        public void CalculateTaxForMultiplePassesInSameHour_Car()
        {
            // Setup
            CongestionTaxCalculator calculator = new CongestionTaxCalculator();
            Vehicle car = new Car();
            DateTime[] passes = {
                // Passes within 60 minutes
                new DateTime(2013, 2, 7, 7, 55, 0), // Fee 18 SEK
                new DateTime(2013, 2, 7, 8, 9, 0), // Fee 13 SEK
                new DateTime(2013, 2, 7, 8, 31, 0), // Fee 8 SEK
            };

            // Execute
            int tax = calculator.GetTax(car, passes);

            // Verify
            // Should only charge the highest fee within the 60-minute window
            Assert.Equal(18, tax);
        }

        [Fact]
        public void CalculateTaxWithExemptVehicle_Motorcycle()
        {
            // Setup
            CongestionTaxCalculator calculator = new CongestionTaxCalculator();
            Vehicle motorcycle = new Motorbike();
            DateTime[] passes = { new DateTime(2013, 2, 7, 6, 15, 0) };

            // Execute
            int tax = calculator.GetTax(motorcycle, passes);

            // Verify
            // Motorcycles are tax exempt
            Assert.Equal(0, tax);
        }

        [Fact]
        public void CalculateTaxForMultipleDaysMaxCharge_Car()
        {
            // Setup
            CongestionTaxCalculator calculator = new CongestionTaxCalculator();
            Vehicle car = new Car();
            DateTime[] passes = {
                new DateTime(2013, 2, 7, 6, 0, 0), // Fee 8 SEK
                new DateTime(2013, 2, 7, 7, 30, 0), // Fee 18 SEK
                new DateTime(2013, 2, 7, 8, 45, 0), // Fee 8 SEK
                new DateTime(2013, 2, 7, 15, 30, 0), // Fee 18 SEK
                new DateTime(2013, 2, 7, 17, 0, 0) // Fee 13 SEK
            };

            // Execute
            int tax = calculator.GetTax(car, passes);

            // Assert
            // Ensure the maximum daily charge is capped at 60 SEK
            // (8 + 18 + 8 + 18 + 13 = 65 > 60)
            // And no additional fees are added
            Assert.Equal(60, tax);
        }
    }
}

