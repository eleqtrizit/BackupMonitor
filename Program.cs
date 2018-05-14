using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;
using System.ComponentModel;

namespace BackupMonitor
{
    class Program
    {

        static void Main(string[] args)
        {
            DateTime from_date = DateTime.Now.AddDays(-1);
            DateTime to_date = DateTime.Now;
            //Console.WriteLine(to_date.ToString("ddd"));
            //Console.ReadLine();

            string line;
            bool newFileFound = false;
            Dictionary<string, string> conf = new Dictionary<string, string>();
            StringBuilder missingBackups = new StringBuilder();

            StreamReader config = File.OpenText(@"BackupCheck.cfg");
            while ((line = config.ReadLine()) != null)
            {
                // skip blank lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }
                string[] items = line.Split('=');
                conf[items[0]] = items[1];
            }


            StreamReader reader = File.OpenText(conf["checklist"]);
            while ((line = reader.ReadLine()) != null)
            {
                // skip blank lines
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                newFileFound = false;
                string[] items = line.Split('\t');
                string directory = items[0]; // directory of files
                string search = items[1]; // wildcard search of files

                string[] files = Directory.GetFiles(directory, search);
                Console.WriteLine("Inspecting " + directory + " with search term " + search);

                foreach (string file in files)
                {
                    DateTime modification = File.GetLastWriteTime(file);
                    if (modification > from_date)
                    {
                        newFileFound = true;
                    }
                }

                if (newFileFound == false)
                {
                    Console.WriteLine("No new file found.  Backup error!");
                    missingBackups.Append("No backup found: ");
                    missingBackups.Append(directory);
                    missingBackups.Append(@"\");
                    missingBackups.Append(search);
                    missingBackups.Append("\n\r");
                }
            }

            MailAddress from = new MailAddress(conf["from"]);
            MailAddress to = new MailAddress(conf["to"]);
            MailMessage message = new MailMessage(from, to);
            
            
            // no errors found so far, so it's OK
            if (missingBackups.Length<1)
            {
                message.Subject = "Backups are OK!";
                missingBackups.Append("All backups are present and accounted for.");
            }
            else
            {
                message.Subject = "Backup Errors";
                
            }

            missingBackups.Append("\n\nThis is being sent from the BackMonitor.exe.  Set to run via the Task Scheduler.\n");
            missingBackups.Append(@"Configuration file is " + conf["checklist"]);
            message.Body = missingBackups.ToString();
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(conf["mailserver"]);
            smtp.Send(message);

        }
    }
}
