using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using System;
using System.Collections.Generic;

namespace PvReport.Services
{
    public class MailRepository
    {
        private readonly string _mailServer, _login, _password;
        private readonly ProgressNotificationService _progressNotificationService;
        private readonly int _port;
        private readonly bool _ssl;

        public MailRepository(string mailServer, int port, bool ssl, string login, string password,
            ProgressNotificationService progressNotificationService)
        {
            _mailServer = mailServer;
            _port = port;
            _ssl = ssl;
            _login = login;
            _password = password;
            _progressNotificationService = progressNotificationService;
        }

        public IEnumerable<string> GetUnreadMails()
        {
            var messages = new List<string>();

            using (var client = new ImapClient())
            {
                client.Connect(_mailServer, _port, _ssl);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(_login, _password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                var results = inbox.Search(SearchOptions.All, SearchQuery.Not(SearchQuery.Seen));
                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message.HtmlBody);
                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }

        public IEnumerable<MimeMessage> GetAllMails(DateTime fromDate, string gmailLabel)
        {
            var messages = new List<MimeMessage>();

            using (var client = new ImapClient())
            {
                _progressNotificationService.Notify("Verbinde mit Mailserver...");
                client.Connect(_mailServer, _port, _ssl);

                _progressNotificationService.Notify("Melde Benutzer an Mailserver an...");
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                client.Authenticate(_login, _password);

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                
                var sq = SearchQuery.HasGMailLabel(gmailLabel)
                    .And(SearchQuery.DeliveredAfter(fromDate));

                var results = inbox.Search(SearchOptions.All, sq);
                _progressNotificationService.Notify($"Starte Download von {results.Count} Report-EMails.");

                var count = 1;
                foreach (var uniqueId in results.UniqueIds)
                {
                    _progressNotificationService.Notify($"Lade Email {count}/{results.Count} herunter.");
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message);

                    count++;
                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }
    }
}