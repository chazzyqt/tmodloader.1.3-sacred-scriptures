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

namespace SacredScriptures.Content.Items
{
    public class JarofDirt : ModItem
	{ 
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Runner's Jar of Dirt");
			Tooltip.SetDefault(
                "Summon a pirate ship for which you can ride.\n" + 
                "Are you the infamous lost pirate?");
		}

		public override void SetDefaults()
        {
            item.width = 20;
            item.height = 30;
            item.useTime = 20;
            item.useAnimation = 20;
            item.scale = 0.7f;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.value = Item.buyPrice(0, 2, 50, 0);
            item.rare = 5;
			item.UseSound = SoundID.Item90;
            item.noMelee = true;
            item.mountType = mod.MountType("BlackShip");
        }  
    }
}