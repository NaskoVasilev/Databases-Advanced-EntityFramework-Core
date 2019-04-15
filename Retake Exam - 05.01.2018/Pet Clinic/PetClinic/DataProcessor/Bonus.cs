namespace PetClinic.DataProcessor
{
    using System;

    using PetClinic.Data;
    using System.Linq;

    public class Bonus
    {
        public static string UpdateVetProfession(PetClinicContext context, string phoneNumber, string newProfession)
        {
            var vet = context.Vets.FirstOrDefault(x => x.PhoneNumber == phoneNumber);

            if(vet == null)
            {
                return $"Vet with phone number {phoneNumber} not found!";
            }

            string message = $"{vet.Name}'s profession updated from {vet.Profession} to {newProfession}.";
            vet.Profession = newProfession;
            context.Vets.Update(vet);
            context.SaveChanges();
            return message;
        }
    }
}
