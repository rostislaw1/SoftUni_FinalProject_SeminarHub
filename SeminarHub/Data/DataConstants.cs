namespace SeminarHub.Data
{
    public class DataConstants
    {
        public const int SeminarTopicMinimumLength = 3;
        public const int SeminarTopicMaximumLength = 100;

        public const int SeminarLecturerMinimumLength = 5;
        public const int SeminarLecturerMaximumLength = 60;

        public const int SeminarDetailsMinimumLength = 10;
        public const int SeminarDetailsMaximumLength = 500;

        public const int SeminarDurationMinimumValue = 30;
        public const int SeminarDurationMaximumValue = 180;
        public const string SeminarDurationErrorMessage = "The value must be between {1} and {2}";

        public const string DateFormat = "dd/MM/yyyy HH:mm";

        public const int CategoryNameMinimumLength = 3;
        public const int CategoryNameMaximumLength = 50;

        public const string RequireErrorMessage = "The field {0} is required";
        public const string StringLengthErrorMessage = "The field {0} must be between {2} and {1} characters long";
    }
}
