using System;

namespace InstallmentLib.Models
{
    public class InstallmentDto
    {
        public Guid Id { get; private set; }
        public int Number { get; private set; }
        public decimal Value { get; private set; }
        public string DueDate { get; private set; }
    }

    public class InstallmentModel
    {
        public InstallmentModel(Guid id, int number, decimal value, string dueDate)
        {
            Id = id;
            Number = number;
            Value = value;
            DueDate = dueDate;
        }

        public Guid Id { get; private set; }
        public int Number { get; private set; }
        public decimal Value { get; private set; }
        public string DueDate { get; private set; }
    }
}
