// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.IO;
using chatbotforas.Dialog;

namespace chatbotforas
{
    public class asBot<T> : ActivityHandler where T : Microsoft.Bot.Builder.Dialogs.Dialog
    {
        protected readonly Microsoft.Bot.Builder.Dialogs.Dialog Dialog;
        protected readonly BotState ConversationState;
        protected readonly BotState UserState;
        protected readonly ILogger Logger;

        public asBot(ConversationState conversationState, UserState userState, T dialog, ILogger<asBot<T>> logger)
        {
            ConversationState = conversationState;
            UserState = userState;
            Dialog = dialog;
            Logger = logger;
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Running dialog with Message Activity.");

            // Run the Dialog with the new message Activity.
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }
    }
}















/*        private readonly IRecognizer _Recognizer;
        private readonly ILogger<asBot> _logger;


        public asBot(IRecognizer Recognizer, ILogger<asBot> logger)
        {
            _logger = logger;
            _Recognizer = Recognizer;
        }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello world!"), cancellationToken);
                }
            }
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var recognizerResult = await _Recognizer.recognizer.RecognizeAsync(turnContext, cancellationToken);
            var topIntent = recognizerResult.GetTopScoringIntent();
            await TopIntentAsync(turnContext, topIntent.intent, recognizerResult, cancellationToken);

        }
        protected async Task TopIntentAsync(ITurnContext<IMessageActivity> turnContext,string Intent,
            RecognizerResult recognizerResult, CancellationToken cancellationToken){
            var str = "";
            switch (Intent)
            {
                case "01":
                    str = " 해결책 "; 
                    await turnContext.SendActivityAsync(MessageFactory.Text("${ topIntent} "), cancellationToken);
                    break;
                case "02":
                    str = "해결책";
                    await turnContext.SendActivityAsync(MessageFactory.Text("${ topIntent} "), cancellationToken);
                    break;
            }
        }
        protected void EmailAsync(string Intent, string ID, string password, string simpleBody)
        {
            MailMessage mail = new MailMessage();
            try
            {
                // @ 보내는 사람
                mail.From = new MailAddress("82javaking@gmail.com", "AS관련", System.Text.Encoding.UTF8);
                // @ 받는 사람
                mail.To.Add(password);
                // @ 제목 
                mail.Subject = $"{Intent} 관련 내용입니다.";
                // @ 본문 내용
                mail.Body = $"<html><body> {simpleBody} </body></html>";
                //본문 내용 html  포맷
                mail.IsBodyHtml = true;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential(ID, password);
                smtpServer.EnableSsl = true;
                smtpServer.Send(mail);
            }
            finally
            {
                foreach( var attach in mail.Attachments)
                {
                    attach.ContentStream.Close();
                }
            }
        }
    }*/


