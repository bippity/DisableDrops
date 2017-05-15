using System;
using System.IO;
using System.IO.Streams;
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
			ServerApi.Hooks.NetGetData.Register(this, OnGetData);
			Commands.ChatCommands.Add(new Command("disabledrops.edit", DisableDropsCommand, "disabledrops"));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
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

		public void OnGetData(GetDataEventArgs args)
		{
			if (args.MsgID == PacketTypes.ItemDrop)
			{
				if (args.Handled || !enabled)
					return;

				TSPlayer player = TShock.Players[args.Msg.whoAmI];

				if (!player.HasPermission("disabledrops.bypass"))
				{
					using (var data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
					{
						Int16 id = data.ReadInt16();
						float posx = data.ReadSingle();
						float posy = data.ReadSingle();
						float velx = data.ReadSingle();
						float vely = data.ReadSingle();
						Int16 stacks = data.ReadInt16();
						int prefix = data.ReadByte();
						bool nodelay = data.ReadBoolean();
						Int16 netid = data.ReadInt16();

						Item item = new Item();
						item.SetDefaults(netid);

						if (id != 400)
							return;

						args.Handled = true;
					}
					player.SendErrorMessage("[Disable Drops] You are not allowed to drop items.");
				}
			}
		}
	}
}
