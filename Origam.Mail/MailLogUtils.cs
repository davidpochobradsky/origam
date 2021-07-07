﻿using System.Collections.Specialized;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Origam.Mail
{
    public static class MailLogUtils
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        public static void SendMessageAndLog(SmtpClient client, MailMessage message)
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("Sending mail:");
                log.Debug(MailLogUtils.ToLogString(client));
                log.Debug( MailLogUtils.ToLogString(message));
            }
            client.Send(message);
            log.Debug("Mail sent");
        }

        public static string ToLogString(SmtpClient client)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"System.Net.Mail.SmtpClient:\n");
            builder.Append($"\tHost: {client.Host}\n");
            builder.Append($"\tPort: {client.Port}\n");
            builder.Append($"\tTimeout: {client.Timeout}\n");
            builder.Append($"\tClientCertificates.Count: {client.ClientCertificates.Count}\n");
            builder.Append($"\tDeliveryFormat: {client.DeliveryFormat}\n");
            builder.Append($"\tDeliveryMethod: {client.DeliveryMethod}\n");
            builder.Append($"\tEnableSsl: {client.EnableSsl}\n");
            builder.Append($"\tServicePoint.Address: {client.ServicePoint.Address}\n");
            builder.Append($"\tTargetName: {client.TargetName}\n");
            builder.Append($"\tPickupDirectoryLocation: {client.PickupDirectoryLocation}\n");
            builder.Append($"\tUseDefaultCredentials: {client.UseDefaultCredentials}\n");
            return builder.ToString();
        }
        
        public static string ToLogString(MailMessage message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append($"System.Net.Mail.MailMessage:\n");
            builder.Append($"\tSubject: {message.Subject}\n");
            builder.Append($"\tTo: {AddressesToString(message.To)}\n");
            builder.Append($"\tCC: {AddressesToString(message.CC)}\n");
            builder.Append($"\tBcc: {AddressesToString(message.Bcc)}\n");
            builder.Append($"\tReplyToList: {AddressesToString(message.ReplyToList)}\n");
            builder.Append($"\tFrom: {message.From}\n");
            builder.Append($"\tSender: {message.Sender}\n");
            builder.Append($"\tBody:\n {AddIndents(message.Body)}\n");
            builder.Append($"\tAttachments.Count: {message.Attachments.Count}\n");
            builder.Append($"\tIsBodyHtml: {message.IsBodyHtml}\n");
            builder.Append($"\tBodyEncoding: {message.BodyEncoding}\n");
            builder.Append($"\tBodyTransferEncoding: {message.BodyTransferEncoding}\n");
            builder.Append($"\tHeaders: {HeadersToString(message.Headers)}\n");
            builder.Append($"\tHeadersEncoding: {message.HeadersEncoding}\n");
            builder.Append($"\tSubjectEncoding: {message.SubjectEncoding}\n");
            builder.Append($"\tPriority: {message.Priority}\n");
            builder.Append($"\tAlternateViews.Count: {message.AlternateViews.Count}\n");
            builder.Append($"\tDeliveryNotificationOptions: {message.DeliveryNotificationOptions}\n");
            return builder.ToString();
        }

        private static string HeadersToString(NameValueCollection headers)
        {
            return $"Headers: [{string.Join(", ", headers.AllKeys.Select(key=> key+": "+headers[key]))}]";
        }

        private static string AddressesToString(MailAddressCollection mails)
        {
            return $"[{string.Join(", ", mails.Select(x => x.Address))}]";
        }

        private static string AddIndents(string message)
        {
            return string.Join("\n", message.Split('\n').Select(x=>"\t\t"+x));
        }
    }
}