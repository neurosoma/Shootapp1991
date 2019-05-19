using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

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

                if (getInfoQ.Count == 0)
                {
                    this.Close();
                    return;
                }

                cname = getInfoQ[0].name;
                cdate = getInfoQ[0].date;

                var singleQ = context.singleprintviews.Where(s => s.cid == compId && s.hits != null).OrderByDescending(s => s.score);
                singlePrintList = singleQ.ToList();

                var teamQ = context.teamprintviews.Where(t => t.cid == compId && t.hits != null);
                teamPrintList = teamQ.ToList();

                var tidQ = context.teamdistviews.Where(t => t.cid == compId);
                teamIds = tidQ.ToList();

            }
            FillTextSingle();
            FillTextTeam();
        }


        //CODE FOR CREATING FIXED DOCUMENT
        private void FillTextTeam()
        {
            int rank = 1;
            int blocks_on_page = 10;
            int counter = 10;

            FixedPage page = null;
            TextBlock pageText = null;
            PageContent pageContent = null;
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

            foreach (TeamPrint t in TeamsL)
            {
                t.GlobalRank = rank;

                if (counter % blocks_on_page == 0)
                {
                    page = new FixedPage();
                    pageText = new TextBlock();
                    pageContent = new PageContent();
                    pageText.FontFamily = new FontFamily("Consolas");

                    pageText.Text = String.Format("\n   {0}, {1}{2,70}. stran\n\n", cname, cdate.ToShortDateString(), counter / blocks_on_page);

                    pageText.Text += String.Format("   {0} {1}{2,20}{3,4}{4,4}{5,4}{6,4}{7,4}{8,4}{9,4}{10,4}{11,4}{12,4}{13,4}{14,13}{15,13}\n", "MESTO", "EKIPA", "M", 10, 9, 8, 7, 6, 5, 4, 3, 2, 1, 0, "STRELI", "REZULTAT");
                }

                pageText.Text += t.ToString();

                if (counter % blocks_on_page == 0)
                {
                    page.Children.Add(pageText);
                    ((IAddChild)pageContent).AddChild(page);
                    fixed_document.Pages.Add(pageContent);
                }

                rank++;
                counter++;
            }
        }

        //CODE FOR CREATING FIXED DOCUMENT SINGLE
        private void FillTextSingle()
        {
            int rank = 1;
            int blocks_on_page = 34;
            int counter = 34;

            FixedPage page = null;
            TextBlock pageText = null;
            PageContent pageContent = null;

            TeamPrint.CheckIfEqualSingle(singlePrintList);         
                   
            foreach (singleprintview spv in singlePrintList)
            {

                if (counter % blocks_on_page == 0)
                {
                    page = new FixedPage();
                    pageText = new TextBlock();
                    pageContent = new PageContent();

                    pageText.FontFamily = new FontFamily("Consolas");

                    pageText.Text = String.Format("\n   {0}, {1}{2,70}. stran\n\n", cname, cdate.ToShortDateString(), counter / blocks_on_page);

                    pageText.Text += String.Format("{0,10}{1,25}{2,30}{3,15}{4,15}\n\n", "MESTO", "IME IN PRIIMEK", "STRELI", "ŠT.STRELOV", "REZULTAT");
                }

                pageText.Text += String.Format("{0,10}{1,25}{2,30}{3,15}{4,15}\n\n", rank + ".", spv.name + " " + spv.surname, string.Join(":", spv.hits.Split(':').Reverse().ToArray()), spv.shots, spv.score);

                if (counter % blocks_on_page == 0)
                {
                    page.Children.Add(pageText);
                    ((IAddChild)pageContent).AddChild(page);
                    fixed_doc_single.Pages.Add(pageContent);
                }

                rank++;
                counter++;
            }
        }
    }
}
