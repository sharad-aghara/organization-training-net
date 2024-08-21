namespace WebApplication1.Models
{
    public class TrainingEmployee
    {
        public int TrainingId { get; set; }
        public Training Training { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
