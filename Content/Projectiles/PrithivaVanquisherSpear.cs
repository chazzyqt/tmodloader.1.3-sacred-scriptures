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

namespace SacredScriptures.Content.Projectiles
{
	public class PrithivaVanquisherSpear : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Prithiva Vanquisher Spear");
		}

		public override void SetDefaults()
		{
			projectile.damage = 909;
			projectile.melee = true;
			projectile.width = 30;
			projectile.height = 30;
			projectile.scale = 0.85f;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.hide = true;
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			if (projectile.ai[0] == 1f)
			{
				int npcIndex = (int)projectile.ai[1];
				if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
				{
					if (Main.npc[npcIndex].behindTiles)
					{
						drawCacheProjsBehindNPCsAndTiles.Add(index);
					}
					else
					{
						drawCacheProjsBehindNPCs.Add(index);
					}

					return;
				}
			}
			drawCacheProjsBehindProjectiles.Add(index);
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			width = height = 10;
			return true;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
			{
				targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
			}
			return projHitbox.Intersects(targetHitbox);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item14, (int)projectile.position.X, (int)projectile.position.Y);
		}

		public bool IsStickingToTarget
		{
			get => projectile.ai[0] == 1f;
			set => projectile.ai[0] = value ? 1f : 0f;
		}

		public int TargetWhoAmI
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		private const int MAX_STICKY_JAVELINS = 10;
		private readonly Point[] _stickingJavelins = new Point[MAX_STICKY_JAVELINS];

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			IsStickingToTarget = true;
			TargetWhoAmI = target.whoAmI;
			projectile.velocity = (target.Center - projectile.Center) * 0.75f;
			projectile.netUpdate = true;
			UpdateStickyJavelins(target);
		}

		public override bool PreKill(int timeLeft)
		{
			projectile.type = ProjectileID.RocketI;
			return true;
		}

		private void UpdateStickyJavelins(NPC target)
		{
			int currentJavelinIndex = 0;

			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile currentProjectile = Main.projectile[i];
				if (i != projectile.whoAmI && currentProjectile.active && currentProjectile.owner == Main.myPlayer && currentProjectile.type == projectile.type && currentProjectile.modProjectile is PrithivaVanquisherSpear javelinProjectile && javelinProjectile.IsStickingToTarget && javelinProjectile.TargetWhoAmI == target.whoAmI)
				{
					_stickingJavelins[currentJavelinIndex++] = new Point(i, currentProjectile.timeLeft);
					if (currentJavelinIndex >= _stickingJavelins.Length)
						break;
				}
			}

			if (currentJavelinIndex >= MAX_STICKY_JAVELINS)
			{
				int oldJavelinIndex = 0;
				for (int i = 1; i < MAX_STICKY_JAVELINS; i++)
				{
					if (_stickingJavelins[i].Y < _stickingJavelins[oldJavelinIndex].Y)
					{
						oldJavelinIndex = i;
					}
				}
				Main.projectile[_stickingJavelins[oldJavelinIndex].X].Kill();
			}
		}

		private const int MAX_TICKS = 45;
		private const int ALPHA_REDUCTION = 25;

		public override void AI()
		{
			UpdateAlpha();
			if (IsStickingToTarget) StickyAI();
			else NormalAI();
			Lighting.AddLight(projectile.position, 0.35f, 0.25f, 0f);
			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 2)
			{
				projectile.tileCollide = false;
				projectile.position = projectile.Center;
				projectile.alpha = 255;
				projectile.width = 150;
				projectile.height = 150;
				projectile.Center = projectile.position;
			}
			return;
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) {
			projectile.timeLeft = 2;
			return;
		}
		
		public override bool OnTileCollide(Vector2 oldVelocity)
        {
			projectile.timeLeft = 2;
			return false;
        }

		private void UpdateAlpha()
		{
			if (projectile.alpha > 0)
			{
				projectile.alpha -= ALPHA_REDUCTION;
			}

			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
		}

		private void NormalAI()
		{
			TargetWhoAmI++;
			
			if (TargetWhoAmI >= MAX_TICKS)
			{
				const float velXmult = 0.98f;
				const float velYmult = 0.35f;
				TargetWhoAmI = MAX_TICKS;
				projectile.velocity.X *= velXmult;
				projectile.velocity.Y += velYmult;
			}
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.ToRadians(90f); 
		}

		private void StickyAI()
		{
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			const int aiFactor = 5;
			projectile.localAI[0] += 1f;
			bool hitEffect = projectile.localAI[0] % 30f == 0f;
			int projTargetIndex = (int)TargetWhoAmI;
			if (projectile.localAI[0] >= 60 * aiFactor || projTargetIndex < 0 || projTargetIndex >= 200)
			{
				projectile.Kill();
			}
			else if (Main.npc[projTargetIndex].active && !Main.npc[projTargetIndex].dontTakeDamage)
			{
				projectile.Center = Main.npc[projTargetIndex].Center - projectile.velocity * 2f;
				projectile.gfxOffY = Main.npc[projTargetIndex].gfxOffY;
				if (hitEffect)
				{
					Main.npc[projTargetIndex].HitEffect(0, 1.0);
				}
			}
			else
			{
				projectile.Kill();
			}
		}
	}
}
