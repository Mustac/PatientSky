using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PatientSky.Models;
using Newtonsoft.Json;
using System.Numerics;

namespace PatientSky.Data
{
    public class AppDbContext
    {
        public List<Appointment> Apointments { get; set; }
        public List<Patient> Patients { get; set; }
        public List<Doctor> Doctors { get; set; } 

        // Trying to simulate Entity Framework

        public AppDbContext()
        {


            // Getting Appoitments directory and parsing them to List of Appointments

            string jsonApoitmentsFile = File.ReadAllText(Directory.GetCurrentDirectory() + "\\PatientsSkyData\\Appoitments.json");

            Apointments = JsonConvert.DeserializeObject<List<Appointment>>(jsonApoitmentsFile);




            // Getting Patients directory and parsing them to List of Patients
            
            string jsonPatientsFile = File.ReadAllText(Directory.GetCurrentDirectory() + "\\PatientsSkyData\\Patients.json");
            

            Dictionary<string, Patient> patientsDictionary = JsonConvert.DeserializeObject<Dictionary<string,Patient>>(jsonPatientsFile);

            Patients = new List<Patient>();

            foreach (var patient in patientsDictionary)
            {

                Patients.Add(new Patient()
                {
                    PatientId = patient.Key,
                    BirthDate = patient.Value.BirthDate,
                    Contacts = patient.Value.Contacts,
                    Firstname = patient.Value.Firstname,
                    Gender = patient.Value.Gender,
                    Lastname = patient.Value.Lastname,
                    Middlename = patient.Value.Middlename,
                    PersonalId = patient.Value.PersonalId
                });   
            }

            
            Doctors = new List<Doctor>();
            Doctors.Add(new Doctor { CalendarId = "48644c7a-975e-11e5-a090-c8e0eb18c1e9", FirstName="Joanna", LastName="Hef" });
            Doctors.Add(new Doctor { CalendarId = "48cadf26-975e-11e5-b9c2-c8e0eb18c1e9", FirstName = "Danny", LastName = "Boy" });
            Doctors.Add(new Doctor { CalendarId = "452dccfc-975e-11e5-bfa5-c8e0eb18c1e9", FirstName = "Emma", LastName = "Win" });

        }

    }


    

}
