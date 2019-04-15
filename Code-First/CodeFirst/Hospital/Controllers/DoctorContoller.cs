using P01_HospitalDatabase.Data;
using P01_HospitalDatabase.Data.Models;
using System;
using System.Linq;

namespace Hospital.Controllers
{
    public class DoctorContoller
    {
        public bool IsLogged { get; set; }

        public void RegisterDoctor(string name, string speciality, HospitalContext context)
        {
            context.Doctors.Add(new Doctor()
            {
                Name = name,
                Specialty = speciality
            });
        }

        public void LoginDoctor(string name, HospitalContext context)
        {
            var doctor = context.Doctors.FirstOrDefault(d => d.Name == name);

            if(doctor != null)
            {
                IsLogged = true;
                Console.WriteLine("You successfully loged in!");
            }
            else
            {
                Console.WriteLine("Invalid name!");
            }
        }

        public void AddPatient(string firstName, string lastName, string address, string email, bool hasInsurrance, HospitalContext context)
        {
            context.Patient.Add(new Patient()
            {
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                Email = email,
                HasInsurance = hasInsurrance
            });
        }

        public void AddMedicament(string name, HospitalContext context)
        {
            context.Medicaments.Add(new Medicament() { Name = name });
        }

        public void GiveMedicamentToPatient(int patientId, string medicamentName, HospitalContext context)
        {
            Patient patient = context.Patient.Find(patientId);

            if(patient == null)
            {
                Console.WriteLine("No such patint!");
                return;
            }

            Medicament medicament = context.Medicaments.FirstOrDefault(m => m.Name == medicamentName);

            if (medicament == null)
            {
                Console.WriteLine("No such medicament!");
                return;
            }

            context.PatientsMedicaments.Add(new PatientMedicament()
            {
                PatientId = patientId,
                MedicamentId = medicament.MedicamentId
            });
        }
    }
}
