using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PvReport.Services
{
    public class MailRepository
    {
        private readonly string _mailServer, _login, _password;
        private readonly int _port;
        private readonly bool _ssl;

        public MailRepository(string mailServer, int port, bool ssl, string login, string password)
        {
            _mailServer = mailServer;
            _port = port;
            _ssl = ssl;
            _login = login;
            _password = password;
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

        public IEnumerable<MimeKit.MimeMessage> GetAllMails(DateTime fromDate, string gmailLabel)
        {
            var messages = new List<MimeKit.MimeMessage>();

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
                
                var sq = SearchQuery.HasGMailLabel(gmailLabel)
                    .And(SearchQuery.DeliveredAfter(fromDate));

                var results = inbox.Search(SearchOptions.All, sq);

                foreach (var uniqueId in results.UniqueIds)
                {
                    var message = inbox.GetMessage(uniqueId);

                    messages.Add(message);

                    //Mark message as read
                    //inbox.AddFlags(uniqueId, MessageFlags.Seen, true);
                }

                client.Disconnect(true);
            }

            return messages;
        }
    }
}