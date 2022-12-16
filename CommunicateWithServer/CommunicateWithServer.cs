using System;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveExpressions.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace CommunicateWithServer
{
    /// <summary>
    /// Communicates with server
    /// </summary>
    public class CommunicateWithServer : Dialog
    {
        [JsonConstructor]
        public CommunicateWithServer([CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
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
        public const string Kind = "CommunicateWithServer";

        /// <summary>
        /// Gets or sets memory path to bind to message (ex: conversation.width).
        /// </summary>
        /// <value>
        /// Memory path to bind to message (ex: conversation.width).
        /// </value>
        [JsonProperty("message")]
        public StringExpression Message { get; set; }

        /// <summary>
        /// Gets or sets caller's memory path to store the result of this step in (ex: conversation.area).
        /// </summary>
        /// <value>
        /// Caller's memory path to store the result of this step in (ex: conversation.area).
        /// </value>
        [JsonProperty("resultProperty")]
        public StringExpression ResultProperty { get; set; }

        static int port = 11000;
        static IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);
        public override Task<DialogTurnResult> BeginDialogAsync(DialogContext dc, object options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var message = Message.GetValue(dc.State);
            byte[] bytes = new byte[1024];
            string result = null;
            try
            {
                // Connect to a Remote server
                // Get Host IP Address that is used to establish a connection
                // In this case, we get one IP address of localhost that is IP : 127.0.0.1
                // If a host has multiple addresses, you will get a list of addresses

                // Create a TCP/IP  socket.
                Socket sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.
                try
                {
                    // Connect to Remote EndPoint
                    sender.Connect(remoteEP);

                    Console.WriteLine("Socket connected to {0}",
                        sender.RemoteEndPoint.ToString());

                    // Encode the data string into a byte array.
                    byte[] msg = Encoding.ASCII.GetBytes(message.ToString());

                    // Send the data through the socket.
                    int bytesSent = sender.Send(msg);

                    // Receive the response from the remote device.
                    int bytesRec = sender.Receive(bytes);
                    //Console.WriteLine("Echoed test = {0}",
                    //    Encoding.ASCII.GetString(bytes, 0, bytesRec));
                    result = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    dc.State.SetValue(this.ResultProperty.GetValue(dc.State), result);
                    // Release the socket.
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //var result = Convert.ToInt32(arg1) * Convert.ToInt32(arg2);
            //if (this.ResultProperty != null)
            //{
            //    dc.State.SetValue(this.ResultProperty.GetValue(dc.State), result);
            //}

            return dc.EndDialogAsync(result: result, cancellationToken: cancellationToken);
        }
    }
}