namespace WibboEmulator.Games.Users.Messenger;

public static class MessengerMessageErrorsUtility
{
    public static int GetMessageErrorPacketNum(MessengerMessageErrors error) => error switch
    {
        MessengerMessageErrors.YourMuted => 4,
        MessengerMessageErrors.FriendOffline => 5,
        MessengerMessageErrors.NotFriends => 6,
        MessengerMessageErrors.FriendBusy => 7,
        MessengerMessageErrors.OfflineFailed => 10,
        _ => 3,
    };
}
