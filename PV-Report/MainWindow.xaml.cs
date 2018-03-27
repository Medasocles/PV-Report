using HtmlAgilityPack;
using MailClient;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows;

namespace PV_Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> _fullMboxContent;
        private List<string> _contentStrings;
        private SolarWebReportEmail _currentMail;
        private List<SolarWebReportEmail> _allEmails;
        private string _directorySuffix;

        public MainWindow()
        {
            DataContext = this;

            InitializeComponent();



            _allEmails = new List<SolarWebReportEmail>();
            MboxFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Test", "PV Report.mbox");
        }

        public static readonly DependencyProperty MboxFilePathProperty = DependencyProperty.Register(
            nameof(MboxFilePath), typeof(string), typeof(MainWindow), new PropertyMetadata(default(string)));

        public string MboxFilePath
        {
            get => (string)GetValue(MboxFilePathProperty);
            set => SetValue(MboxFilePathProperty, value);
        }

        private void DownloadReport(string storePath, SolarWebReportEmail mail)
        {
            foreach (var mailDownloadLink in mail.DownloadLinks)
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(mailDownloadLink);

                try
                {
                    using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                    {
                        // "attachment; filename*=UTF-8''PV.Bauer_Produktion_Weekly_W%C3%B6chentlicher_Report_1.csv"
                        var fileName = httpWebResponse.Headers["Content-Disposition"];
                        if (fileName.Contains("*"))
                        {
                            // "UTF-8''PV.Bauer_Produktion_Weekly_W%C3%B6chentlicher_Report_1.csv"                   
                            fileName = fileName.Replace("attachment; filename*=", "");
                            var tokens = fileName.Split(new[] { "''" }, StringSplitOptions.RemoveEmptyEntries);
                            fileName = tokens[1];
                        }
                        else
                        {
                            // "UTF-8''PV.Bauer_Produktion_Weekly_W%C3%B6chentlicher_Report_1.csv"                   
                            fileName = fileName.Replace("attachment; filename=", "");

                        }

                        fileName = fileName.Replace("%C3%A4", "ä");
                        fileName = fileName.Replace("%C3%B6", "ö");
                        fileName = fileName.Replace("%C3%BC", "ü");
                        fileName = fileName.Replace("%C3%9F", "ß");

                        var directory = Path.Combine(storePath, mail.Date.Year.ToString());
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        var filePath = Path.Combine(directory, fileName);

                        using (var responseStream = httpWebResponse.GetResponseStream())
                        {
                            var downBuffer = new byte[1024 * 4];
                            using (var saveFileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
                                FileShare.ReadWrite))
                            {
                                int receivedBytes;
                                while ((receivedBytes = responseStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                                {
                                    saveFileStream.Write(downBuffer, 0, receivedBytes);
                                    saveFileStream.Flush();
                                }
                            }
                        }
                    }
                }
                catch (WebException e)
                {
                    mail.HasError = true;
                    mail.Exception = e;
                }
                catch (Exception exc)
                {

                }
            }
        }

        private void OnOpenMboxFileButtonClick(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "MBox-Dateien|*.mbox" };
            ofd.ShowDialog();

            MboxFilePath = ofd.FileName;
        }

        private void OnParseMboxFileButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MboxFilePath))
                return;

            ParseMboxFile();

            var mailsWithError = _allEmails.Where(mail => mail.HasError);

        }

        private void ParseMboxFile()
        {
            try
            {
                _fullMboxContent = File.ReadAllLines(MboxFilePath).ToList();
                _contentStrings = new List<string>();
                _directorySuffix = Guid.NewGuid().ToString();

                // todo: fix error with PV.csv filename when mail has two attachments

                for (var i = 0; i < _fullMboxContent.Count; i++)
                {
                    var line = _fullMboxContent[i];

                    // parse out full message
                    if (line.Contains("From: <noreply@solarweb.com>"))
                    {
                        _currentMail = new SolarWebReportEmail();
                        _allEmails.Add(_currentMail);

                        var innerCount = i;
                        var innerMessageLines = new List<string>();

                        while (true)
                        {
                            if (innerCount == _fullMboxContent.Count)
                                break;

                            var innerLine = _fullMboxContent[innerCount];

                            if (innerLine.StartsWith("Date: "))
                            {
                                _currentMail.Date = DateTime.Parse(innerLine.Replace("Date: ", ""));
                            }

                            // check subject so we only work with report-emails
                            else if (innerLine.StartsWith("Subject: "))
                            {
                                // "Subject: =?utf-8?B?U29sYXIud2ViIFJlcG9ydCBmw7xyIFBWLUFubGFnZSAgUFYg?="

                                var encodedSubject = innerLine;
                                if (!_fullMboxContent[innerCount + 1].StartsWith("Content-Type:"))
                                    encodedSubject += _fullMboxContent[innerCount + 1];
                                encodedSubject = encodedSubject.Replace("Subject: ", "");
                                var encSubTokens =
                                    encodedSubject.Split(new[] { "=?utf-8?B?", "?=", " " }, StringSplitOptions.RemoveEmptyEntries);

                                var subject = string.Empty;
                                try
                                {
                                    foreach (var token in encSubTokens)
                                    {
                                        subject += Encoding.UTF8.GetString(Convert.FromBase64String(token));
                                    }
                                }
                                catch
                                {
                                    subject = string.Join("", encodedSubject);
                                }

                                _currentMail.Subject = subject;
                                //StatusTextBlock.Text = subject;

                                //"Solar.web Report für PV-Anlage  PV "
                                if (!subject.StartsWith("Solar.web Report"))
                                {
                                    _currentMail.IsRejected = true;
                                    Trace.WriteLine("Rejected Subject: " + subject);
                                    break;
                                }
                            }

                            if (!innerLine.Contains("@xxx")) // From [...]@xxx 
                            {
                                innerMessageLines.Add(innerLine);
                                innerCount++;
                                continue;
                            }

                            _currentMail.Content = innerMessageLines;

                            // old style with attachement
                            if (innerMessageLines.Any(l => l.Contains("Content-Disposition: attachment")))
                            {
                                GetContentFromEmailWithAttachedReports(innerMessageLines);
                            }
                            else
                            // actual style with embedded download links
                            {
                                GetContentFromEmail(innerMessageLines);
                            }

                            i = innerCount;
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _currentMail.HasError = true;
                _currentMail.Exception = exception;
            }

            // all content read
            foreach (var solarWebMail in _allEmails.Where(mail => mail.DownloadLinks?.Count > 0))
            {
                DownloadReport(Path.Combine(Directory.GetCurrentDirectory(), $"Reports.{DateTime.Now:MM.dd.yyy}.{_directorySuffix}"), solarWebMail);
            }
        }

        private void GetContentFromEmail(List<string> emailLines)
        {
            for (int i = 0; i < emailLines.Count; i++)
            {
                var line = emailLines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    // get the message body
                    var innerCount = i + 1;
                    var innerContentLines = new List<string>();
                    while (true)
                    {
                        if (innerCount == emailLines.Count)
                            break;

                        var innerLine = emailLines[innerCount];
                        if (!string.IsNullOrWhiteSpace(innerLine))
                        {
                            innerContentLines.Add(innerLine);
                            innerCount++;
                            continue;
                        }

                        // after message content come two empty lines, so the innerContentLines is empty for the second empty line
                        if (innerContentLines.Count > 0)
                        {
                            var contentBase64String = Convert.FromBase64String(string.Join("", innerContentLines));
                            var contentString = Encoding.UTF8.GetString(contentBase64String);
                            contentString = contentString.Replace("%C3%A4", "ä");
                            contentString = contentString.Replace("%C3%B6", "ö");
                            contentString = contentString.Replace("%C3%BC", "ü");
                            contentString = contentString.Replace("%C3%9F", "ß");
                            _contentStrings.Add(contentString);

                            ExtractReportDownloadLinksFromContent(contentString);
                        }

                        break;
                    }
                }

            }

        }

        private void GetContentFromEmailWithAttachedReports(IReadOnlyList<string> emailLines)
        {
            var inBoundary = 0;
            var inAttachment = false;
            var inName = false;
            var attachmentLines = new List<string>();
            var nameLines = new List<string>();
            var fileName = string.Empty;

            for (var i = 0; i < emailLines.Count; i++)
            {
                var line = emailLines[i];

                // check if old-mail-style with attachments
                if (line.Contains("----boundary_"))
                {
                    inBoundary++;

                    if (inAttachment)
                    {
                        if (attachmentLines.Count > 0)
                        {
                            var strB64 = string.Join("", attachmentLines);
                            var fileContent = Encoding.UTF8.GetString(Convert.FromBase64String(strB64));

                            if (fileName.Equals("PV.csvPV.csv"))
                            { }

                            if (fileName.Equals("PV.csv"))
                            {
                                fileName = $"PV_{_currentMail.Date:MM.dd.yyy}_{_currentMail.Date:hh.mm.ss}.csv";
                                _currentMail.FileName = fileName;
                            }

                            var directory = Path.Combine(Directory.GetCurrentDirectory(),
                                $"Reports.{DateTime.Now:MM.dd.yyy}.{_directorySuffix}", _currentMail.Date.Year.ToString());
                            if (!Directory.Exists(directory))
                                Directory.CreateDirectory(directory);

                            using (var fs = new FileStream(Path.Combine(directory, fileName), FileMode.Create))
                            {
                                using (var writer = new StreamWriter(fs))
                                {
                                    writer.Write(fileContent);
                                    writer.Flush();
                                }
                            }
                        }

                        attachmentLines.Clear();
                        inAttachment = false;
                    }

                    continue;
                }

                if (inBoundary > 0 || inAttachment)
                {
                    if (line.Contains("Content-Disposition: attachment"))
                    {
                        inAttachment = true;
                    }

                    else if (line.Contains("Content-Type: application/octet-stream; name="))
                    {
                        inName = true;
                        nameLines.Add(line);
                    }

                    else if (line.Contains("Content-Transfer-Encoding"))
                    {
                        if (inName)
                        {
                            inName = false;
                            fileName = string.Empty;
                            var str = string.Join("", nameLines);

                            if (str.Contains("PV.csv"))
                            {
                                str = str.Replace("Content-Type: application/octet-stream; name=", "");
                                fileName = str.Replace("\"", "");
                            }
                            else
                            {
                                // "Content-Type: application/octet-stream; name=\t\"=?utf-8?B?RGFpbHktS1dILVJlcG9ydF9Uw6RnbGljaGVyX1JlcG9ydF8yMDE3XzA3?= =?utf-8?B?XzE1LmNzdg==?=\""
                                str = str.Replace("Content-Type: application/octet-stream; name=\t\"=", "");
                                var tokens = str.Split(new[] { "?utf-8?B?" }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var token in tokens)
                                {
                                    var innerTokens = token.Split('?');
                                    fileName += Encoding.UTF8.GetString(Convert.FromBase64String(innerTokens[0]));
                                }
                            }
                        }
                    }
                    else if (inName)
                    {
                        nameLines.Add(line);
                    }
                    else if (inAttachment)
                    {
                        attachmentLines.Add(line);
                    }
                }
            }
        }

        private void ExtractReportDownloadLinksFromContent(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var rows = doc.DocumentNode.Descendants("a");
            foreach (var htmlNode in rows)
            {
                _currentMail.DownloadLinks = htmlNode.Attributes.Where(attr => attr.Name.Equals("href")).Select(attr => attr.Value).ToList();
            }
        }

        private class SolarWebReportEmail
        {
            public DateTime Date { get; set; }
            public string Subject { get; set; }
            public string FileName { get; set; }
            public List<string> DownloadLinks { get; set; }
            public bool IsRejected { get; set; }

            public List<string> Content { get; set; }

            public bool HasError { get; set; }
            public Exception Exception { get; set; }
        }

        private void OnTryDirectMailConnection(object sender, RoutedEventArgs e)
        {
            var mailRepository = new MailRepository("imap.gmail.com", 993, true, "xxx@gmail.com", "xxx");
            var allEmails = mailRepository.GetAllMails();

            foreach (var email in allEmails)
            {
                Console.WriteLine(email);
            }
        }
    }
}
