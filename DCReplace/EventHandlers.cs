using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.CustomRoles.API;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using Exiled.Loader;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace DCReplace
{
    class EventHandlers
	{
		// Don't want to swap players out after the round ends
		private bool isRoundStarted = false;

		public void OnRoundStart()
		{
			isRoundStarted = true;
		}

		public void OnRoundEnd(RoundEndedEventArgs ev) => isRoundStarted = false;
		
		public void OnRoundRestarting() => isRoundStarted = false;
		
		public void OnPlayerLeave(LeftEventArgs ev)
		{
			
			if (!isRoundStarted || ev.Player.Role == RoleTypeId.Spectator || ev.Player.Position.y < -1997 ||
			    (ev.Player.Zone == ZoneType.LightContainment &&
			     (Map.DecontaminationState > DecontaminationState.Remain1Minute)))
			{
				ev.Player.DropItems();
				Log.Debug("Player is not eligible for replacement: "+ev.Player.Nickname);
				return;
			}
			Log.Debug($"Player {ev.Player.Nickname} has left the round. Attempting to replace...");
			
			Player replacement;
			// Give priority to players already spectating the leaver
			if (!ev.Player.CurrentSpectatingPlayers.IsEmpty())
			{
				replacement = ev.Player.CurrentSpectatingPlayers.GetRandomValue(player => player.Role == RoleTypeId.Spectator);
			}
			else
			{
				replacement = Player.List.GetRandomValue(x => x.Role == RoleTypeId.Spectator && x.UserId != string.Empty && x.UserId != ev.Player.UserId && !x.IsOverwatchEnabled);
			}
			Log.Debug("Found replacement: " + (replacement != null ? replacement.Nickname : "null"));
			
			if (replacement != null)
			{

				// save info
				Vector3 pos = ev.Player.Position;
				var inventory = ev.Player.Items;
				var ammo  = ev.Player.Ammo;
				var effects = ev.Player.ActiveEffects;
				float health = ev.Player.Health;
				var customRoles = ev.Player.GetCustomRoles();
				string uniqueRole = ev.Player.UniqueRole;
				var role = ev.Player.Role.Type;
				Log.Debug($"{replacement.Nickname} will replace {ev.Player.Nickname}, who was a {role} with {inventory.Count()} items, had {ammo.Keys.Count} kinds of ammo, {health} HP, had {customRoles.Count} custom roles and {effects.Count()} effects.");
				
				replacement.UniqueRole = uniqueRole;
				
				replacement.RoleManager.ServerSetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
				Log.Debug("Added unique role: "+replacement.UniqueRole);
				Log.Debug("Set role to "+role);

				Timing.CallDelayed(0.3f, () =>
				{
					replacement.Position = pos;
					replacement.ClearInventory();
					replacement.ResetInventory(inventory);
					foreach (var item in inventory)
					{
						Log.Debug("Giving item: "+item.Type);
						replacement.AddItem(item);
					}
					foreach (var ammoType in ammo.Keys)
					{
						Log.Debug($"Setting {ammoType} ammo to {ammo[ammoType]}");
						replacement.Ammo[ammoType] = ammo[ammoType];
					}
					replacement.DisableAllEffects();
					foreach (var effect in effects)
					{
						Log.Debug($"Enabling effect {effect.name} with duration {effect.Duration}");
						replacement.EnableEffect(effect, effect.Duration);
					}
					replacement.Health = health;
					Log.Debug("Set health to "+health);
					
					foreach(var customRole in customRoles)
					{
						Log.Debug("Adding custom role: "+customRole.Name);
						customRole.AddRole(replacement);
					}
					
					replacement.Broadcast(5, "<i>You have replaced a player who has disconnected.</i>");
				});
			}
			else
			{
				//Need to make sure all items are dropped because the server is set to not do so.
				ev.Player.DropItems();
				Log.Debug("No replacement found for "+ev.Player.Nickname);
			}
		}
		
		public void RegisterEvents()
		{
			Exiled.Events.Handlers.Server.RoundStarted += OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnd;
			Exiled.Events.Handlers.Server.RestartingRound += OnRoundRestarting;
			Exiled.Events.Handlers.Player.Left += OnPlayerLeave;
		}
		
		public void UnregisterEvents()
		{
			Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStart;
			Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnd;
			Exiled.Events.Handlers.Server.RestartingRound -= OnRoundRestarting;
			Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;
		}

		
	}
}