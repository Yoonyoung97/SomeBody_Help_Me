using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace chatbotforas.Dialog
{
    public class UserProfileDialog : ComponentDialog
    {
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly IRecognizer _Recognizer;
        public UserProfileDialog(UserState userState, IRecognizer recognizer)
            : base(nameof(UserProfileDialog))
        {
            _userProfileAccessor = userState.CreateProperty<UserProfile>("UserProfile");
            _Recognizer = recognizer;
            // This array defines how the Waterfall will execute.
            var waterfallSteps = new WaterfallStep[]
            {
                NameStepAsync,
                NameConfirmStepAsync,
                PhoneStepAsync,
                ConfirmStepAsync,
                ErrorInputStepAsync,
                ErrorStepAsync,
                SuccessStepAsync,
                SolutionStepAsync,
                EmailStepAsync,
                CommentStepAsync
            };
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallSteps));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            InitialDialogId = nameof(WaterfallDialog);

        }
        //@ 이름 프롬프트
        private static async Task<DialogTurnResult> NameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("기사님, 성함이 어떻게 되시나요?") }, cancellationToken);
        }
        private async Task<DialogTurnResult> NameConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Name"] = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text($"{stepContext.Result } 맞으신가요!?") }, cancellationToken);
        }

        private async Task<DialogTurnResult> PhoneStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("휴대폰 번호는 어떻게 되시는가요?") }, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("다시 시작하여 주십시요."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }
        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Phone"] = (string)stepContext.Result;
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text($"{stepContext.Result } 맞으신가요!?") }, cancellationToken);
        }


        private async Task<DialogTurnResult> ErrorInputStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool)stepContext.Result)
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("어떤 증상 인가요 ?") }, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("다시 시작하여 주십시요."), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken:cancellationToken);
            }


        }
        //@ 에러 증상 및 솔루션 프롬프트 
        private async Task<DialogTurnResult> ErrorStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var recognizerResult = await _Recognizer.recognizer.RecognizeAsync(stepContext.Context, cancellationToken);
            var topIntent = recognizerResult.GetTopScoringIntent();
            stepContext.Values["LuisResult"] = topIntent.intent;
            stepContext.Values["Error"] = (string)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"{stepContext.Result}"), cancellationToken);
            Soultion(stepContext, cancellationToken);
            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text("고장해결 하셨습니까?") }, cancellationToken);

        }
        //@ 해결 유무 프롬프트
        private async Task<DialogTurnResult> SuccessStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!(bool)stepContext.Result){
                return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
                {
                    Prompt = MessageFactory.Text("어떤 문제인가요? \n 1.메뉴얼대로 했지만 해결불가 \n2.교체부품부족"),
                    Choices = ChoiceFactory.ToChoices(new List<string> { "1", "2" }),
                }, cancellationToken);
            }
            else
            {
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("오늘 하루도 고생하셨습니다!"), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
        }

        // 해결 못할 경우 추가적인 작업
        private async Task<DialogTurnResult> SolutionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if(((FoundChoice)stepContext.Result).Value == "1")
            {
                stepContext.Values["Problem"] = "메뉴얼대로 했지만 해결불가";
            }
            else
            {
                stepContext.Values["Problem"] = "교체부품부족";
            }
            
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("담당자에게 메일을 보내기 전, \n 기사님 정보를 확인 하겠습니다."), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"이름 : {stepContext.Values["Name"]} " +
                $"\n 연락처:{stepContext.Values["Phone"]} " +
                $"\n 문의 내용:{stepContext.Values["Error"]}\n"), cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"겪으신 고장 현상은 \"{stepContext.Values["Error"]}\"이며, 문의 메일의 목적은 \"{stepContext.Values["Problem"]}\" 입니다. "));
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Prompt = MessageFactory.Text("덧붙여 설명할 이야기가 있나요!?"),
                Choices = ChoiceFactory.ToChoices(new List<string> { "네", " 아니요" })
            }, cancellationToken);

        }
        // 메일 송신 프롬프트

        private async Task<DialogTurnResult> EmailStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (((FoundChoice)stepContext.Result).Value == "네")
            {
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = MessageFactory.Text("코멘트를 달아주세요!") }, cancellationToken);

            }
            else
            {
                var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

                userProfile.Name = (string)stepContext.Values["Name"];
                userProfile.Phone = (string)stepContext.Values["Phone"];
                userProfile.Error = (string)stepContext.Values["Error"];
                userProfile.Problem = (string)stepContext.Values["Problem"];
                userProfile.LuisResult = (string)stepContext.Values["LuisResult"];
                userProfile.Comment = "X";
                //SendEmail(userProfile);
                await stepContext.Context.SendActivityAsync(MessageFactory.Text("위의 내용을 토대로  메일을 보내겠습니다. \n 향후 개인 연락처로 연락 드리겠습니다!"), cancellationToken);
                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

            }
        }
        private async Task<DialogTurnResult> CommentStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["Comment"] = (string)stepContext.Result;
            var userProfile = await _userProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            userProfile.Name = (string)stepContext.Values["Name"];
            userProfile.Phone = (string)stepContext.Values["Phone"];
            userProfile.Error = (string)stepContext.Values["Error"];
            userProfile.Problem = (string)stepContext.Values["Problem"];
            userProfile.LuisResult = (string)stepContext.Values["LuisResult"];
            userProfile.Comment = (string)stepContext.Values["Comment"];
            await stepContext.Context.SendActivityAsync(MessageFactory.Text("위의 내용을 토대로  메일을 보내겠습니다. \n 향후 개인 연락처로 연락 드리겠습니다!"), cancellationToken);
            //SendEmail(userProfile);
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);


        }
        protected async void Soultion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            switch ((string)stepContext.Values["LuisResult"])
            {
                case "01":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 실내기 공기 온도 센서가 불량인 것 같습니다. 실내기에서 다음과 같은 항목을 확인해주십시요.\n 1. 실내기를 내부를 확인해주시길 바랍니다.\n 2. 실내기 공기 온도 센서가 단선 또는 합선 되었는지 확인 부탁드립니다."), cancellationToken);
                    break;
                case "02":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 실내기 배관입구 온도 센서가 불량인 것 같습니다. 실내기에서 다음과 같은 항목을 확인해주십시요.\n 1. 실내기를 내부를 확인해주시길 바랍니다. \n2. 실내기 배관입구 온도 센서가 단선 또는 합선 되었는지 확인 부탁드립니다."), cancellationToken);
                    break;
                case "03":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 리모컨과 실내기 간의 통신 불량인 것 같습니다. 실내기에서 다음과 같은 항목을 확인해주십시요. \n 1. 실외기 차단기를 찾아 에어컨 차단기 스위치를 내려 전원을 차단 후 3분 정도 기다립니다.\n 2. 차단기 스위치를 다시 올려 에어컨을 리셋 시켜주면 됩니다."), cancellationToken);
                    break;
                case "04":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 실내기 Drain 불량인 것 같습니다. 실내기에서 다음과 같은 항목을 확인해주십시요.\n1. 배수 드레인 펌프 및 플로트 스위치를 확인하여 주시길 바랍니다.\n 2. 불량 제품을 교체해주시길 바랍니다."), cancellationToken);
                    break;
                case "05":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 실외기와 실내기간의 통신이 불량인 것 같습니다. 실내기에서 다음과 같은 항목을 확인해주십시요.\n 1. 실내기 박스안에 메인기판과 실외기로 나가는 전선의 소켓 부분을 확인해 주시길 바랍니다.\n 2. 소켓이 존재한다면 소켓을 제거하고 메인기판과 전선을 납땜하여 직결해주십시요."), cancellationToken);
                    break;
                case "06":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 배관 출구 온도 센서가 고장난 것으로 추측됩니다. 다음과 같은 항목을 확인해주십시요.\n 1.  센서 교체를 위해 실내기를 분해해 주십시요.\n 2. 검은색 입구센서와 빨간섹 출구센서를 분리해 교체하여 연결해주시길 바랍니다."), cancellationToken);
                    break;
                case "notcold":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 냉매부족 혹은 냉매 누출입니다. 다음과 같은 항목을 확인해주십시요 .\n 1. 부족한 냉매를 채워 주십시요. 후에 압력계를 꽂고 3일 뒤 재방문하여 압력계의 변화를 확인하십시요. \n 2. 변화가 있다면 유니온과 배관을 교체하여 주십시요."), cancellationToken);
                    break;
                case "07":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 이질 운전 입니다. 다음과 같은 항목을 확인해주십시요.\n 1. 에어컨과 리모컨의 운전 모드를 통일시켜주십시요.\n2. 1번 방법이 안된다면 실내기, 실외기 전원차단기를 내렸다 1분 정도 기다린후 다시 올려주십시요."), cancellationToken);
                    break;
                case "09":
                    await stepContext.Context.SendActivityAsync(MessageFactory.Text("저희가 추측하는 고장원인은 실내기 BLDC 모터 피드백 신호 불량인 것 같습니다. 실내기에서 다음과 같은 항목을 확인해주십시요.\n1. 모터 컨넥터 탈거 또는 모터가 불량인 경우 이러한 상황이 발생합니다. 모터 컨넥터 탈거 를 확인해주십시요.\n 2. 접촉 불량 혹은 고장인 부품을 교체하여 주시길 바랍니다."), cancellationToken);
                    break;
            }
        }



        protected void SendEmail(UserProfile userProfile)
        {
            // 보내는 사람 아이디 / 이름 / 인코딩 방식
            MailAddress from = new MailAddress("82javaking@gmail.com", "고장관련문의", System.Text.Encoding.UTF8);

            // @받는사람
            MailAddress to = new MailAddress("hyy0238@naver.com");
            MailMessage mail = new MailMessage(from, to);
            try
            {
                string simpleBody = $"{userProfile.Name} 기사님이 보내신 메일입니다. {userProfile.Error} 문제로 인해 {userProfile.LuisResult}가 의심됩니다.<br>" +
                    $"기사님의 요청 사항은 {userProfile.Problem} 이시고 부가적인 코멘트로 {userProfile.Comment}를 달으셨습니다.<br>" +
                    $"연락처 : {userProfile.Phone}";
                    
                // @ 제목 
                mail.Subject = $"고장관련 메일입니다.";
                // @ 본문 내용
                mail.Body = $"<html><body>{simpleBody}</body></html>";
                //본문 내용 html  포맷
                mail.IsBodyHtml = true;
                mail.SubjectEncoding = System.Text.Encoding.UTF8;
                mail.BodyEncoding = System.Text.Encoding.UTF8;

                SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Port = 587;
                smtpServer.Credentials = new System.Net.NetworkCredential("82javaking@gmail.com", "!Aa1597532");
                smtpServer.EnableSsl = true;
                smtpServer.Send(mail);
                mail.Dispose();
            }
            finally
            {
                foreach (var attach in mail.Attachments)
                {
                    attach.ContentStream.Close();
                }
            }
        }
    }
}



