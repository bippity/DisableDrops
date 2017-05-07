using System;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace DisableDrops
{
	[ApiVersion(2, 1)]
    public class DisableDrops : TerrariaPlugin
    {
		public override Version Version
		{
			get { return new Version("1.0"); }
		}

		public override string Name
		{
			get { return "Disable Drops"; }
		}

		public override string Author
		{
			get { return "Bippity"; }
		}

		public override string Description
		{
			get { return "Disable players from dropping items."; }
		}

		public DisableDrops(Main game) : base(game)
		{
			Order = 1;
		}

		public override void Initialize()
		{
			ServerApi.Hooks.NetGetData.Register(this, GetData);
			Commands.ChatCommands.Add(new Command("disabledrops.edit", DisableDropsCommand, "disabledrops"));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, GetData);
			}
			base.Dispose(disposing);
		}


		bool enabled = true;
		public void DisableDropsCommand(CommandArgs args)
		{
			enabled = !enabled;

			if (enabled)
			{
				args.Player.SendWarningMessage("[Disable Drops] Drop disabling is enabled");
			}
			else
				args.Player.SendWarningMessage("[Disable Drops] Drop disabling is disabled");
		}

		public void GetData(GetDataEventArgs e)
		{
			if (e.MsgID == PacketTypes.ItemDrop)
			{
				if (e.Handled)
					return;

				if (enabled)
				{
					TSPlayer player = TShock.Players[e.Msg.whoAmI];
					if (!player.HasPermission("disabledrops.bypass"))
					{
						e.Handled = true;
						player.SendErrorMessage("[Disable Drops] You are not allowed to drop items.");
					}
				}
			}
		}
	}
}
