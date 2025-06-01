using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Items
{
	public class DiamondSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Diamond Sword");
			Tooltip.SetDefault("It seems that this item forgot which game it was supposed to be used at.");
		}

		public override void SetDefaults() 
		{
			item.damage = 19;
			item.ranged = false;
			item.melee = true;
			item.magic = false;
			item.width = 40;
			item.height = 40;
			item.useTime = 20;
			item.useAnimation = 20;
			item.knockBack = 6;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 2;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.crit = 6;
			item.useStyle = 1;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Diamond, 2);
			recipe.AddIngredient(ItemID.Wood, 1);
			recipe.AddTile(TileID.WorkBenches);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}