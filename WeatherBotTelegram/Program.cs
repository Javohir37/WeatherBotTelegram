using Microsoft.VisualBasic;
using System.Net;
using System.Transactions;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using WeatherBotTelegram;
var botClient = new TelegramBotClient("");

using CancellationTokenSource cts = new();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();
WebClient client1 = new WebClient();
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    // Echo received message text
    CLassMain objMain = new CLassMain();
    if(messageText == "/start")
    {
        Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "Hey! I'm a weather bot \nsend /location to enter your location and \n/weather to get a weather info \n/help for getting help",
        cancellationToken: cancellationToken);
    }
    else if(messageText == "/location")
    {
        BotParams.isLocation = true;//to make bot know that the next message will be a locaiton
        Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Enter the name of your city",
                cancellationToken: cancellationToken);
    }
    else if(messageText == "/help"){
        Message sentMessage = await botClient.SendPhotoAsync(
                chatId: chatId,
                photo: new InputFileUrl("https://www.dropbox.com/s/thuucwwdrnoaz3n/Annotation%202023-02-12%20164555.png?dl=0"), //add some photo i guess to to indicate that there hould be
                cancellationToken: cancellationToken);
    }
    else if(messageText == "/weather")
    {
        if (Coordinates.city == "null")
        {
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "You didn't set your location, please use /location",
                cancellationToken: cancellationToken);
        }
        else
        {
            objMain.weather();
            Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: Weather.readyToUse,
            cancellationToken: cancellationToken);
        }
    }
    else if(BotParams.isLocation == true)
    {
        objMain.coordinates(messageText);
        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "got your location ✔\nnow you can use /weather",
            cancellationToken: cancellationToken);
        BotParams.isLocation = false; //to make bot know that the next message won't be a locaiton
    }
    else
    {
        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "Sorry, I can't understand you",
            cancellationToken: cancellationToken);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}
