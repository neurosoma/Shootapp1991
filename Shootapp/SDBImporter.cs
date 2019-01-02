using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows;
using System.Data;


namespace Shootapp
{
    class SDBImporter
    {
        public void ImportData()
        {
            string name;
            string surname;
            string team;

            string read_string;

            string csvFile = @"C:\Shootapp1991\ImportCSV\ShootersTeams.csv";

            try
            {
                using (StreamReader sr = new StreamReader(csvFile))
                {
                    while ((read_string = sr.ReadLine()) != null)
                    {
                        string[] split_string = read_string.Split(',');

                        name = split_string[0];
                        surname = split_string[1];
                        team = split_string[2];

                        if (name.ToLower() == "name" && surname.ToLower() == "surname" && team.ToLower() == "team")
                        {
                            continue;
                        }

                        if (name != String.Empty && surname != String.Empty && team != String.Empty)
                        {
                            team teamIn = new team();
                            teamIn.name = team;

                            using (ShootappDBEntities context = new ShootappDBEntities())
                            {
                                context.teams.Add(teamIn);
                                try
                                {
                                    context.SaveChanges();
                                }
                                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException e)
                                {
                                    //Do nothing
                                }
                            }

                            using (ShootappDBEntities context = new ShootappDBEntities())
                            {

                                teamIn = context.teams.FirstOrDefault(t => t.name == teamIn.name);

                                shooter shooter1 = new shooter();
                                shooter1.name = name;
                                shooter1.surname = surname;
                                shooter1.tid = teamIn.id;

                                context.shooters.Add(shooter1);

                                try
                                {
                                    context.SaveChanges();
                                }
                                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException e)
                                {
                                    //Do nothing
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + "\n" + e.Message, "Warning");
            }
        }
    }
}

