using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace Shootapp
{
    class TeamPrint : IComparable
    {
        // Shooters that belong to this team
        public List<teamprintview> ShootersInTeam { get; set; }

        private long team_penal = 0;

        public int CShots { get; set; }

        //Global rank of team set after sort
        public int GlobalRank { get; set; }

        public long Id { get; set; }

        string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value.ToUpper();
            }
        }

        public int TeamScore { get; set; }

        public int Total { get; set; }

        List<int> hits;
        public List<int> Hits
        {
            get
            {
                return hits;
            }
            set
            {
                Total = 0;
                hits = value;
                foreach (int i in hits)
                {
                    Total += i;
                }
            }
        }

        public TeamPrint(List<teamprintview> tList)
        {
            TeamScore = 0;

            if (tList != null)
            {
                Name = tList[0].tname;
                ShootersInTeam = tList;
                ConstructTotal(ShootersInTeam);
                CheckIfEqualSingle(ShootersInTeam);                

                foreach (teamprintview tp in tList)
                {
                    if (tp.penal != null)
                    {
                        team_penal += (long)tp.penal;
                    }
                }
            }
        }

        public override string ToString()
        {
            //shooter rank
            int rank = 1;

            string outprint = "   ---------------------------------------------------------------------------------------------------------\n";

            outprint += String.Format("   {0,5}. {1,-20}{2,4}{3,4}{4,4}{5,4}{6,4}{7,4}{8,4}{9,4}{10,4}{11,4}{12,4}{13,4}{14,13}{15,13}", GlobalRank, Name.Length > 20 ?  Name.Substring(0, 20) : Name, Hits[11], Hits[10], Hits[9], Hits[8], Hits[7], Hits[6], Hits[5], Hits[4], Hits[3], Hits[2], Hits[1], Hits[0], Total, TeamScore);


            outprint += "\n\n";

            foreach (teamprintview tpv in ShootersInTeam)
            {
                string[] singleHits = tpv.hits.Split(':');
                outprint += String.Format("   {0,25}{1,6}{2,4}{3,4}{4,4}{5,4}{6,4}{7,4}{8,4}{9,4}{10,4}{11,4}{12,4}{13,13}{14,13}\n", (tpv.name + " " + tpv.surname).Length > 25 ? (tpv.name + " " + tpv.surname).Substring(0, 20) : tpv.name + " " + tpv.surname, singleHits[11], singleHits[10], singleHits[9], singleHits[8], singleHits[7], singleHits[6], singleHits[5], singleHits[4], singleHits[3], singleHits[2], singleHits[1], singleHits[0], tpv.shots, tpv.score);
                rank++;
            }
            outprint += "\n";
            return outprint;
        }

        // Construct total hits string
        private void ConstructTotal(List<teamprintview> tList)
        {
            List<int> totalHits = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            foreach (teamprintview tpv in tList)
            {
                List<int> hits = DeconstructHits(tpv.hits);

                for (int n = 0; n < hits.Count; n++)
                {
                    totalHits[n] += hits[n];
                }

                TeamScore += (int)tpv.score;
            }
            Hits = totalHits;
        }

        // Finds equilibrium in single under team and sort it
        // OVERLOADED    
        public static void CheckIfEqualSingle(List<teamprintview> tList)
        {
            teamprintview temp;

            for (int i = 0; i < tList.Count; i++)
            {
                for (int j = i + 1; j < tList.Count; j++)
                {
                    if (tList[i].score < tList[j].score) {
                        temp = tList[i];
                        tList[i] = tList[j];
                        tList[j] = temp;                       
                    }
                    if (tList[i].score == tList[j].score)
                    {
                        List<int> hitsA = DeconstructHits(tList[i].hits);
                        List<int> hitsB = DeconstructHits(tList[j].hits);

                        hitsA.Reverse();
                        hitsB.Reverse();

                        for (int k = 0; k < hitsA.Count; k++)
                        {
                            if (hitsA[k] < hitsB[k])
                            {
                                temp = tList[i];
                                tList[i] = tList[j];
                                tList[j] = temp;
                                break;
                            }
                            else if (hitsA[k] > hitsB[k])
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Finds equilibrium in single and sort it
        // OVERLOADED
        public static void CheckIfEqualSingle(List<singleprintview> sList)
        {
            singleprintview temp;

            for (int i = 0; i < sList.Count; i++)
            {
                for (int j = i + 1; j < sList.Count; j++)
                {
                    if (sList[i].score < sList[j].score)
                    {
                        temp = sList[i];
                        sList[i] = sList[j];
                        sList[j] = temp;
                    }
                    if (sList[i].score == sList[j].score)
                    {
                        List<int> hitsA = DeconstructHits(sList[i].hits);
                        List<int> hitsB = DeconstructHits(sList[j].hits);

                        hitsA.Reverse();
                        hitsB.Reverse();

                        for (int k = 0; k < hitsA.Count; k++)
                        {
                            if (hitsA[k] < hitsB[k])
                            {           
                                temp = sList[i];
                                sList[i] = sList[j];
                                sList[j] = temp;
                                break;
                            }
                            else if (hitsA[k] > hitsB[k])
                            {                                
                                break;
                            }
                        }
                    }
                }
            }
        }

        // Deconstruct string from DB that contains hits
        public static List<int> DeconstructHits(string hitss)
        {
            string[] strList = hitss.Split(':');

            List<int> tmp = new List<int>();

            foreach (string st in strList)
            {
                tmp.Add(int.Parse(st));
            }
            return tmp;
        }

        public int CompareTo(object obj)
        {
            TeamPrint tp = obj as TeamPrint;

            if (this.TeamScore > tp.TeamScore)
            {
                return 1;
            }
            else if (this.TeamScore < tp.TeamScore)
            {
                return -1;
            }
            else if (this.TeamScore == tp.TeamScore)
            {
                for (int i = this.Hits.Count - 1; i > 0; i--)
                {
                    if (this.Hits[i] == tp.Hits[i])
                    {
                        continue;
                    }
                    else if (this.Hits[i] < tp.Hits[i])
                    {
                        return -1;
                    }
                    else if (this.Hits[i] > tp.Hits[i])
                    {
                        return 1;
                    }
                }
            }
            return 0;
        }        
    }
}
