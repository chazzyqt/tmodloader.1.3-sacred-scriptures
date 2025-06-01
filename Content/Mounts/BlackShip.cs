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

namespace SacredScriptures.Content.Mounts
{
    public class BlackShip : ModMountData
    {
        public override void SetDefaults()
        {
            mountData.spawnDust = 160;
            mountData.buff = mod.BuffType("BlackShip_Buff");
            mountData.heightBoost = 20;
            mountData.fallDamage = 0f;
            mountData.runSpeed = 4f;
            mountData.dashSpeed = 17f;
            mountData.flightTimeMax = 320;
            mountData.fatigueMax = 320;
            mountData.jumpHeight = 10;
            mountData.acceleration = 0.3f;
            mountData.jumpSpeed = 10f;
            mountData.blockExtraJumps = true;
            mountData.totalFrames = 10;           
			mountData.usesHover = true;
            int[] array = new int[mountData.totalFrames];
            for (int l = 0; l < array.Length; l++)
            {
                array[l] = 16;
            }
			mountData.playerYOffsets = array;
			mountData.yOffset = -1;
			mountData.xOffset = 2;
			mountData.bodyFrame = 3;
            mountData.playerHeadOffset = 22;

            if (Main.netMode != NetmodeID.Server)
            {
                mountData.textureWidth = mountData.frontTexture.Width;
                mountData.textureHeight = mountData.frontTexture.Height;
            }
        }

        //float num6;
        public override void UpdateEffects(Player player) //this is like mostly just decompiled vanilla flying mount code because using the default flying mount code did not work for custom animation style iirc
		{
            //Lighting.AddLight(player.position, 0f, 0.5f, 1f); 
			player.gravity = 0;
			player.fallStart = (int)(player.position.Y / 16.0);
            float num1 = 0.5f;
            float acc = 0.4f;

            float xylevel = -1f / 10000f;

			if (player.controlUp || player.controlJump)
            {
				xylevel = -5f;
				player.velocity.Y -= acc * num1;
            }
            else if (player.controlDown)
            {
				player.velocity.Y += acc * num1;
				if (TileID.Sets.Platforms[Framing.GetTileSafely((int)(player.Center.X / 16), (int)((player.MountedCenter.Y + (player.height / 2)) / 16) + 1).type])
					player.position.Y += 1;

				xylevel = 5f;
            }

            if (player.velocity.Y < xylevel)
            {
                if (xylevel - player.velocity.Y < acc)
					player.velocity.Y = xylevel;
                else
					player.velocity.Y += acc * num1;
            }
            else if (player.velocity.Y > xylevel)
            {
                if (player.velocity.Y - xylevel < acc)
					player.velocity.Y = xylevel;
                else
					player.velocity.Y -= acc * num1;
			}

            // This code spawns some dust if we are moving fast enough.
			if (!(Math.Abs(player.velocity.X) > 10f)) {
				return;
			}
			Rectangle rect = player.getRect();
			Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, ModContent.DustType<Content.Dusts.ShipDust>());
        }
        
        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            if (Math.Abs(mountedPlayer.velocity.X) > 2)
            {
                if (Math.Abs(mountedPlayer.velocity.X) > 10)
                {
                    mountedPlayer.mount._frameCounter += 0.5f;
                    mountedPlayer.mount._frame = (int)(mountedPlayer.mount._frameCounter %= 10);
                }
                else
                {
                    mountedPlayer.mount._frameCounter += 0.25f;
                    mountedPlayer.mount._frame = (int)(mountedPlayer.mount._frameCounter %= 10);
                }  
            }

            else
            {
                mountedPlayer.mount._frameCounter += 0.12f;
                mountedPlayer.mount._frame = (int)(mountedPlayer.mount._frameCounter %= 10);
            }
            return false;
        }
        
        public override bool Draw(List<Terraria.DataStructures.DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            glowTexture = mod.GetTexture("Content/Glow/BlackShip_Glow");
            /*
            if (drawPlayer.velocity.X < 0 && drawPlayer.direction > 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                drawOrigin.X -= 52;//52
            }
            if (drawPlayer.velocity.X > 0 && drawPlayer.direction < 0)
            {
                spriteEffects = 0;
                drawOrigin.X += 52;//52
            }
            */
            return true;
        }
    }
}