using Microsoft.Win32;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Assignment
{
    public partial class MainWindow : Window
    {
        //List to store the teams in object within a list 
        List<TeamDetails> teamList = new List<TeamDetails>();
        //File management object used for reading and writing to text.
        FileManager file = new FileManager();
        //Constructor for the main window.
        bool isEditMode = false;
        bool isSaved = true;
        public MainWindow()
        {
            InitializeComponent();
            teamList = file.ReadDataFromFile();
            UpdateTableView();
            btnDel.IsEnabled = false;
            btnClear.IsEnabled = false;
        }

        //Methods called from these button clicks
        //Clears the form for the user
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
          ClearFields();
            btnClear.IsEnabled = false;
            btnDel.IsEnabled = false;
        }
        //Add entry
        private void btnEntry_Click(object sender, RoutedEventArgs e)
        {
            //if not in edit mode add the entry if in edit mode save edit.
            if (!isEditMode)
            {
                AddNewEntry();
            }
            else 
            {
                EditEntry();
            }
        }
        
        //Delete Selected Row
        private void btnDel_Click(object sender, RoutedEventArgs e)
        {
            DeleteEntry();
        }
        //Save .CSV to file
        private void btnSave_Click(object sender, RoutedEventArgs e){SaveToFile();}
        //Open new .CSV
        private void btnOpen_Click(object sender, RoutedEventArgs e) {
            //on click opens a file dialog to handle selection
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //set defaults for dialog box
            openFileDialog.Filter = "CSV Files(*.csv) | *.csv";
            openFileDialog.DefaultExt = ".csv";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //if ok selected in dialog change the file path and update table
            if (openFileDialog.ShowDialog() == true)
            {
                filePath.Content = "Current File Path: " + openFileDialog.FileName;
                file.fileName = openFileDialog.FileName;
                teamList = file.ReadDataFromFile();
                UpdateTableView();
            }
        }
        
        
        //Method to check email validity
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            //copied from online - not 100% on how it works but it works
            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,RegexOptions.None, TimeSpan.FromMilliseconds(200));
                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match){
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();
                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);
                    return match.Groups[1].Value + domainName;}
            }
            catch (RegexMatchTimeoutException e){return false;}
            catch (ArgumentException e){return false;}
            try{return Regex.IsMatch(email,@"^[^@\s]+@[^@\s]+\.[^@\s]+$",RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));}
            catch (RegexMatchTimeoutException){return false;}
        }
        //Checks each text field to see if it is blank. 
        //If any of them are it will return a false value
        //and sets the focus on the first blank field,
        //otherwise it returns a true(all filled correctly) value.
        private bool IsFormFilledCorrectly()
        {
            if (string.IsNullOrWhiteSpace(txtTeamName.Text)|| txtTeamName.Text.Contains(',')) { txtTeamName.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtContact.Text) || txtContact.Text.Contains(',')) { txtContact.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtPhone.Text) || txtPhone.Text.Contains(',')) { txtPhone.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || txtEmail.Text.Contains(',')) { txtEmail.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtPoints.Text) || txtPoints.Text.Contains(',')) { txtPoints.Focus(); return false; }
            return true;
        }
        //Method to add new entries to the table
        public void AddNewEntry()
        {
            //check form filled
            if (IsFormFilledCorrectly() == false)
            {
                MessageBox.Show("You Must Fill all Fields Correctly!\n(Cannot Be Empty OR Contain a Comma.)");
                return;
            }
            TeamDetails newEntry = GetEnteredFields();
            //check each item to see if it passes entry checks
            //if not shows error box and stops user from adding entry
            foreach (TeamDetails team in teamList)
            {
                //cant have same team name as one that already exists
                if (newEntry.TeamName == team.TeamName)
                {
                    MessageBox.Show($"This team already exixts ({team.TeamName}).");
                    txtTeamName.Focus();
                    return;
                }
                //must be valid phone number
                if (!int.TryParse(newEntry.Phone, out int result1) || newEntry.Phone.Length > 10 || newEntry.Phone.Length < 8)
                {
                    MessageBox.Show($"Not a valid Phone number! ({result1} is not possible).\nMust be 8-10 Numbers Long.");
                    txtPhone.Focus();
                    return;
                }
                //must be correct email format 
                if (!IsValidEmail(newEntry.EmailAddress))
                {
                    MessageBox.Show($"Email Adress not valid! \n({newEntry.EmailAddress} is not possible).");
                    txtEmail.Focus();
                    return;
                }
                //must be integers for points
                if (!int.TryParse(newEntry.TeamPoints, out int result2) || result2<=0)
                {
                    MessageBox.Show($"Team points must be positive, whole numbers! \n({result2} is not possible).");
                    txtPoints.Focus();
                    return;
                }
            }
            teamList.Add(newEntry);
            UpdateTableView();
            ClearFields();
            isSaved = false;
        }

        private TeamDetails GetEnteredFields()
        {
            //make new object from fields to be inserted
            TeamDetails newEntry = new TeamDetails();
            newEntry.TeamName = txtTeamName.Text;
            newEntry.ContactName = txtContact.Text;
            newEntry.Phone = txtPhone.Text;
            newEntry.EmailAddress = txtEmail.Text;
            newEntry.TeamPoints = txtPoints.Text;
            return newEntry;
        }

        //Method to save edit
        private void EditEntry()
        {
            //check form filled
            if (IsFormFilledCorrectly() == false)
            {
                MessageBox.Show("You Must Fill all Fields Correctly!\n(Cannot Be Empty OR Contain a Comma.)");
                return;
            }
            //make new object from fields to be inserted
            TeamDetails newEntry = new TeamDetails();
            newEntry.TeamName = txtTeamName.Text;
            newEntry.ContactName = txtContact.Text;
            newEntry.Phone = txtPhone.Text;
            newEntry.EmailAddress = txtEmail.Text;
            newEntry.TeamPoints = txtPoints.Text;
            //check each item to see if it passes entry checks
            //if not shows error box and stops user from adding entry
            foreach (TeamDetails entry in teamList)
            {
                //must be valid phone number
                if (!int.TryParse(newEntry.Phone, out int result1) || newEntry.Phone.Length > 10 || newEntry.Phone.Length < 8)
                {
                    MessageBox.Show($"Not a valid Phone number! ({result1} is not possible).\nMust be 8-10 Numbers Long.");
                    txtPhone.Focus();
                    return;
                }
                //must be correct email format 
                if (!IsValidEmail(newEntry.EmailAddress))
                {
                    MessageBox.Show($"Email Adress not valid! \n({newEntry.EmailAddress} is not possible).");
                    txtEmail.Focus();
                    return;
                }
                //must be integers for points
                if (!int.TryParse(newEntry.TeamPoints, out int result2)|| result2<=0)
                {
                    MessageBox.Show($"Team points must be positive, whole numbers! \n({result2} is not possible).");
                    txtPoints.Focus();
                    return;
                }
            }
            TeamDetails team = (TeamDetails)dgvTableView.SelectedItem;
            if (team != null)
            {
                //removes old row from datagrid
                teamList.Remove(team);
            }
            //adds new "edited" row back into datagrid
            teamList.Add(newEntry);
            UpdateTableView();
            ClearFields();
            //reset button to add
            btnEntry.Content = "Add Entry";
            isEditMode = false;
            MessageBox.Show("Changes Applied!");
            isSaved = false;
        }
        //Method to delete entries from the table
        private void DeleteEntry()
        {
            //checks for a selection
            if (IsFormFilledCorrectly() == false)
            {
                MessageBox.Show("Nothing Selected!");
                return;
            }
            //puts selection into object, asks if they want to delete
            //if yes deletes and resets table
            //if no cancels delete action
            TeamDetails teamDel = (TeamDetails)dgvTableView.SelectedItem;
            if (teamDel != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete?", $"Deleting {teamDel.TeamName}...", MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        teamDel.TeamName = txtTeamName.Text;
                        teamDel.ContactName = txtContact.Text;
                        teamDel.Phone = txtPhone.Text;
                        teamDel.EmailAddress = txtEmail.Text;
                        teamDel.TeamPoints = txtPoints.Text;
                        teamList.Remove(teamDel);
                        UpdateTableView();
                        ClearFields();
                        isSaved = false;
                        btnClear.IsEnabled = false;
                        btnDel.IsEnabled = false;
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            }
        }
        //Method to save table to .CSV
        private void SaveToFile()
        {
            //opens save dialog box, sets defaults
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV Files(*.csv) | *.csv";
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Nullable<bool> result = saveFileDialog.ShowDialog();
            //if they selected ok in dialog box saves the file
            if (result == true)
            {
                filePath.Content = "Current File Path: " + saveFileDialog.FileName;
                file.fileName = saveFileDialog.FileName;
                file.WriteDataToFile(teamList.ToArray());
                UpdateTableView();
                ClearFields();
                MessageBox.Show("Save Successful.");
                isSaved = true;
            }
        }
        //Method to clear entries
        public void ClearFields()
        {
           //Sets all the text fields to blank
            txtTeamName.Text = "";
            txtContact.Text = "";
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtPoints.Text = "";
            //Puts ther focus(cursor) back to the first text field
            txtTeamName.Focus();
            btnEntry.Content = "Add Entry";
            btnEntry.FontWeight = FontWeights.Regular;
            btnDel.IsEnabled = false;
            isEditMode = false;
            btnClear.IsEnabled = false;
            dgvTableView.SelectedItem = null;
        }
        //Sets the source of data for the table to display.
        private void UpdateTableView()
        {
            dgvTableView.ItemsSource = teamList;            
            dgvTableView.Items.Refresh();
        }
        //Checks if the enter key is pressed and produces a button click on the add/edit button
        private void StackPanel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) 
            { 
                btnEntry.Click += btnEntry_Click;
            }
        }
        //Displays Team selected from DataGrid in Text boxes.
        private void dgvTableView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TeamDetails team = (TeamDetails)dgvTableView.SelectedItem;
            if (team != null)
            {
                txtTeamName.Text = team.TeamName;
                txtContact.Text = team.ContactName;
                txtPhone.Text = team.Phone;
                txtEmail.Text = team.EmailAddress;
                txtPoints.Text = team.TeamPoints;
            }
            btnDel.IsEnabled = true;
            //change to edit mode
            btnEntry.Content = "Save Changes";
            btnEntry.FontWeight = FontWeights.Bold;
            isEditMode = true;
            btnClear.IsEnabled = true;
        }
        //Asks the user if they want to save on close
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (isSaved) return;
            MessageBoxResult result = MessageBox.Show($"Closing Program, Changes are not saved...\n Do you want to save to\n {file.fileName}?", $"Closing...", MessageBoxButton.YesNoCancel);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    SaveToFile();
                    break;
                case MessageBoxResult.No:
                    MessageBoxResult result2 = MessageBox.Show($"Are you sure you want to quit without saving?", $"Closing...", MessageBoxButton.YesNo);
                    if(result2 != MessageBoxResult.Yes)e.Cancel = true;
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

    }
}