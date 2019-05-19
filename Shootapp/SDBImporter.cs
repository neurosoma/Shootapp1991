using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;


namespace Shootapp
{
    class SDBImporter
    {
        public void ImportData()
        {
            string name;
            string surname;
            string team;

            string[] split_string;
            string read_string;
            string csvFile = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                csvFile = openFileDialog.FileName;
            }

            if (csvFile == null)
            {
                return;
            }

            try
            {
                using (StreamReader sr = new StreamReader(csvFile))
                {
                    // Checks if file has right structure
                    if ((read_string = sr.ReadLine()) != null)
                    {
                        split_string = read_string.Split(',');

                        name = split_string[0];
                        surname = split_string[1];
                        team = split_string[2];

                        if (name != "NAME" || surname != "SURNAME" || team != "TEAM")
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }

                    while ((read_string = sr.ReadLine()) != null)
                    {
                        split_string = read_string.Split(',');

                        name = split_string[0];
                        surname = split_string[1];
                        team = split_string[2];

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
                                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
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
                                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException)
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

