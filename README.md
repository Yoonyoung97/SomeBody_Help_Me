누가 좀 도와줘봇 ChatBot For Repair Engineer
==================

<p><a href="https://youtu.be/iUrwiaHB7p4" target="_blank"><img src="https://raw.githubusercontent.com/MadupPinket/ket-bot/master/docs/images/FINKET%20DEmo.jpg" alt="FINKET Demo" style="max-width:100%;"></a></p>

[Microsoft Bot Framework](https://dev.botframework.com/)는 Bot을 만들기 위해 마이크로소프트가 운영하는 서비스와 SDK입니다. Bot Connector라는 서비스를 중심으로 Skype, Facebook Messenger등의 채널을 우리가 만들 Bot에 연결시켜 줍니다. Node.js 와 C# 을 지원하는 Bot Builder SDK를 사용하면 빠르게 Bot을 만들 수 있습니다. 본 코드에선 C#을 사용하였습니다.

[Microsoft Languages Understanding(Luis)](https://www.luis.ai/) 는 사용자 지정 기계 학습 인텔리전스를 사용자의 자연스러운 기존 언어 텍스트에 적용하여 전체적인 의미를 예측하고 관련된 자세한 정보를 추출하는 클라우드 기반 API 서비스입니다. 클라이언트 애플리케이션은 자연어로 사용자와 통신하여 작업을 완료하는 대화형 애플리케이션입니다. 클라이언트 애플리케이션의 예로는 소셜 미디어 앱, 챗봇 및 음성 지원 데스크톱 애플리케이션을 들 수 있습니다.  Node.js와 C#을 지원하는 Bot builder SDK를 사용하면 빠르게 Bot을 만들 수 있습니다. 본 코드에선 C#을 사용하였습니다.



## 개발 환경

### Bot
- Windows 10 / Visual Studio 2019
- C# 
- Entity Framework
- [Bot Framework Template](http://aka.ms/bf-bc-vstemplate) 
- [Bot Framework Emulator](https://aka.ms/bf-bc-emulator)

### WEB

* PHP
* HTML
* Heroku

## 유용한 문서

- [Getting started in .NET](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html)
- [Microsoft Luis Documentation](https://docs.microsoft.com/ko-kr/azure/cognitive-services/luis/)
- [Bot Framework Documentation](https://docs.botframework.com)     
- [Bot Basics](https://docs.microsoft.com/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)
- [Dialogs](https://docs.microsoft.com/azure/bot-service/bot-builder-concept-dialog?view=azure-bot-service-4.0)
- [Gathering Input Using Prompts](https://docs.microsoft.com/azure/bot-service/bot-builder-prompts?view=azure-bot-service-4.0&tabs=csharp)

- [Activity processing](https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-concept-activity-processing?view=azure-bot-service-4.0)
- [Azure Bot Service Introduction](https://docs.microsoft.com/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
- [Azure Bot Service Documentation](https://docs.microsoft.com/azure/bot-service/?view=azure-bot-service-4.0)
- [Azure CLI](https://docs.microsoft.com/cli/azure/?view=azure-cli-latest)
- [Azure Portal](https://portal.azure.com)
- [Language Understanding using LUIS](https://docs.microsoft.com/azure/cognitive-services/luis/)
- [Channels and Bot Connector Service](https://docs.microsoft.com/azure/bot-service/bot-concepts?view=azure-bot-service-4.0)


## 아키텍쳐

- 사용자는 웹을 통해서 메시지를 입력하여 Bot에 전달하거나 Bot이 전달 해준 메시지를 받습니다. 
- 사용자가 입력한 메시지는 Web Chat API를 통해 Bot에 전달 됩니다. 

## 배포환경  

 1. Azure Web App Bot : Bot 제작
 1. Azure Luis : 자연어 처리 
 1. github : 형태 관리 및 서버 파일 저장 
 1. Heroku : 호스팅

## Configuration 

appSetting.json

1. Luis App Id: Luis를 등록하면서 얻을 수 잇습니다.
1. Luis App Password: Luis를 등록하면서 얻을 수 있스비낟.
1. Microsoft App Id: http://dev.botframework.com 에서 Bot을 등록하면서 얻을 수 있습니다. 
1. Microsoft App Password: 역시 Bot을 등록하면서 얻을 수 있습니다.

## 코드 흐름 

Compoment Dialog를 상속받은 MainDialog를 이용하여 작동된다. Dialog는 WaterFall Dialog로 순차적으로 대화가 진행된다.

개인정보 질문 -> 고장 내용 질문 -> 해결방안 제시 -> 해결 못할 경우 담당자에게 이메일을 보냄

## Microsoft Bot Framework 기초 강좌 

[Bot Framework 4.0 개발 가이드 (1) 시작하면서](http://youngwook.com/221558237007)

[Bot Framework 4.0 개발 가이드 (2) Hello Bot](http://youngwook.com/221557638246) 

[Bot Framework 4.0 개발 가이드 (3) 배포하기](http://youngwook.com/221558237007)

[Bot Framework 4.0 개발 가이드 (4) ActivityHandler](http://youngwook.com/221559799475)

[Bot Framework 4.0 개발 가이드 (5) 메세지 보내기](http://youngwook.com/221560704140)

[Bot Framework 4.0 개발 가이드 (6) 이미지 보내기](http://youngwook.com/221563047499)

[Bot Framework 4.0 개발 가이드 (7) 버튼 사용하기](http://youngwook.com/221563059789)

*Enjoy with your bot!!*