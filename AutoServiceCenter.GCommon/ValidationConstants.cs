namespace AutoServiceCenter.GCommon
{
    public static class ValidationConstants
    {

        public static class Vehicle
        {
            public const int MakeMaxLength = 120;
            public const int ModelMaxLength = 120;
            public const int YearMinValue = 1950;
            public const int YearMaxValue = 2100;
            public const int LicensePlateMaxLength = 15;
        }

        public static class Service
        {
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 500;
            public const double PriceMinValue = 0.00; // Minimum price for a service (Left 0.00 in case of a mix-up and job redone)
            public const double PriceMaxValue = 10000.00; // Maximum price for a service
        }

        public static class Customer
        {
            public const int AddressMaxLength = 255;
        }

        public static class Mechanic
        {
            public const int SpecializationMaxLength = 100;
            public const int ExperienceYearsMinValue = 0;
            public const int ExperienceYearsMaxValue = 50;
        }

        public static class Appointment
        {
            public const int NotesMaxLength = 1000;
            public const string DateFormat = "yyyy-MM-dd";
        }
    }
}