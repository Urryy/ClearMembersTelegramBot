# ClearMembersTelegramBot
The project was created to clean up telegram channels and groups - written in C#. 

In order to get started, you need to tell the project itself and specify all the important fields in the TelegramBotService class, 
such as - phone number, AppID, AppHash.

	Phone number - The phone number to which the confirmation code will be sent
 
	AppId - It is taken from Telegram when creating your application and Telegram itself will provide you with your code.
 
	AppHash - It is taken from Telegram when creating your application and Telegram itself will provide you with your code.

	[TelegramBotService -> GetAllUsers].
	[In this fragment, all these fields are set]
![image](https://github.com/Urryy/ClearMembersTelegramBot/assets/94054268/57d0a6c2-95c9-45c9-8166-4aaed80e36d9)

When you first start the application, you will receive a confirmation code to your phone number to register your bot.


And after all the operations done, you will only have to add the bot to the group - where you need to clean up unnecessary users and bots.
And after that, write the /Clear command to the group and it will remain to wait for feedback from the bot after cleaning
