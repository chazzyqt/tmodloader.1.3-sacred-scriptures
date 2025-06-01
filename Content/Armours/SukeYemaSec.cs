using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace SacredScriptures.Content.Armours
{
	[AutoloadEquip(EquipType.Head)]
	public class SukeYemaSec : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Yemahutz' Second Boar Mask");
			Tooltip.SetDefault("Yemahutz' Second abomination.");
		}

		public override void SetDefaults() 
		{
			item.width = 36;
			item.height = 36;
			item.value = 192;
			item.rare = ItemRarityID.Blue;
			item.vanity = true;
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Silk, 12);
			recipe.AddTile(TileID.Loom);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}