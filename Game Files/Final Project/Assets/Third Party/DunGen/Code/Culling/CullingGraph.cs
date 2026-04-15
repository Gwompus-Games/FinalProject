using DunGen.Culling.Shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DunGen.Culling
{
	public sealed class CullingGraph
	{
		public ReadOnlyCollection<Room> Rooms
		{
			get
			{
				roomsReadonly ??= allRooms.AsReadOnly();
				return roomsReadonly;
			}
		}

		public event Action<Room> RoomAdded;
		public event Action<Room> RoomChanged;
		public event Action GraphChanged;

		private ReadOnlyCollection<Room> roomsReadonly;
		private readonly Dictionary<Tile, Room> tileRoomMap = new Dictionary<Tile, Room>();
		private readonly HashSet<CompositeDungeon> compositeDungeons = new HashSet<CompositeDungeon>();
		private readonly HashSet<Dungeon> dungeons = new HashSet<Dungeon>();
		private readonly List<Room> allRooms = new List<Room>();


		public bool TryGetCullingRoomByTile(Tile tile, out Room room)
		{
			if (tile == null)
			{
				room = null;
				return false;
			}

			return tileRoomMap.TryGetValue(tile, out room);
		}

		public void AddCompositeDungeon(CompositeDungeon compositeDungeon)
		{
			if (compositeDungeon == null || compositeDungeons.Contains(compositeDungeon))
				return;

			compositeDungeons.Add(compositeDungeon);

			// Listen to dungeon events
			compositeDungeon.DungeonAdded += AddDungeon;
			compositeDungeon.DungeonRemoved += RemoveDungeon;

			// Add all existing dungeons in the composite dungeon
			foreach (var dungeon in compositeDungeon.Dungeons)
				AddDungeon(dungeon);
		}

		public void RemoveCompositeDungeon(CompositeDungeon compositeDungeon)
		{
			if (compositeDungeon == null || !compositeDungeons.Contains(compositeDungeon))
				return;

			compositeDungeons.Remove(compositeDungeon);

			// Stop listening to dungeon events
			compositeDungeon.DungeonAdded -= AddDungeon;
			compositeDungeon.DungeonRemoved -= RemoveDungeon;
		}

		public void AddDungeon(Dungeon dungeon)
		{
			if(dungeon == null || dungeons.Contains(dungeon))
				return;

			dungeons.Add(dungeon);

			var newTiles = dungeon.AllTiles.ToArray();
			var newRooms = new List<Room>();

			// Add the new rooms based on the dungeon tiles
			foreach (var tile in newTiles)
			{
				if(tileRoomMap.ContainsKey(tile))
					continue;

				var room = new Room(tile);
				allRooms.Add(room);
				tileRoomMap[tile] = room;
				newRooms.Add(room);

				room.RenderersChanged += OnRoomChanged;

				RoomAdded?.Invoke(room);
			}

			// Refresh portals for new rooms
			foreach (var room in newRooms)
			{
				room.RefreshPortals(allRooms);

				// Also refresh portals for any rooms that were already in the dungeon that any of the new rooms are connected to
				var attachedRooms = room.Tile.UsedDoorways
					.Where(x => !newTiles.Contains(x.ConnectedDoorway.Tile))
					.Select(x => allRooms.FirstOrDefault(r => r.Tile == x));

				foreach (var attachedRoom in attachedRooms)
					attachedRoom.RefreshPortals(allRooms);
			}

			GraphChanged?.Invoke();
		}

		private void OnRoomChanged(Room room) => RoomChanged?.Invoke(room);

		public void RemoveDungeon(Dungeon dungeon)
		{
			if(dungeon == null || !dungeons.Contains(dungeon))
				return;

			var removedTiles = dungeon.AllTiles.ToArray();

			foreach (var tile in removedTiles)
			{
				var room = tileRoomMap[tile];
				tileRoomMap.Remove(tile);

				allRooms.Remove(room);
				room.RenderersChanged -= OnRoomChanged;
			}

			// Refresh portals for remaining rooms
			foreach (var room in allRooms)
				room.RefreshPortals(allRooms);

			GraphChanged?.Invoke();
		}
	}
}
