namespace WibboEmulator.Communication.Packets.Outgoing.Users;

using WibboEmulator.Games.Users.Messenger;

internal class GetRelationshipsComposer : ServerPacket
{
    public GetRelationshipsComposer(int userId, List<Relationship> relationships)
        : base(ServerPacketHeader.MESSENGER_RELATIONSHIPS)
    {
        this.WriteInteger(userId);
        this.WriteInteger(relationships.Count);
        ICollection<Relationship> relations = relationships;

        var relationRandom = new Dictionary<int, Relationship>();

        foreach (var userRelation in relations)
        {
            relationRandom.Add(userRelation.UserId, userRelation);
        }

        var rand = new Random();
        relationRandom = relationRandom.OrderBy(x => rand.Next()).ToDictionary(item => item.Key, item => item.Value);

        var loves = relationRandom.Count(x => x.Value.Type == 1);
        var likes = relationRandom.Count(x => x.Value.Type == 2);
        var hates = relationRandom.Count(x => x.Value.Type == 3);
        foreach (var rel in relationRandom.Values)
        {
            var hHab = WibboEnvironment.GetUserById(rel.UserId);
            if (hHab == null)
            {
                this.WriteInteger(0);
                this.WriteInteger(0);
                this.WriteInteger(0); // Their ID
                this.WriteString("Placeholder");
                this.WriteString("hr-115-42.hd-190-1.ch-215-62.lg-285-91.sh-290-62");
            }
            else
            {
                this.WriteInteger(rel.Type);
                this.WriteInteger(rel.Type == 1 ? loves : rel.Type == 2 ? likes : hates);
                this.WriteInteger(rel.UserId); // Their ID
                this.WriteString(hHab.Username);
                this.WriteString(hHab.Look);
            }
        }
    }
}
