namespace WibboEmulator.Games.Users.Messenger;

public static class MessengerMessageErrorsUtility
{
    public static int GetMessageErrorPacketNum(MessengerMessageErrors error)
    {
        switch (error)
        {
            default:
            case MessengerMessageErrors.FriendMuted:
                return 3;

            case MessengerMessageErrors.YourMuted:
                return 4;

            case MessengerMessageErrors.FriendOffline:
                return 5;

            case MessengerMessageErrors.NotFriends:
                return 6;

            case MessengerMessageErrors.FriendBusy:
                return 7;

            case MessengerMessageErrors.OfflineFailed:
                return 10;
        }
    }
}
