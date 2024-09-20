using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Assignment 
{ 
    public class FileManager()
    {
        //Variable to store the file name
        public string fileName = "TournamentData.csv";
        // Takes a provided array and writes its contents to a csv file
        // in a comma delimited format.
        public void WriteDataToFile(TeamDetails[] teamData)
        {
            //The 'using' statement generates the class to connect to a file(streamwriter) and
            //uses it as the structure runs. Once the using statement finishes, the resource
            //will be automatically disconnected and destroyed
            using (var writer = new StreamWriter(fileName))
            {
                //Cycyles through each element in the array
                foreach (var team in teamData)
                {
                    //Each property of each entry is printed ona single line with commas to
                    //separate them (Comma Delimmited File or Comma Separated Values(CSV))
                    writer.WriteLine($"{team.TeamName},{team.ContactName},{team.Phone},{team.EmailAddress},{team.TeamPoints}");
                }
            }
        }
        // Reads the data from the specified file and breaks it back down into TeamDetails objects
        // which are then placed in a list.
        // <returns>A list of DataModel objects representing the lines in the file.</returns>
        public List<TeamDetails> ReadDataFromFile()
        {
            try
            {
                //List to store the data from the file
                List<TeamDetails> teamList = new List<TeamDetails>();
                //The using statement generates the class to connect to a file(streamreader) and
                //uses it as the structure runs. Once the using statement finishes, the resource
                //will be automatically disconnected and destroyed
                using (var reader = new StreamReader(fileName))
                {
                    //Variable to store each line as it is processd
                    string line;
                    //This while loop reads the next line from the file and stores it in the line
                    //variable. Once done it uses the IsNullOrWhiteSpace method to check if the line
                    //has text or not. If it is not empty the while statement runs.
                    while (String.IsNullOrWhiteSpace(line = reader.ReadLine()) == false)
                    {
                        //splits the line into sections where each section its the text between the commas
                        //and line ends
                        //once done the resulting temp array will hold each piece of data in each of the
                        //seperate elements.
                        string[] temp = line.Split(',');
                        //Each element of the temp array into a data model in the required order
                        TeamDetails details = new TeamDetails(temp[0], temp[1], temp[2], temp[3], temp[4]);
                        //Adds the new DataModel to our list
                        teamList.Add(details);
                    }
                }
                return teamList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return new List<TeamDetails>();
            }
            
        }
    }

}
