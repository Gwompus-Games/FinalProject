using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DunGen.Culling.Shared
{
	public class Room
	{
		/// <summary>
		/// The tile toom is based on
		/// </summary>
		public Tile Tile { get; }

		/// <summary>
		/// World space bounds of the room
		/// </summary>
		public Bounds Bounds { get; private set; }

		/// <summary>
		/// Portals (doorways) in the room
		/// </summary>
		public Portal[] Portals { get; private set; }

		/// <summary>
		/// All renderers in the room. Calculated in `RefreshRenderers()` by getting all child renderers of the tile root
		/// </summary>
		public Renderer[] Renderers { get; private set; }

		/// <summary>
		/// A set of additional renderers that should be considered part of this room. This is not populated automatically and must be managed manually
		/// </summary>
		public readonly List<Renderer> AdditionalRenderers = new List<Renderer>();

		public event Action<Room> RenderersChanged;


		public Room(Tile tile)
		{
			Tile = tile;
			RefreshBounds();
			RefreshRenderers();
		}

		public void RefreshBounds()
		{
			Bounds = Tile.Bounds;
		}

		/// <summary>
		/// Refreshes the list of portals in this room by getting all used doorways from the tile and finding the connected rooms from the provided list
		/// </summary>
		/// <param name="allRooms">A complete set of rooms in the scene</param>
		public void RefreshPortals(List<Room> allRooms)
		{
			Portals = Tile.UsedDoorways.Select(d => new Portal()
			{
				From = this,
				To = allRooms.FirstOrDefault(x => x.Tile == d.ConnectedDoorway.Tile),
				DoorComponent = d.DoorComponent,
				Transform = d.transform,
				Size = d.Socket.Size,
			}).ToArray();

			foreach(var portal in Portals)
				portal.RefreshRenderers();
		}

		/// <summary>
		/// Refreshes the list of renderers in this room by getting all child renderers of the tile root
		/// </summary>
		public void RefreshRenderers()
		{
			Renderers = Tile.GetComponentsInChildren<Renderer>(true);

			if (Portals != null)
			{
				foreach (var portal in Portals)
					portal.RefreshRenderers();
			}

			Refresh();
		}

		/// <summary>
		/// Triggers an event to notify that the renderers have changed. This method raises the <see cref="RenderersChanged"/> event, allowing subscribers to respond to
		/// changes in the renderers.</summary>
		public void Refresh() => RenderersChanged?.Invoke(this);
	}
}
