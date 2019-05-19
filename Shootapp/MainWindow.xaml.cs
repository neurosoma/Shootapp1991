using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.ComponentModel;

namespace Shootapp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Shots intput array
        private TextBox[] shootBox;

        //Competition ID
        private long compId;

        //Flags for quick editing
        private bool quickEditing = false;

        //Filter global Variables
        int ColumnIndex { get; set; }
        ListSortDirection FilterSortDirection { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            // Set Shots intput array 
            shootBox = new TextBox[12];
            shootBox[0] = txb0;
            shootBox[1] = txb1;
            shootBox[2] = txb2;
            shootBox[3] = txb3;
            shootBox[4] = txb4;
            shootBox[5] = txb5;
            shootBox[6] = txb6;
            shootBox[7] = txb7;
            shootBox[8] = txb8;
            shootBox[9] = txb9;
            shootBox[10] = txb10;
            shootBox[11] = txb11;

            ColumnIndex = -1;
        }

        // On window loaded connect listis to datagrid items source
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateCompGrid();
            UpdateShooGrid();
            UpdateTeamGrid();

            UpdateShootersGrid2();
            UpdateCompetitionBox2();
        }

        // Update competitions grid
        void UpdateCompGrid()
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                competitionsGrid.ItemsSource = context.competitions.ToList();
            }
        }

        // Update shooters grid
        void UpdateShooGrid()
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                if (context.shooterscoviews != null)
                {
                    shootersGrid.ItemsSource = context.shooterscoviews.ToList();
                }
            }
        }

        // Update teams grid
        void UpdateTeamGrid()
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                teamsGrid.ItemsSource = context.teams.ToList();
                cbxShooTeam.ItemsSource = context.teams.ToList().OrderBy(l => l.name).ToList();
                cbxShooTeam.DisplayMemberPath = "name";
                cbxShooTeam.SelectedValuePath = "id";
                cbxShooTeam.SelectedIndex = 0;
            }
        }

        // Clear competitions tab
        void ClearCompTab()
        {
            txbCompId.Text = string.Empty;
            txbCompName.Text = string.Empty;
            dapCompDate.Text = string.Empty;
            txbCompShots.Text = "10";
            competitionsGrid.SelectedItem = null;
        }

        // Clear shooters tab
        void ClearShooTab()
        {
            txbShooId.Text = string.Empty;
            txbShooName.Text = string.Empty;
            txbShooSname.Text = string.Empty;
            txbShooNote.Text = string.Empty;
            shootersGrid.SelectedItem = null;
            //cbxShooTeam.SelectedIndex = 0;
        }

        // Clear teams tab
        void ClearTeamTab()
        {
            txbTeamId.Text = string.Empty;
            txbTeamName.Text = string.Empty;
            teamsGrid.SelectedItem = null;
        }

        // Clear competitions tab button
        private void btnCompClear_Click(object sender, RoutedEventArgs e)
        {
            ClearCompTab();
            txbCompName.Focus();
        }

        // Clear shooters tab button
        private void btnShooClear_Click(object sender, RoutedEventArgs e)
        {
            ClearShooTab();
            txbShooName.Focus();
        }

        // Clear teams tab button
        private void btnTeamClear_Click(object sender, RoutedEventArgs e)
        {
            ClearTeamTab();
            txbTeamName.Focus();
        }

        /***************************************************COMPETITION******************************************************/
        private void FlipStringHits(List<competitionview> cList)
        {
            if (cList != null)
            {
                foreach (competitionview cv in cList)
                {
                    if (cv.hits is string)
                    {
                        cv.hits = string.Join(":", cv.hits.Split(':').Reverse().ToArray());
                    }
                }
            }
        }

        private void FlipStringHits(competitionview cv)
        {
            if (cv != null && cv.hits != null)
            {
                cv.hits = string.Join(":", cv.hits.Split(':').Reverse().ToArray());
            }
        }


        private void UpdateSeekData()
        {
            UpdateShootersGrid2();
            UpdateCompetitionGrid2();
            UpdateCompetitionGridSingle2();
        }

        private void UpdateCompetitionGrid2()
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                var query = from cv in context.competitionviews
                            where cv.comid == compId
                            select cv;

                if (query != null)
                {
                    List<competitionview> lc = query.ToList();
                    FlipStringHits(lc);
                    ListCollectionView view = new ListCollectionView(lc);
                    view.GroupDescriptions.Add(new PropertyGroupDescription("teaname"));
                    competitionGrid.ItemsSource = view;
                }
            }
        }

        private void UpdateCompetitionGridSingle2()
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                var query = from cv in context.competitionviews
                            where cv.comid == compId
                            select cv;

                if (query != null)
                {
                    List<competitionview> lc = query.ToList();
                    FlipStringHits(lc);
                    shootersSingleGrid.ItemsSource = query.ToList();
                }

            }
        }

        private void UpdateShootersGrid2()
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                if (context.shooterscoviews != null)
                {
                    shootersGrid2.ItemsSource = context.shooterscoviews.ToList();
                }
            }

            //MessageBox.Show(ColumnIndex.ToString());


            if (ColumnIndex > -1)
            {
                //Clear current sort descriptions 
                shootersGrid2.Items.SortDescriptions.Clear();

                //Get property name to apply sort based on desired column              
                DataGridBoundColumn dgbc = (DataGridBoundColumn)shootersGrid2.Columns[ColumnIndex];
                Binding binding = (Binding)dgbc.Binding;
                string propertyName = binding.Path.Path;

                //Add the new sort description 
                shootersGrid2.Items.SortDescriptions.Add(new SortDescription(propertyName, FilterSortDirection));

                shootersGrid2.Columns[ColumnIndex].SortDirection = FilterSortDirection;

                //refresh items to display sort 
                shootersGrid2.Items.Refresh();
            }
        }


        private void UpdateCompetitionBox2()
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                cbxComp.ItemsSource = context.competitions.ToList();
                cbxComp.DisplayMemberPath = "name";
                cbxComp.SelectedValuePath = "id";
                cbxComp.SelectedIndex = 0;
            }
        }

        // Load competitionif you pick competition on combobox
        private void cbxComp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                if (cbxComp.SelectedValue != null)
                {
                    // Set labels
                    competition competition = context.competitions.FirstOrDefault(c => c.id == (long)cbxComp.SelectedValue);

                    lblDate.Content = competition.date;
                    lblShoo.Content = competition.shots;
                    compId = competition.id;

                    var qComp = from cv in context.competitionviews
                                where cv.comid == competition.id
                                select cv;

                    shootersSingleGrid.ItemsSource = qComp.ToList();

                    List<competitionview> lc = qComp.ToList();
                    FlipStringHits(lc);
                    ListCollectionView view = new ListCollectionView(lc);
                    view.GroupDescriptions.Add(new PropertyGroupDescription("teaname"));
                    competitionGrid.ItemsSource = view;
                }
            }
        }

        // Add shooter to competition
        private void btnAdd2_Click(object sender, RoutedEventArgs e)
        {
            AddShooterToComp();
        }

        private void AddShooterToComp()
        {
            shooterscoview shooterco = (shooterscoview)shootersGrid2.SelectedItem;
            long sid = -1;

            if (shooterco != null)
            {
                try
                {
                    using (ShootappDBEntities context = new ShootappDBEntities())
                    {
                        result result = new result()
                        {
                            cid = compId,
                            sid = shooterco.id,
                            shots = null,
                            score = null,
                            hits = null
                        };

                        // for selecting competitor after sdding it
                        sid = shooterco.id;

                        context.results.Add(result);
                        context.SaveChanges();
                    }
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateException)
                {
                    MessageBox.Show("Already on the list.", "Warning");
                    return;
                }
                UpdateCompetitionGrid2();
                UpdateCompetitionGridSingle2();
            }

            //Find added competitor in datagrid by teams
            for (int i = 0; i < competitionGrid.Items.Count; i++)
            {
                competitionview cv = (competitionview)competitionGrid.Items[i];
                if (cv.shoid == sid)
                {
                    competitionGrid.SelectedItem = cv;
                    break;
                }
            }

            //Find added competitor in datagrid single
            for (int i = 0; i < shootersSingleGrid.Items.Count; i++)
            {
                competitionview cv = (competitionview)shootersSingleGrid.Items[i];
                if (cv.shoid == sid)
                {
                    shootersSingleGrid.SelectedItem = cv;
                    break;
                }
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                //Focus on editing
                txb0.Focus();
            }));
        }

        // Overloaded
        private void AddShooterToComp(long id)
        {
            try
            {
                using (ShootappDBEntities context = new ShootappDBEntities())
                {
                    result result = new result()
                    {
                        cid = compId,
                        sid = id,
                        shots = null,
                        score = null,
                        hits = null
                    };
                    context.results.Add(result);
                    context.SaveChanges();
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException)
            {
                MessageBox.Show("Already on the list.", "Warning");
                return;
            }
            UpdateCompetitionGrid2();
            UpdateCompetitionGridSingle2();

            //Find added competitor in datagrid by teams
            for (int i = 0; i < competitionGrid.Items.Count; i++)
            {
                competitionview cv = (competitionview)competitionGrid.Items[i];
                if (cv.shoid == id)
                {
                    competitionGrid.SelectedItem = cv;
                    break;
                }
            }

            //Find added competitor in datagrid single
            for (int i = 0; i < shootersSingleGrid.Items.Count; i++)
            {
                competitionview cv = (competitionview)shootersSingleGrid.Items[i];
                if (cv.shoid == id)
                {
                    shootersSingleGrid.SelectedItem = cv;
                    break;
                }
            }

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                //Focus on editing
                txb0.Focus();
            }));
        }

        // Focus on shooter in main window to edit
        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            shooterscoview shooco = (shooterscoview)shootersGrid2.SelectedItem;

            if (shooco != null)
            {
                SeekSelectRow(shooco.id);
            }
            tabCont.SelectedIndex = 2;

            FreezeTabs();

            btnShooAdd.IsEnabled = false;
            btnShooDel.IsEnabled = false;
            btnShooClear.IsEnabled = false;
        }

        // Delete competitior from competition
        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            competitionview comView = (competitionview)competitionGrid.SelectedItem;

            if (comView == null)
            {
                MessageBox.Show("Please select shooter for removal.", "Warning!");
                return;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                result result = context.results.FirstOrDefault(r => r.cid == comView.comid && r.sid == comView.shoid);
                context.results.Remove(result);
                context.SaveChanges();
            }
            UpdateCompetitionGrid2();
            UpdateCompetitionGridSingle2();
        }

        // Set Shots
        private void btnSetShots_Click(object sender, RoutedEventArgs e)
        {
            competitionview comView = null;

            int selectedRow = competitionGrid.SelectedIndex;

            if (tbiTeaShoo.SelectedIndex == 0)
            {
                selectedRow = competitionGrid.SelectedIndex;
                comView = (competitionview)competitionGrid.SelectedItem;
            }
            else if (tbiTeaShoo.SelectedIndex == 1)
            {
                selectedRow = shootersSingleGrid.SelectedIndex;
                comView = (competitionview)shootersSingleGrid.SelectedItem;
            }

            if (comView == null)
            {
                MessageBox.Show("Please select shooter in competition grid.", "Warning!");
                return;
            }

            FlipStringHits(comView);

            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                result result = context.results.FirstOrDefault(r => r.cid == comView.comid && r.sid == comView.shoid);
                if (!SetResults(result))
                {
                    FlipStringHits(comView);
                    return;
                }
                context.SaveChanges();
            }
            UpdateCompetitionGrid2();
            UpdateCompetitionGridSingle2();

            if (tbiTeaShoo.SelectedIndex == 0)
            {
                competitionGrid.SelectedIndex = selectedRow;
            }
            else if (tbiTeaShoo.SelectedIndex == 1)
            {
                shootersSingleGrid.SelectedIndex = selectedRow;
            }
            FlipStringHits(comView);
        }

        // Create Results
        private bool SetResults(result result)
        {
            long nshots = 0;
            long score = 0;
            long penal = 0;
            long nParse;

            string lshots = string.Empty;

            List<long> shotss = new List<long>();

            for (int i = 0; i < shootBox.Length; i++)
            {
                if (shootBox[i].Text == string.Empty)
                {
                    shootBox[i].Text = "0";
                }
                if (long.TryParse(shootBox[i].Text, out nParse))
                {
                    shotss.Add(nParse);
                    score += nParse * (i != 11 ? i : 10);
                    nshots += nParse;
                }
                else
                {
                    MessageBox.Show("Hit text box only accepts integers", "Warning!");
                    return false;
                }
            }
            // Get number of shots per game
            nParse = (long)lblShoo.Content;

            // Check if shots 
            if (nshots < nParse)
            {
                shotss[0] += nParse - nshots;
                //shootBox[0].Text = string.Format("{0}", nParse - nshots);
            }
            else if (nshots > nParse)
            {
                List<long> cpyShotss = new List<long>(shotss);
                for (long i = nshots - nParse; 0 < i; i--)
                {
                    for (int j = cpyShotss.Count - 1; 0 < j; j--)
                    {
                        if (cpyShotss[j] > 0)
                        {
                            score -= j != 11 ? j : 10;
                            penal += j != 11 ? j : 10;
                            cpyShotss[j]--;
                            break;
                        }
                    }
                }
            }
            // Create string
            for (int i = 0; i < shotss.Count; i++)
            {
                if (i == 11)
                {
                    lshots += shotss[i].ToString();
                }
                else
                {
                    lshots += shotss[i].ToString() + ":";
                }
            }

            // Total amount of shots
            result.shots = 0;
            foreach (long shot in shotss)
            {
                result.shots += shot;
            }

            result.score = score;
            result.penal = penal;
            result.hits = lshots;
            return true;
        }


        // Selection Changed Comp Grid
        private void competitionGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int regShotsInt = 0;
            //int finScoreDou = 0;

            competitionview comrow = (competitionview)competitionGrid.SelectedItem;

            FlipStringHits(comrow);

            if (comrow == null)
            {
                return;
            }
            if (comrow.hits != null)
            {
                string[] xhits = comrow.hits.Split(':');

                for (int i = 0; i < shootBox.Length; i++)
                {
                    shootBox[i].Text = xhits[i];

                    switch (i)
                    {
                        case 0:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 1:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 2:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 3:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 4:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 5:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 6:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 7:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 8:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 9:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 10:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 11:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                    }
                }

                FlipStringHits(comrow);

                lblRegShots.Content = regShotsInt.ToString();
                lblFinalScore.Content = !(comrow.score is null) ? comrow.score.ToString() : "";

                //finScoreDou.ToString();

                /*CHANGE TEXT COLOR*/
                if (regShotsInt > int.Parse(lblShoo.Content.ToString()))
                {
                    lblRegShots.Foreground = Brushes.Red;
                    lblRegShots.FontWeight = FontWeights.Bold;
                }
                else if (regShotsInt == int.Parse(lblShoo.Content.ToString()))
                {
                    lblRegShots.Foreground = Brushes.Green;
                }
                else
                {
                    lblRegShots.Foreground = Brushes.Red;
                }
            }
            else
            {
                foreach (TextBox tb in shootBox)
                {
                    tb.Text = string.Empty;
                }
                lblRegShots.Content = string.Empty;
                lblFinalScore.Content = string.Empty;
            }
        }


        //FILTER INNER WORKINGS
        private void shootersGrid2_Sorting(object sender, DataGridSortingEventArgs e)
        {
            ColumnIndex = e.Column.DisplayIndex;
            FilterSortDirection = e.Column.SortDirection == ListSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        }

        //FILTER EVENT
        private void txbNameSer_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchStr = txbNameSer.Text.ToLower();

            if (searchStr != String.Empty)
            {

                using (ShootappDBEntities context = new ShootappDBEntities())
                {
                    if (context.shooterscoviews != null)
                    {
                        ListCollectionView lview = new ListCollectionView(context.shooterscoviews.ToList());

                        //Custom filter please work you bastard!!!

                        if (ColumnIndex == 1)
                        {

                            lview.Filter = (s) =>
                            {
                                shooterscoview ssv = s as shooterscoview;
                                if (ssv.name != null && ssv.name.ToLower().StartsWith(searchStr))
                                {
                                    return true;
                                }
                                else return false;
                            };
                        }
                        else if (ColumnIndex == 2)
                        {

                            lview.Filter = (s) =>
                            {
                                shooterscoview ssv = s as shooterscoview;
                                if (ssv.surname != null && ssv.surname.ToLower().StartsWith(searchStr))
                                {
                                    return true;
                                }
                                else return false;
                            };
                        }
                        else if (ColumnIndex == 4)
                        {

                            lview.Filter = (s) =>
                            {
                                shooterscoview ssv = s as shooterscoview;
                                if (ssv.tname != null && ssv.tname.ToLower().StartsWith(searchStr))
                                {
                                    return true;
                                }
                                else return false;
                            };
                        }
                        else
                        {
                            context.Dispose();
                            return;
                        }
                        shootersGrid2.ItemsSource = lview;
                    }
                }

            }
            else
            {
                UpdateShootersGrid2();
            }
        }

        private void shootersGrid2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddShooterToComp();
        }

        /***************************************************COMPETITION_DBM******************************************************/

        // Add competition to DB
        private void AddCompetition()
        {
            DateTime date;
            int shots;

            if (txbCompId.Text != string.Empty)
            {
                MessageBox.Show("Press new buton, create new competition then press add.", "Warning!");
                return;
            }
            if (txbCompName.Text == string.Empty)
            {
                MessageBox.Show("Competition name can't be empty.", "Warning!");
                return;
            }
            if (!DateTime.TryParse(dapCompDate.Text, out date))
            {
                MessageBox.Show("Invalid competition date.", "Warning!");
                return;
            }
            if (!int.TryParse(txbCompShots.Text, out shots))
            {
                MessageBox.Show("Number of shots must be integer.", "Warning!");
                return;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                context.competitions.Add(new competition()
                {
                    name = txbCompName.Text,
                    date = date,
                    shots = shots
                });

                try
                {
                    context.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    MessageBox.Show("Competition name and date must be unique.", "Warning!");
                }
            }

            ClearCompTab();
            UpdateCompGrid();

            txbCompName.Focus();

            UpdateCompetitionBox2();
        }

        private void btnCompAdd_Click(object sender, RoutedEventArgs e)
        {
            AddCompetition();
        }

        // Update competition record
        private void UpdateCompetition()
        {
            DateTime date;
            int shots;

            if (txbCompId.Text == string.Empty)
            {
                MessageBox.Show("Please select competition", "Warning!");
                return;
            }
            if (txbCompName.Text == string.Empty)
            {
                MessageBox.Show("Competition name can't be empty.", "Warning!");
                return;
            }
            if (!DateTime.TryParse(dapCompDate.Text, out date))
            {
                MessageBox.Show("Invalid competition date.", "Warning!");
                return;
            }
            if (!int.TryParse(txbCompShots.Text, out shots))
            {
                MessageBox.Show("Number of shots must be integer.", "Warning!");
                return;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                competition competition = (competition)competitionsGrid.SelectedItem;
                competition = context.competitions.FirstOrDefault(c => c.id == competition.id);
                competition.name = txbCompName.Text;
                competition.date = date;
                competition.shots = shots;

                try
                {
                    context.SaveChanges();
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    MessageBox.Show("Competition name and date must be unique.", "Warning!");
                }

            }

            ClearCompTab();
            UpdateCompGrid();

            txbCompName.Focus();

            UpdateCompetitionBox2();

            //Update both grids in competition
            UpdateSeekData();
        }

        private void btnCompUpd_Click(object sender, RoutedEventArgs e)
        {
            UpdateCompetition();
        }

        //Delete competition
        private void CompDelete()
        {
            competition competition = (competition)competitionsGrid.SelectedItem;

            if (competition == null)
            {
                MessageBox.Show("Please select competition", "Warning!");
                return;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                // Remove all rows in Results by FK
                context.results.RemoveRange(context.results.Where(r => r.cid == competition.id));

                // Remove competition
                competition = context.competitions.FirstOrDefault(c => c.id == competition.id);
                context.competitions.Remove(competition);
                context.SaveChanges();
            }
            ClearCompTab();
            UpdateCompGrid();

            UpdateCompetitionBox2();

            //Update both grids in competition
            UpdateSeekData();
        }

        // Delete competition from DB
        private void btnCompDel_Click(object sender, RoutedEventArgs e)
        {
            CompDelete();
        }

        // Competition row selection event handler
        private void competitionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            competition competition = (competition)competitionsGrid.SelectedItem;

            if (competition != null)
            {
                txbCompId.Text = competition.id.ToString();
                txbCompName.Text = competition.name.ToString();
                dapCompDate.Text = competition.date.ToString();
                txbCompShots.Text = competition.shots.ToString();
            }
        }
        /***************************************************SHOOTER_DBM**********************************************************/

        // Add shooter to DB
        private long AddShooter()
        {
            long sid = -1;

            if (txbShooId.Text != string.Empty)
            {
                MessageBox.Show("Press new buton, create new shooter then press add", "Warning!");
                return -1;
            }
            if (txbShooName.Text == string.Empty)
            {
                MessageBox.Show("Shooter name cant be empty", "Warning!");
                return -1;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                shooter shooter1 = new shooter();
                shooter1.name = txbShooName.Text;
                shooter1.surname = txbShooSname.Text != string.Empty ? txbShooSname.Text : null;
                shooter1.note = txbShooNote.Text;

                if (cbxShooTeam.SelectedIndex == -1 || cbxShooTeam.SelectedItem == null)
                {
                    shooter1.tid = 1;
                }
                else
                {
                    shooter1.tid = (long)cbxShooTeam.SelectedValue;
                }
                context.shooters.Add(shooter1);
                context.SaveChanges();

                sid = shooter1.id;
            }
            //Update both grids in competition
            UpdateSeekData();

            ClearShooTab();
            UpdateShooGrid();

            return sid;
        }

        private void btnShooAdd_Click(object sender, RoutedEventArgs e)
        {
            long id = AddShooter();

            if (quickEditing && id > -1)
            {
                AddShooterToComp(id);

                UnfrezeAll();
                txbNameSer_TextChanged(this, null);
                return;
            }
            txbNameSer_TextChanged(this, null);
        }

        // Update shooter record
        private long UpdateShooter()
        {
            long id = -1;

            if (txbShooId.Text == string.Empty)
            {
                MessageBox.Show("Please select shooter", "Warning!");
                return -1;
            }
            if (txbShooName.Text == string.Empty)
            {
                MessageBox.Show("Shooter name cant be empty", "Warning!");
                return -1;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                shooterscoview shooterco = (shooterscoview)shootersGrid.SelectedItem;
                id = shooterco.id;

                shooter shooter1 = context.shooters.FirstOrDefault(c => c.id == shooterco.id);
                shooter1.name = txbShooName.Text;
                shooter1.surname = txbShooSname.Text != string.Empty ? txbShooSname.Text : null;
                if (cbxShooTeam.SelectedIndex == -1 || cbxShooTeam.SelectedItem == null)
                {
                    shooter1.tid = 1;
                }
                else
                {
                    shooter1.tid = (long)cbxShooTeam.SelectedValue;
                }
                shooter1.note = txbShooNote.Text != string.Empty ? txbShooNote.Text : null;
                context.SaveChanges();
            }
            ClearShooTab();
            UpdateShooGrid();

            //Update both grids in competition
            UpdateSeekData();
            return id;
        }

        private void btnShooUpd_Click(object sender, RoutedEventArgs e)
        {
            long id = UpdateShooter();

            if (quickEditing)
            {
                UnfrezeAll();

                List<shooterscoview> shooList = (List<shooterscoview>)shootersGrid2.ItemsSource;
                for (int i = 0; i < shooList.Count; i++)
                {
                    if (shooList[i].id == id)
                    {
                        shootersGrid2.SelectedIndex = i;
                        txbNameSer_TextChanged(this, null);
                        return;
                    }
                }
                txbNameSer_TextChanged(this, null);
                return;
            }

            txbShooName.Focus();
            //Event
            txbNameSer_TextChanged(this, null);
        }

        // Delete shooter
        private void ShooDelete()
        {
            shooterscoview shooterco = (shooterscoview)shootersGrid.SelectedItem;

            if (shooterco == null)
            {
                MessageBox.Show("Please select shooter", "Warning!");
                return;
            }

            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                // Delete shooter from all results. Warning deletes all data of specified shooter 
                context.results.RemoveRange(context.results.Where(r => r.sid == shooterco.id));

                // Delete shooter from shooter table                
                shooter shooter = context.shooters.FirstOrDefault(c => c.id == shooterco.id);
                context.shooters.Remove(shooter);
                context.SaveChanges();
            }
            ClearShooTab();
            UpdateShooGrid();

            //Update both grids in competition
            UpdateSeekData();
        }

        // Delete shooter from DB
        private void btnShooDel_Click(object sender, RoutedEventArgs e)
        {
            ShooDelete();

            if (quickEditing)
            {
                UnfrezeAll();
                txbNameSer_TextChanged(this, null);
                return;
            }

            txbShooName.Focus();
            txbNameSer_TextChanged(this, null);
        }

        // Shooter row selection event handler
        private void shootersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            shooterscoview shooter = (shooterscoview)shootersGrid.SelectedItem;
            if (shooter != null)
            {
                txbShooId.Text = shooter.id.ToString();
                txbShooName.Text = shooter.name.ToString();
                txbShooSname.Text = shooter.surname != null ? shooter.surname.ToString() : null;
                cbxShooTeam.SelectedValue = shooter.tid;
                txbShooNote.Text = shooter.note != null ? shooter.note.ToString() : null;
            }
        }

        // Focus on shooters row being updated
        private void SeekSelectRow(long id)
        {
            tabCont.Focus();
            tabCont.SelectedIndex = 2;

            List<shooterscoview> shooList = (List<shooterscoview>)shootersGrid.ItemsSource;
            for (int i = 0; i < shooList.Count; i++)
            {
                if (shooList[i].id == id)
                {
                    shootersGrid.SelectedIndex = i;
                    return;
                }
            }
        }

        private void SeekSelectRow2(long id)
        {

            List<shooterscoview> shooList = (List<shooterscoview>)shootersGrid2.ItemsSource;
            for (int i = 0; i < shooList.Count; i++)
            {
                if (shooList[i].id == id)
                {
                    shootersGrid2.SelectedIndex = i;
                    return;
                }
            }
        }

        /***************************************************TEAM_DBM*************************************************************/

        // Add team to DB
        private void AddTeam()
        {
            long team_id = -1;

            if (txbTeamId.Text != string.Empty)
            {
                MessageBox.Show("Press new buton, create new team then press add.", "Warning!");
                return;
            }
            if (txbTeamName.Text == string.Empty)
            {
                MessageBox.Show("Team name cant be empty", "Warning!");
                return;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                team new_team = new team();
                new_team.name = txbTeamName.Text;
                context.teams.Add(new_team);

                try
                {
                    context.SaveChanges();
                    team_id = new_team.id;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                {
                    MessageBox.Show("Team name must be unique.", "Warning!");
                }
            }

            ClearTeamTab();
            UpdateTeamGrid();

            cbxShooTeam.SelectedValue = team_id;
        }

        private void btnTeamAdd_Click(object sender, RoutedEventArgs e)
        {
            AddTeam();
            TeamQuickEditing();
        }

        // Update team from DB
        private void UpdateTeam()
        {
            if (txbTeamId.Text == string.Empty)
            {
                MessageBox.Show("Please select team", "Warning!");
                return;
            }
            if (txbTeamName.Text == string.Empty)
            {
                MessageBox.Show("Team name cant be empty", "Warning!");
                return;
            }
            team team = (team)teamsGrid.SelectedItem;

            if (team.id != 1)
            {
                using (ShootappDBEntities context = new ShootappDBEntities())
                {
                    team = context.teams.FirstOrDefault(c => c.id == team.id);
                    team.name = txbTeamName.Text;

                    try
                    {
                        context.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
                    {
                        MessageBox.Show("Team name must be unique.", "Warning!");
                    }
                }
                ClearTeamTab();
                UpdateTeamGrid();
                UpdateShooGrid();

                //Update both grids in competition
                UpdateSeekData();
            }
        }

        private void btnTeamUpd_Click(object sender, RoutedEventArgs e)
        {
            UpdateTeam();
        }

        private void TeamDelete()
        {
            team team = (team)teamsGrid.SelectedItem;
            if (team == null)
            {
                MessageBox.Show("Please select team", "Warning!");
                return;
            }

            if (team.id != 1)
            {
                using (ShootappDBEntities context = new ShootappDBEntities())
                {
                    team = context.teams.FirstOrDefault(c => c.id == team.id);
                    context.teams.Remove(team);
                    context.SaveChanges();
                }
                ClearTeamTab();
                UpdateTeamGrid();
                UpdateShooGrid();

                //Update both grids in competition
                UpdateSeekData();
            }
        }


        void TeamQuickEditing()
        {
            if (!(tbiCompetition.IsEnabled && tbiCompetitionDBM.IsEnabled && tbiShooterDBM.IsEnabled) && !quickEditing)
            {
                tbiCompetition.IsEnabled = true;
                tbiCompetitionDBM.IsEnabled = true;
                tbiShooterDBM.IsEnabled = true;
                btnTeamUpd.IsEnabled = true;
                btnTeamDel.IsEnabled = true;
                tabCont.SelectedIndex = 2;

            }
            else if (!(tbiCompetition.IsEnabled && tbiCompetitionDBM.IsEnabled && tbiShooterDBM.IsEnabled) && quickEditing)
            {
                tbiShooterDBM.IsEnabled = true;
                tbiTeamDBM.IsEnabled = false;
                btnTeamUpd.IsEnabled = true;
                btnTeamDel.IsEnabled = true;
                tabCont.SelectedIndex = 2;

            }

        }

        // Delete team from DB
        private void btnTeamDel_Click(object sender, RoutedEventArgs e)
        {
            TeamDelete();
        }

        // Team row selection event handler
        private void teamsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            team team = (team)teamsGrid.SelectedItem;
            if (team != null)
            {
                txbTeamId.Text = team.id.ToString();
                txbTeamName.Text = team.name.ToString();
            }
        }

        /****************************************REMAINING**********************************************************************/

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        // Holds instance of opened window so you can only have one laded windoww
        WindowPrint window;

        private void btnPrint2_Click(object sender, RoutedEventArgs e)
        {
            if (window is null || !window.IsLoaded)
            {
                window = new WindowPrint(compId);
                window.Show();
            }
        }

        private void btnDeleteSin2_Click(object sender, RoutedEventArgs e)
        {
            competitionview comView = (competitionview)shootersSingleGrid.SelectedItem;

            if (comView == null)
            {
                MessageBox.Show("Please select shooter for removal.", "Warning!");
                return;
            }
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                result result = context.results.FirstOrDefault(r => r.cid == comView.comid && r.sid == comView.shoid);
                context.results.Remove(result);
                context.SaveChanges();
            }
            UpdateCompetitionGrid2();
            UpdateCompetitionGridSingle2();
        }

        private void shootersSingleGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int regShotsInt = 0;

            //double finScoreDou = 0.0;

            competitionview comrow = (competitionview)shootersSingleGrid.SelectedItem;

            FlipStringHits(comrow);

            if (comrow == null)
            {
                return;
            }
            if (comrow.hits != null)
            {
                string[] xhits = comrow.hits.Split(':');

                for (int i = 0; i < shootBox.Length; i++)
                {
                    shootBox[i].Text = xhits[i];

                    switch (i)
                    {
                        case 0:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 1:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 2:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 3:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 4:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 5:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 6:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 7:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 8:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 9:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 10:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 11:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                    }
                }
                lblRegShots.Content = regShotsInt.ToString();

                lblFinalScore.Content = !(comrow.score is null) ? comrow.score.ToString() : "";

                FlipStringHits(comrow);

                /*CHANGE TEXT COLOR*/

                if (regShotsInt > int.Parse(lblShoo.Content.ToString()))
                {
                    lblRegShots.Foreground = Brushes.Red;
                    lblRegShots.FontWeight = FontWeights.Bold;
                }
                else if (regShotsInt == int.Parse(lblShoo.Content.ToString()))
                {
                    lblRegShots.Foreground = Brushes.Green;
                }
                else
                {
                    lblRegShots.Foreground = Brushes.Red;
                }
            }
            else
            {
                foreach (TextBox tb in shootBox)
                {
                    tb.Text = string.Empty;
                }
                lblRegShots.Content = string.Empty;
                lblFinalScore.Content = string.Empty;
            }
        }

        private void tbiTeaShoo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            competitionview comrow = null;

            if (tbiTeaShoo.SelectedIndex == 0)
            {
                comrow = (competitionview)competitionGrid.SelectedItem;
            }
            else if (tbiTeaShoo.SelectedIndex == 1)
            {
                comrow = (competitionview)shootersSingleGrid.SelectedItem;
            }

            int regShotsInt = 0;

            //double finScoreDou = 0.0;

            if (comrow == null)
            {
                return;
            }

            FlipStringHits(comrow);

            if (comrow.hits != null)
            {
                string[] xhits = comrow.hits.Split(':');

                for (int i = 0; i < shootBox.Length; i++)
                {
                    shootBox[i].Text = xhits[i];

                    switch (i)
                    {
                        case 0:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 1:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 2:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 3:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 4:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 5:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 6:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 7:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 8:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 9:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 10:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                        case 11:
                            regShotsInt += int.Parse(xhits[i]);
                            break;
                    }
                }
                lblRegShots.Content = regShotsInt.ToString();

                lblFinalScore.Content = !(comrow.score is null) ? comrow.score.ToString() : "";

                FlipStringHits(comrow);

                /*CHANGE TEXT COLOR*/
                if (regShotsInt > int.Parse(lblShoo.Content.ToString()))
                {
                    lblRegShots.Foreground = Brushes.Red;
                    lblRegShots.FontWeight = FontWeights.Bold;
                }
                else if (regShotsInt == int.Parse(lblShoo.Content.ToString()))
                {
                    lblRegShots.Foreground = Brushes.Green;
                }
                else
                {
                    lblRegShots.Foreground = Brushes.Red;
                }
            }
            else
            {
                foreach (TextBox tb in shootBox)
                {
                    tb.Text = string.Empty;
                }
                lblRegShots.Content = string.Empty;
                lblFinalScore.Content = string.Empty;
            }
        }

        private void btnNewTeam_Click(object sender, RoutedEventArgs e)
        {
            tabCont.SelectedIndex = 3;
            ClearTeamTab();

            tbiCompetition.IsEnabled = false;
            tbiCompetitionDBM.IsEnabled = false;
            tbiShooterDBM.IsEnabled = false;
            tbiTeamDBM.IsEnabled = true;

            btnTeamUpd.IsEnabled = false;
            btnTeamDel.IsEnabled = false;

        }

        private void btnNewComp2_Click(object sender, RoutedEventArgs e)
        {
            tabCont.SelectedIndex = 2;
            FreezeTabs();
            ClearShooTab();

            btnShooUpd.IsEnabled = false;
            btnShooDel.IsEnabled = false;

            btnCancelQuick.Visibility = Visibility.Visible;
        }

        private void tabCont_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == "Return")
            {

                switch (tabCont.SelectedIndex)
                {
                    case 0:
                        btnSetShots_Click(this, null);
                        break;
                    case 1:
                        if (txbCompId.Text == String.Empty)
                        {
                            AddCompetition();
                        }
                        else
                        {
                            UpdateCompetition();
                        }
                        break;
                    case 2:
                        if (txbShooId.Text == String.Empty)
                        {
                            AddShooter();
                        }
                        else
                        {
                            UpdateShooter();
                        }
                        break;
                    case 3:
                        if (txbTeamId.Text == String.Empty)
                        {
                            AddTeam();
                        }
                        else
                        {
                            UpdateTeam();
                        }
                        break;
                }
            }
            else if (e.Key.ToString() == "Delete")
            {
                switch (tabCont.SelectedIndex)
                {
                    case 1:
                        if (txbCompId.Text != String.Empty)
                        {
                            CompDelete();
                        }
                        break;
                    case 2:
                        if (txbShooId.Text != String.Empty)
                        {
                            ShooDelete();
                        }
                        break;
                    case 3:
                        if (txbTeamId.Text != String.Empty)
                        {
                            TeamDelete();
                        }
                        break;
                }
            }
        }

        // Delete all Teams
        private void btnTeamDelAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You are about to delete whole Team table.", "Warrning!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                using (ShootappDBEntities context = new ShootappDBEntities())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM Teams WHERE id != 1");
                }
            }

            // Update Grids
            UpdateTeamGrid();
            UpdateShooGrid();
            UpdateSeekData();
        }

        // Delete all shooters
        private void btnShooDelAll_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("You are about to delete whole Shooters table.", "Warrning!", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
            {
                using (ShootappDBEntities context = new ShootappDBEntities())
                {
                    context.Database.ExecuteSqlCommand("DELETE FROM Shooters");
                }
            }

            // Update Grids
            UpdateTeamGrid();
            UpdateShooGrid();
            UpdateSeekData();
        }

        // Import from csv file
        private void btnImportAll_Click(object sender, RoutedEventArgs e)
        {
            SDBImporter sdbimp = new SDBImporter();
            sdbimp.ImportData();

            // Update Grids
            UpdateTeamGrid();
            UpdateShooGrid();
            UpdateSeekData();

        }

        private void UnfrezeAll()
        {

            tbiCompetition.IsEnabled = true;
            tbiCompetitionDBM.IsEnabled = true;
            tbiShooterDBM.IsEnabled = true;
            tbiTeamDBM.IsEnabled = true;

            btnImportAll.IsEnabled = true;
            btnShooDelAll.IsEnabled = true;
            btnTeamDelAll.IsEnabled = true;

            btnShooAdd.IsEnabled = true;
            btnShooUpd.IsEnabled = true;
            btnShooDel.IsEnabled = true;
            btnShooClear.IsEnabled = true;

            quickEditing = false;

            tabCont.SelectedIndex = 0;

            btnCancelQuick.Visibility = Visibility.Hidden;
        }

        // freeze tabs when quick editing
        private void FreezeTabs()
        {
            tbiCompetition.IsEnabled = false;
            tbiCompetitionDBM.IsEnabled = false;
            tbiTeamDBM.IsEnabled = false;

            btnImportAll.IsEnabled = false;
            btnShooDelAll.IsEnabled = false;
            btnTeamDelAll.IsEnabled = false;

            quickEditing = true;
        }

        // Makes only 1 focus
        int selectedIndex = 0;

        private void tabCont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (tabCont.SelectedIndex == 0 && selectedIndex != 0)
            {
                selectedIndex = 0;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    txbNameSer.Focus();
                }));

            }
            else if (tabCont.SelectedIndex == 1 && selectedIndex != 1)
            {
                selectedIndex = 1;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    txbCompName.Focus();
                }));
            }
            else if (tabCont.SelectedIndex == 2 && selectedIndex != 2)
            {
                selectedIndex = 2;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    txbShooName.Focus();
                }));
            }
            else if (tabCont.SelectedIndex == 3 && selectedIndex != 3)
            {
                selectedIndex = 3;

                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                {
                    txbTeamName.Focus();
                }));
            }
        }

        private void btnClearShots_Click(object sender, RoutedEventArgs e)
        {
            foreach (TextBox tb in shootBox)
            {
                tb.Text = string.Empty;
            }
        }

        private void countShots()
        {
            int shots = 0;
            int parsed;

            foreach (TextBox tb in shootBox)
            {
                if (int.TryParse(tb.Text, out parsed))
                {
                    shots += parsed;
                }
            }

            lblRegShots.Content = shots.ToString();
        }

        private void txb0_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb1_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb2_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb3_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb4_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb5_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb6_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb7_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb8_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb9_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb10_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void txb11_TextChanged(object sender, TextChangedEventArgs e)
        {
            countShots();
        }

        private void BtnCancelQuick_Click(object sender, RoutedEventArgs e)
        {
            UnfrezeAll();
        }
    }
}
