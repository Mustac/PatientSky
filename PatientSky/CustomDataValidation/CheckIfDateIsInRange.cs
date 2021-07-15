using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PatientSky.CustomDataValidation
{
    public class CheckIfDateIsInRange : ValidationAttribute
    {
        
        public override bool IsValid(object value)
        {
            DateTime startDate = DateTime.Parse("2019-03-01T13:00:00Z");
            DateTime endDate = DateTime.Parse("2019-05-11T23:59:59Z");

            DateTime date = (DateTime)value;

            if(date.Date>=startDate.Date && date.Year>= startDate.Year && date.Date <= endDate.Date && date.Year<=endDate.Year)
                return true;

            return false;
        }
    }
}
