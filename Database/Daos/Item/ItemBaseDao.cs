namespace WibboEmulator.Database.Daos.Item;

using System.Data;
using Dapper;

internal sealed class ItemBaseDao
{
    internal static List<ItemBaseEntity> GetAll(IDbConnection dbClient) => dbClient.Query<ItemBaseEntity>(
        @"SELECT id, sprite_id, item_name, type, width, length, stack_height, can_stack, is_walkable, can_sit, allow_recycle, allow_trade, allow_gift, allow_inventory_stack, interaction_type, interaction_modes_count, vending_ids, height_adjustable, effect_id, is_rare, rarity_level, item_stat.amount
        FROM `item_base` 
        LEFT JOIN item_stat ON item_base.id = item_stat.base_id"
    ).ToList();
}

public class ItemBaseEntity
{
    public int Id { get; set; }
    public string ItemName { get; set; }
    public ItemType Type { get; set; }
    public int Width { get; set; }
    public int Length { get; set; }
    public double StackHeight { get; set; }
    public bool CanStack { get; set; }
    public bool CanSit { get; set; }
    public bool IsWalkable { get; set; }
    public int SpriteId { get; set; }
    public bool AllowRecycle { get; set; }
    public bool AllowTrade { get; set; }
    public bool AllowMarketplaceSell { get; set; }
    public bool AllowGift { get; set; }
    public bool AllowInventoryStack { get; set; }
    public string InteractionType { get; set; }
    public int InteractionModesCount { get; set; }
    public string VendingIds { get; set; }
    public string HeightAdjustable { get; set; }
    public int EffectId { get; set; }
    public bool IsRare { get; set; }
    public int RarityLevel { get; set; }
    public int? Amount { get; set; }
}

public enum ItemType
{
    S,
    I,
    R,
    B,
    P,
    C
}