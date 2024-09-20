using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment
{
    //Class to create Team Objects to fill
    public class TeamDetails
    {
        public string TeamName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public string TeamPoints { get; set; } = string.Empty;

        public TeamDetails() { }

        public TeamDetails(string teamName, string contactName, string phone, string emailAddress,string teamPoints)
        {
            TeamName = teamName;
            ContactName = contactName;
            Phone = phone;
            EmailAddress = emailAddress;
            TeamPoints = teamPoints;
        }
    }
}
