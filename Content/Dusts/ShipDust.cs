﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace SacredScriptures.Content.Dusts
{
	public class ShipDust : ModDust
	{
		public override void OnSpawn(Dust dust) {
			dust.color = new Color(255, 255, 255);
			dust.alpha = 1;
			dust.scale = 1.1f;
			dust.velocity *= 0.2f;
			dust.noGravity = true;
			dust.noLight = true;
		}

		public override bool Update(Dust dust) {
			dust.rotation += dust.velocity.X / 6f;
			dust.position += dust.velocity;
			Lighting.AddLight(dust.position, (1), (1), (1));
			int oldAlpha = dust.alpha;
			dust.alpha = (int)(dust.alpha * 1.2);
			if (dust.alpha == oldAlpha) {
				dust.alpha++;
			}
			if (dust.alpha >= 255) {
				dust.alpha = 255;
				dust.active = false;
			}
			return false;
		}
	}
}