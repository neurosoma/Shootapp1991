using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Shootapp
{
    /// <summary>
    /// Interaction logic for WindowPrint.xaml
    /// </summary>
    public partial class WindowPrint : Window
    {
        long compId;
        string cname;
        DateTime cdate;

        List<singleprintview> singlePrintList;
        List<teamprintview> teamPrintList;
        List<teamdistview> teamIds;

        //All teams ordered
        List<TeamPrint> TeamsL;

        public WindowPrint(long compId)
        {
            InitializeComponent();
            this.compId = compId;
            TeamsL = new List<TeamPrint>();
        }

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

        // When window first load
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                //Sets competition details
                List<competition> getInfoQ = context.competitions.Where(c => c.id == compId).ToList();
                cname = getInfoQ[0].name;
                cdate = getInfoQ[0].date;

                var singleQ = context.singleprintviews.Where(s => s.cid == compId && s.hits != null).OrderByDescending(s => s.score);
                singlePrintList = singleQ.ToList();

                var teamQ = context.teamprintviews.Where(t => t.cid == compId && t.hits != null);
                teamPrintList = teamQ.ToList();

                var tidQ = context.teamdistviews.Where(t => t.cid == compId);
                teamIds = tidQ.ToList();

                
                // Connect alternative grids
                var query2 = from cv in context.competitionviews
                             where cv.comid == compId
                             select cv;

                if (query2 != null)
                {
                    List<competitionview> qcv = query2.ToList();

                    FlipStringHits(qcv);

                    shootersSingleGrid.ItemsSource = qcv;

                    ListCollectionView view = new ListCollectionView(qcv);
                    view.GroupDescriptions.Add(new PropertyGroupDescription("teaname"));

                    competitionGrid.ItemsSource = view;
                }                               
            }
            FillTextSingle();
            FillTextTeam();
        }
        

        // Create teams and text
        private void FillTextTeam()
        {
            txbTeam.Text = String.Format("{0}, {1}\n\n", cname, cdate.ToShortDateString());

            txbTeam.Text += String.Format("{0} {1}{2,20}{3,4}{4,4}{5,4}{6,4}{7,4}{8,4}{9,4}{10,4}{11,4}{12,4}{13,4}{14,10}{15,10}{16,10}\n", "MESTO", "EKIPA", "M", 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, "X", "STRELI", "ODBITEK", "REZULTAT");

            TeamPrint tp;

            using (ShootappDBEntities context = new ShootappDBEntities())
            {
                foreach (teamdistview tdw in teamIds)
                {
                    List<teamprintview> tempTeam = context.teamprintviews.Where(t => t.tid == tdw.tid && t.cid == compId && t.hits != null).ToList();

                    if (tempTeam.Count == 0)
                    {
                        continue;
                    }
                    else
                    {
                        tp = new TeamPrint(tempTeam);
                        TeamsL.Add(tp);
                    }

                }
            }

            TeamsL.Sort();
            TeamsL.Reverse();

            int rank = 1;

            foreach (TeamPrint t in TeamsL)
            {
                t.GlobalRank = rank;
                txbTeam.Text += t.ToString();
                rank++;
            }
        }

        // Create single text
        private void FillTextSingle()
        {
            int rank = 1;

            TeamPrint.CheckIfEqualSingle(singlePrintList);

            txbSingle.Text += cname + ", " + cdate.ToShortDateString() + "\n\n";

            txbSingle.Text += String.Format("{0,10}{1,25}{2,30}{3,15}{4,15}{5,15}\n\n", "MESTO", "IME IN PRIIMEK", "STRELI", "ŠT.STRELOV", "ODBITEK", "REZULTAT");

            foreach (singleprintview spv in singlePrintList)
            {
                //txbSingle.Text += rank.ToString() + ". " + spv.name + " " + spv.surname + " (" + spv.score.ToString() + ")\n";
                txbSingle.Text += String.Format("{0,10}{1,25}{2,30}{3,15}{4,15}{5,15}\n\n", rank + ".", spv.name + " " + spv.surname, string.Join(":", spv.hits.Split(':').Reverse().ToArray()), spv.shots, spv.penal, spv.score);
                rank++;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                switch (tabRoot.SelectedIndex)
                {
                    case 0:
                        printDialog.PrintVisual(txbTeam, "Print Team");
                        break;
                    case 1:
                        printDialog.PrintVisual(txbSingle, "Print Individual");
                        break;
                    case 2:
                        printDialog.PrintVisual(competitionGrid, "Print Team advanced");
                        break;
                    case 3:
                        printDialog.PrintVisual(shootersSingleGrid, "Print Individual advanced");
                        break;
                }
            }
        }
    }
}
