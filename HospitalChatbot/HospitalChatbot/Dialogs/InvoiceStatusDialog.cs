using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HospitalChatbot.Forms;
using System.Web;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using HospitalChatbot.Common;
using System.Configuration;
using HospitalChatbot.Model;
using HospitalChatbot.Repository;
using HospitalChatbot.Common;
using AdaptiveCards;
using Fact = AdaptiveCards.Fact;
// Fact = Microsoft.Bot.Connector.Fact;

namespace HospitalChatbot.Dialogs
{
    [Serializable]
    public class InvoiceStatusDialog : IDialog<object>
    {
        private string _identity = string.Empty;
        private int _invoiceChkCounter = 0;
        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync("Do you want to know the status of Invoice(Yes/No)?");
            //context.Wait(this.MessageReceivedInvoiceNumberCheck);
            string emailId = string.Empty;
            string mobileNo = string.Empty;
            context.PrivateConversationData.TryGetValue<string>("EmailId", out emailId);
            context.PrivateConversationData.TryGetValue<string>("MobileNumber", out mobileNo);

            _identity = !string.IsNullOrEmpty(emailId) ? emailId : mobileNo;

            await InvoiceStatusStart(context);

        }

        private async Task InvoiceStatusStart(IDialogContext context)
        {
            await context.PostAsync("Do you know the invoice number(Yes/No)?");
            context.Wait(this.MessageReceivedInvoiceStatusCheck);
        }

        private async Task MessageReceivedInvoiceStatusCheck(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if ((message.Text.ToLower().Contains("yes")) || message.Text.ToLower().Contains("y"))
            {
                await context.PostAsync("Please enter the invoice number");
                context.Wait(this.MessageReceivedInvoiceStatusCheckProcess);
            }
            else if ((message.Text.ToLower().Contains("no")) || message.Text.ToLower().Contains("n"))
            {
                ShowInvoiceOptions(context);
            }
            else
            {
                context.Done<object>(result);
            }
        }

