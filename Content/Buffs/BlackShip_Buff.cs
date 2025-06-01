using Terraria.ModLoader;
using Terraria;
 
namespace  SacredScriptures.Content.Buffs
{
    public class BlackShip_Buff : ModBuff
    {
        public override void SetDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
			DisplayName.SetDefault("To North!");
			Description.SetDefault("Is this the north? No, its the other way around.");
        }
 
        public override void Update(Player player, ref int buffIndex)
        {
            player.mount.SetMount(mod.MountType("BlackShip"), player);
            player.buffTime[buffIndex] = 10;
        }
    }
}