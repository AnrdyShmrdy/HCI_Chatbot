using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace GenerateTicket
{
//namespace Microsoft.Bot.Components.Samples.MultiplyDialog
//{
    /// <summary>
    /// Generates a ticket number for the customer.
    /// </summary>
    public class GenerateTicket : Dialog
    {
        [JsonConstructor]
        public GenerateTicket([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
            : base()
        {
            // enable instances of this command as debug break point
            RegisterSourceLocation(sourceFilePath, sourceLineNumber);
        }

        /// <summary>
        /// Gets the unique name (class identifier) of this trigger.
        /// </summary>
        /// <remarks>
        /// There should be at least a .schema file of the same name.  There can optionally be a
        /// .uischema file of the same name that describes how Composer displays this trigger.
        /// </remarks>
        [JsonProperty("$kind")]
        public const string Kind = "GenerateTicket";

        /// <summary>
        /// Gets or sets caller's memory path to store the file path of this step in (ex: conversation.area).
        /// </summary>
        /// <value>
        /// Caller's memory path to store the file path of this step in (ex: conversation.area).
        /// </value>
        [JsonProperty("filePath")]
        public StringExpression FilePath { get; set; }

        /// <summary>
        /// Gets or sets caller's memory path to store the ticket number of this step in (ex: conversation.area).
        /// </summary>
        /// <value>
        /// Caller's memory path to store the ticket number of this step in (ex: conversation.area).
        /// </value>
        [JsonProperty("ticketNumber")]
        public ValueExpression TicketNumber { get; set; }

        /// <summary>
        /// Gets or sets caller's memory path to store the user name of this step in (ex: conversation.area).
        /// </summary>
        /// <value>
        /// Caller's memory path to store the user name of this step in (ex: conversation.area).
        /// </value>
        [JsonProperty("name")]
        public ValueExpression Name { get; set; }

        /// <summary>
        /// Gets or sets caller's memory path to store the user phone number of this step in (ex: conversation.area).
        /// </summary>
        /// <value>
        /// Caller's memory path to store the user phone number of this step in (ex: conversation.area).
        /// </value>
        [JsonProperty("phoneNumber")]
        public ValueExpression PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets caller's memory path to store the user email of this step in (ex: conversation.area).
        /// </summary>
        /// <value>
        /// Caller's memory path to store the user email of this step in (ex: conversation.area).
        /// </value>
        [JsonProperty("email")]
        public ValueExpression Email { get; set; }

        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Random rnd = new Random();
            var filePath = FilePath.GetValue(dc.State);
            var email = Email.GetValue(dc.State);
            var phoneNumber = PhoneNumber.GetValue(dc.State);
            var ticketNumber = rnd.Next(100000, 500000);
            var name = Name.GetValue(dc.State);
            //string filePath = @"c:\temp\MyTest.txt";

            // Delete the file if it exists.
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            static void AddText(FileStream fs, string value)
            {
                byte[] info = new UTF8Encoding(true).GetBytes(value);
                fs.Write(info, 0, info.Length);
            }

            //Create the file.
            using (FileStream fs = File.Create(filePath))
            {
                AddText(fs, "Ticket Number: " + ticketNumber.ToString());
                AddText(fs, "\n\tName: " + name.ToString());
                AddText(fs, "\n\tEmail: " + email.ToString());
                AddText(fs, "\n\tPhone Number: " + phoneNumber.ToString());
            }

            if (this.TicketNumber != null)
            {
                dc.State.SetValue(this.TicketNumber.GetValue(dc.State).ToString(), ticketNumber);
            }
            return dc.EndDialogAsync(result: ticketNumber, cancellationToken: cancellationToken);

        }
    }
}