namespace Patient_Transferral_System.Entities
{
    public class TransferralPatient
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public TransferralPatient(string FirstName, string LastName, string PhoneNumber, string Address)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.PhoneNumber = PhoneNumber;
            this.Address = Address;
        }
    }
}