        private void ShowInvoiceOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnInvoiceSelected, GetInvoices(), "Below are your invoice numbers. Please select", "Not a valid option", 3);
        }

        private async Task OnInvoiceSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var option = await result;
                string invoiceSelected = option.ToString();
                Invoice invoice = GetSingleInvoice(Convert.ToDouble(invoiceSelected));
                // respond back to client with invoice details               
                await DisplayReceiptCard(context, invoice);
                await context.PostAsync($"\n\n Do you wish to obtain status of any other invoice?(Yes/No)");
                context.Wait(this.MessageReceivedInvoiceStatusRestart);
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                //context.Wait(this.MessageReceivedAsync);
                context.Done<object>(result);
            }
        }

        private async Task MessageReceivedInvoiceStatusCheckProcess(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            string invoiceNo = message.Text;
            Invoice invoice = GetSingleInvoice(Convert.ToDouble(invoiceNo));
            if (invoice == null)
            {
                if (_invoiceChkCounter < 2)
                {
                    _invoiceChkCounter++;
                    await context.PostAsync("Invalid Invoice Number. Please enter again");
                    context.Wait(this.MessageReceivedInvoiceStatusCheckProcess);
                }
                else
                {
                    ShowInvoiceOptions(context);
                }
            }
            else
            {
                // respond back to client with invoice details
                await DisplayReceiptCard(context, invoice);
                await context.PostAsync($"\n\n Do you wish to obtain status of any other invoice?(Yes/No)");
                context.Wait(this.MessageReceivedInvoiceStatusRestart);
                //context.Done<object>(result);
            }



        }

        private async Task MessageReceivedInvoiceStatusRestart(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            if ((message.Text.ToLower().Contains("yes")) || message.Text.ToLower().Contains("y"))
            {
                await InvoiceStatusStart(context);
            }
            else if ((message.Text.ToLower().Contains("no")) || message.Text.ToLower().Contains("n"))
            {
                await context.PostAsync("Thank you for your interaction. Happy to help.");
                //context.Done<object>(result);
            }
        }

        public async Task DisplayReceiptCard(IDialogContext context, Invoice invoice)
        {
            
            var replyMessage = context.MakeMessage();
            Attachment attachment = getBotAdaptiveCard(invoice);
            replyMessage.Attachments = new List<Attachment> { attachment };
            await context.PostAsync(replyMessage);
        }

        private List<string> GetInvoices()
        {
            InvoiceRepository invoiceRepo = new InvoiceRepository();
            List<Invoice> invoiceList = invoiceRepo.GetInvoices(_identity);

            List<string> invoiceNos = new List<string>();
            foreach (var invoice in invoiceList)
            {
                invoiceNos.Add(Convert.ToString(invoice.MIRONumber));
            }
            return invoiceNos;

        }

        private Invoice GetSingleInvoice(double? invoiceNumber)
        {
            InvoiceRepository invoiceRepo = new InvoiceRepository();
            Invoice invoice = invoiceRepo.GetSingleInvoice(invoiceNumber, _identity);

            return invoice;

        }

        //private static Attachment getBotReceiptCard(Invoice invoice)
        //{
        //    List<Fact> Facts;
        //    if (invoice.Payment_Status == "Paid")
        //    {
        //        Facts = new List<Fact>
        //        {
        //            new Fact("Name:",invoice.VendorName),
        //            new Fact("Invoice Number:",Convert.ToString(invoice.MIRONumber)),
        //            new Fact("Status:", Convert.ToString(invoice.Payment_Status)),
        //            new Fact("------------------------","-----------"),
        //            new Fact("PO Number:",Convert.ToString(invoice.PONumber)),
        //            new Fact("PO Date:",DateTimeParser.ConvertDateTimeToDate(invoice.PODate)),
        //            new Fact("------------------------","-----------"),
        //            new Fact("Payment Date:", (DateTimeParser.ConvertDateTimeToDate(invoice.PaymentDate) == "1/1/0001")? "NA" : DateTimeParser.ConvertDateTimeToDate(invoice.PaymentDate)),
        //            new Fact("Payment Amount:", string.IsNullOrEmpty(Convert.ToString(invoice.PaymentAmt))? "0": Convert.ToString(invoice.PaymentAmt)),
        //            new Fact("Pay Cheque No:", string.IsNullOrEmpty(Convert.ToString(invoice.PayCheckNo))? "0": Convert.ToString(invoice.PayCheckNo)),
        //            new Fact("Pay Cheque Date:", (DateTimeParser.ConvertDateTimeToDate(invoice.PayCheckDt) == "1/1/0001")? "NA" : DateTimeParser.ConvertDateTimeToDate(invoice.PayCheckDt)),
        //            new Fact("Value:", Convert.ToString(invoice.Value)),
        //            new Fact("------------------------","-----------")
        //        };
        //    }
        //    else
        //    {
        //        Facts = new List<Fact>
        //        {
        //            new Fact("Name:",invoice.VendorName),
        //            new Fact("Invoice Number:",Convert.ToString(invoice.MIRONumber)),
        //            new Fact("Status:", Convert.ToString(invoice.Payment_Status)),
        //            new Fact("------------------------","-----------"),
        //            new Fact("PO Number:",Convert.ToString(invoice.PONumber)),
        //            new Fact("PO Date:",DateTimeParser.ConvertDateTimeToDate(invoice.PODate)),
        //            new Fact("------------------------","-----------"),
        //            new Fact("Value:", Convert.ToString(invoice.Value)),
        //            new Fact("------------------------","-----------")
        //        };
        //    }


        //    var receiptCard = new ReceiptCard
        //    {
        //        Title = "Invoice Details",
        //        Facts = Facts,

        //        //Items = new List<ReceiptItem>
        //        //{

        //        //    new ReceiptItem("Hit Refresh",subtitle:"by Satya Nadella (Author)",text:"Kindle Edition", price: "278.31", quantity: "1", image: new CardImage(url: "https://images-eu.ssl-images-amazon.com/images/I/41eAfVuUzeL.jpg"),tap: new CardAction("Read More")),
        //        //    new ReceiptItem("XamarinQA",subtitle:"by Suthahar J (Author)",text:"Kindle Edition", price: "116.82", quantity: "1", image: new CardImage(url: "https://images-eu.ssl-images-amazon.com/images/I/51z6GXy3FSL.jpg"),tap: new CardAction("Read More")),
        //        //    new ReceiptItem("Surface Pro 4",subtitle:"Core i5 - 6th Gen/4GB/128GB/Windows 10 Pro/Integrated Graphics/31.242 Centimeter Full HD Display", price: "66,500", quantity: "1", image: new CardImage(url: "https://images-na.ssl-images-amazon.com/images/I/41egJVu%2Bc0L.jpg"),tap: new CardAction("Read More")),
        //        //    new ReceiptItem("Windows 10 pro",subtitle:"by Microsoft", price: "13,846", quantity: "1", image: new CardImage(url: "https://images-na.ssl-images-amazon.com/images/I/7176wliQYsL._SL1500_.jpg"),tap: new CardAction("Read More"))
        //        //},
        //        //Tax = "2000",
        //        //Total = "82,741.13",
        //        //Tap = new CardAction(ActionTypes.OpenUrl, value: "https://www.microsoft.com/en-in/store/b/home?SilentAuth=1&wa=wsignin1.0"),
        //        //Buttons = new List<CardAction>
        //        //{
        //        //  new CardAction(
        //        //      ActionTypes.OpenUrl,
        //        //      "Request Email",
        //        //      "https://assets.onestore.ms/cdnfiles/external/uhf/long/9a49a7e9d8e881327e81b9eb43dabc01de70a9bb/images/microsoft-gray.png",
        //        //      "mailto:jssuthahar@gmail.com?subject=Order%20Number:97421&body=Hi%team,%20I%need%20Invoice")
        //        //}
        //    };

        //    return receiptCard.ToAttachment();
        //}

        private static Attachment getBotAdaptiveCard(Invoice invoice)
        {
            List<Fact> factList;
            if (invoice.Payment_Status == "Paid")
            {
                factList = new List<Fact>
                {
                    new Fact("Name:",invoice.VendorName),
                    new Fact("Invoice Number:",Convert.ToString(invoice.MIRONumber)),
                    new Fact("Status:", Convert.ToString(invoice.Payment_Status)),
                    new Fact("------------------------","-----------"),
                    new Fact("PO Number:",Convert.ToString(invoice.PONumber)),
                    new Fact("PO Date:",DateTimeParser.ConvertDateTimeToDate(invoice.PODate)),
                    new Fact("------------------------","-----------"),
                    new Fact("Payment Date:", (DateTimeParser.ConvertDateTimeToDate(invoice.PaymentDate) == "1/1/0001")? "NA" : DateTimeParser.ConvertDateTimeToDate(invoice.PaymentDate)),
                    new Fact("Payment Amount:", string.IsNullOrEmpty(Convert.ToString(invoice.PaymentAmt))? "0": Convert.ToString(invoice.PaymentAmt)),
                    new Fact("Pay Cheque No:", string.IsNullOrEmpty(Convert.ToString(invoice.PayCheckNo))? "0": Convert.ToString(invoice.PayCheckNo)),
                    new Fact("Pay Cheque Date:", (DateTimeParser.ConvertDateTimeToDate(invoice.PayCheckDt) == "1/1/0001")? "NA" : DateTimeParser.ConvertDateTimeToDate(invoice.PayCheckDt)),
                    new Fact("Value:", Convert.ToString(invoice.Value)),
                    new Fact("------------------------","-----------")
                };
            }
            else
            {
                factList = new List<Fact>
                {
                    new Fact("Name:",invoice.VendorName),
                    new Fact("Invoice Number:",Convert.ToString(invoice.MIRONumber)),
                    new Fact("Status:", Convert.ToString(invoice.Payment_Status)),
                    new Fact("------------------------","-----------"),
                    new Fact("PO Number:",Convert.ToString(invoice.PONumber)),
                    new Fact("PO Date:",DateTimeParser.ConvertDateTimeToDate(invoice.PODate)),
                    new Fact("------------------------","-----------"),
                    new Fact("Value:", Convert.ToString(invoice.Value)),
                    new Fact("------------------------","-----------")
                };
            }

            AdaptiveCard adaptiveCard = new AdaptiveCard()
            {
                // Defining the Body contents of the card 
                Body = new List<CardElement>()
    {
        new Container()
        {
            Items = new List<CardElement>()
            {
                new TextBlock()
                {
                    Text = invoice.VendorName,
                    Weight = TextWeight.Bolder,
                    Size = TextSize.Medium
                },
                new ColumnSet()
                {
                    Columns = new List<Column>()
                    {
                        //new Column()
                        //{
                        //   Items = new List<CardElement>()
                        //   {
                        //       new Image()
                        //       {
                        //           Url = "127.0.0.1",
                        //           Size = ImageSize.Large
                        //       }
                        //   }
                        //},
                        new Column()
                        {
                            Items = new List<CardElement>()
                            {
                                new FactSet()
                                {
                                    Facts = factList
                                }
                            }
                        }
                    }
                }
            }
        }
    },
                // Defining the actions (buttons) our card will have, as well as their functions 
                //Actions = new List<ActionBase>()
                //{
                //    new SubmitAction()
                //    {
                //        Title = "Order",
                //        // returning our StringData as JSON to the bot 
                //        DataJson = StringData
                //    }
                //}
            };


            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = adaptiveCard
            };

            return attachment;

        }

    }
}