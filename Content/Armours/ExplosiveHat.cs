using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Armours
{
	[AutoloadEquip(EquipType.Head)]
	public class ExplosiveHat : ModItem
	{
		public static short customGlowMask = 0;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magician's Explosive Hat");
			Tooltip.SetDefault(
				"Single use only.\n" +
				"Pretty sure this is one of the hats of either a developer or a character");
			ScripturesGlowMask.AddGlowMask(item.type, "SacredScriptures/Content/Glow/ExplosiveHatHead_Glow");
			customGlowMask = SacredScriptures.SetStaticDefaultsGlowMask(this);
		}

		public override void SetDefaults() 
		{
			item.width = 38;
			item.height = 22;
			item.value = 5000;
			item.rare = 8;
			item.vanity = true;
			item.glowMask = customGlowMask;
		}

		
		public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor)
		{
			glowMaskColor = Color.White;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Silk, 21);
            recipe.AddIngredient(ItemID.StarinaBottle, 1);
			recipe.AddTile(TileID.Loom);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}