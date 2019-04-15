using Hospital.Controllers;
using P01_HospitalDatabase.Data;
using System;

namespace P01_HospitalDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            using (HospitalContext context = new HospitalContext())
            {
                DoctorContoller doctorContoller = new DoctorContoller();
                string input = "";

                while ((input = Console.ReadLine()) != "exit")
                {
                    string[] tokens = input.Split();

                    ParseCommand(tokens, doctorContoller, context);
                }
            }
        }

        private static void ParseCommand(string[] tokens, DoctorContoller doctorContoller, HospitalContext context)
        {
            string command = tokens[0];

            if (command == "register")
            {
                doctorContoller.RegisterDoctor(tokens[1], tokens[2], context);
                context.SaveChanges();
                Console.WriteLine("You registred!");
            }
            else if (command == "login")
            {
                if (doctorContoller.IsLogged)
                {
                    Console.WriteLine("You are logged!");
                    return;
                }
                doctorContoller.LoginDoctor(tokens[1], context);
            }
            else if (command == "addPatient")
            {
                string firstName = tokens[1];
                string lastName = tokens[2];
                string address = tokens[3];
                string email = tokens[4];

                if (doctorContoller.IsLogged)
                {
                    doctorContoller.AddPatient(firstName, lastName, address, email, true, context);
                    context.SaveChanges();
                    Console.WriteLine("Ptient add");
                }
                else
                {
                    Console.WriteLine("You are not logged in!");
                }
            }
            else if (command == "addMedicament")
            {
                string name = tokens[1];

                if (!doctorContoller.IsLogged)
                {
                    Console.WriteLine("You are not logged in!");
                    return;
                }

                doctorContoller.AddMedicament(name, context);
                context.SaveChanges();
                Console.WriteLine("Medicament was added!");
            }
            else if(command == "giveMedicamentToPatient")
            {
                if (!doctorContoller.IsLogged)
                {
                    Console.WriteLine("You are not logged in!");
                    return;
                }

                int patinetId = int.Parse(tokens[1]);
                string medicamentName = tokens[2];
                doctorContoller.GiveMedicamentToPatient(patinetId, medicamentName, context);
                context.SaveChanges();
                Console.WriteLine("Medicament was given to the selected patient!");
            }
        }
    }
}
