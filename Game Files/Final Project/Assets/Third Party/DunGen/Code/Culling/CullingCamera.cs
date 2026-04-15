using DunGen.Culling.Shared;
using DunGen.Culling.Strategies;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if RENDER_PIPELINE
using UnityEngine.Rendering;
#endif

namespace DunGen.Culling
{
	[DisallowMultipleComponent]
	[AddComponentMenu("DunGen/Culling/Culling Camera")]
	[RequireComponent(typeof(Camera))]
	[HelpURL("https://dungen-docs.aegongames.com/optimization/culling/")]
	public class CullingCamera : MonoBehaviour
	{
		#region Statics

		public static CullingGraph CullingGraph { get; private set; } = new CullingGraph();
		public static IReadOnlyCollection<CullingCamera> ActiveCameras => activeCameras;

		protected static HashSet<CullingCamera> activeCameras = new HashSet<CullingCamera>();

		protected static void RegisterCamera(CullingCamera camera)
		{
			activeCameras.Add(camera);

			if (activeCameras.Count == 1)
			{
				DungeonGenerator.OnAnyDungeonGenerationComplete += OnAnyDungeonGenerationComplete;

				// Find any existing dungeons in the scene and add them to the culling graph
				foreach (var runtimeDungeon in UnityUtil.FindObjectsByType<RuntimeDungeon>())
				{
					if (runtimeDungeon.Generator.CompositeDungeon != null)
						CullingGraph.AddCompositeDungeon(runtimeDungeon.Generator.CompositeDungeon);
				}
			}

			if (activeCameras.Count > 1)
			{
				if (activeCameras.Any(c => !c.PerCameraCulling))
					Debug.LogWarning("Multiple CullingCamera components are active in the scene, and at least one is not using the per-camera culling. If you need multiple culling cameras active at once, they should all have per-camera culling enabled.", camera);
			}
		}

		protected static void UnregisterCamera(CullingCamera camera)
		{
			activeCameras.Remove(camera);

			if (activeCameras.Count == 0)
				DungeonGenerator.OnAnyDungeonGenerationComplete -= OnAnyDungeonGenerationComplete;
		}

		protected static void OnAnyDungeonGenerationComplete(DungeonGenerator generator, Dungeon dungeon)
		{
			if (generator.CompositeDungeon != null)
				CullingGraph.AddCompositeDungeon(generator.CompositeDungeon);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitialiseStatics()
		{
			CullingGraph = new CullingGraph();
			activeCameras.Clear();
		}

		#endregion

		[SubclassSelector(allowNone: false)]
		[SerializeReference]
		public ICullingStrategy Strategy = new PortalCullingStrategy();
		public bool PerCameraCulling = false;
		public bool DebugDraw = false;
		public Camera CameraComponent => cameraComponent;
		public CullingContext Context { get; private set; }

		protected Camera cameraComponent;
		protected bool hookedRenderPipeline;


		protected virtual void OnEnable()
		{
			cameraComponent = GetComponent<Camera>();

			if (Context == null)
				Context = new CullingContext(this);

			RegisterCamera(this);
			CullingGraph.RoomAdded += OnRoomAdded;
			CullingGraph.RoomChanged += OnRoomChanged;
			CullingGraph.GraphChanged += OnCullingGraphChanged;

#if RENDER_PIPELINE
			if (RenderPipelineManager.currentPipeline != null)
			{
				RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
				RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
				hookedRenderPipeline = true;
			}
#endif

			if (!hookedRenderPipeline)
			{
				Camera.onPreCull += OnCameraPreCull;
				Camera.onPostRender += OnCameraPostRender;
			}

			// Hide all rooms initially
			foreach (var room in CullingGraph.Rooms)
				ApplyRoomVisibility(room, false);

			Strategy?.OnEnable(this);
		}

		protected virtual void OnDisable()
		{
			UnregisterCamera(this);

			CullingGraph.RoomAdded -= OnRoomAdded;
			CullingGraph.RoomChanged -= OnRoomChanged;
			CullingGraph.GraphChanged -= OnCullingGraphChanged;
			Context.ResetCaches();

#if RENDER_PIPELINE
			RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
			RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
#endif

			Camera.onPreCull -= OnCameraPreCull;
			Camera.onPostRender -= OnCameraPostRender;
			hookedRenderPipeline = false;

			if (!PerCameraCulling)
			{
				foreach (var room in CullingGraph.Rooms)
					ApplyRoomVisibility(room, true);
			}

			Strategy?.OnDisable(this);
		}

		protected virtual void OnCullingGraphChanged()
		{
			foreach (var room in CullingGraph.Rooms)
				ApplyRoomVisibility(room, false);
		}

		protected void ApplyRoomVisibility(Room room, bool visible)
		{
			if (room == null)
				return;

			foreach (var renderer in room.Renderers)
				ApplyRendererVisibility(renderer, visible);

			foreach (var renderer in room.AdditionalRenderers)
				ApplyRendererVisibility(renderer, visible);
		}

		protected void ApplyRendererVisibility(Renderer renderer, bool visible)
		{
			if (renderer != null)
				renderer.forceRenderingOff = !visible;
		}

#if RENDER_PIPELINE
		protected void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			EnableCullingForCamera(camera);
		}

		protected void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
		{
			DisableCullingForCamera(camera);
		}
#endif

		protected virtual void OnCameraPreCull(Camera camera) => EnableCullingForCamera(camera);

		protected virtual void OnCameraPostRender(Camera camera) => DisableCullingForCamera(camera);

		protected virtual void EnableCullingForCamera(Camera camera)
		{
			if (!PerCameraCulling || camera != cameraComponent)
				return;

			Context.VisibleRooms.Clear();
			Strategy?.GetVisibleRooms(cameraComponent, CullingGraph.Rooms, ref Context.VisibleRooms);

			foreach (var room in Context.VisibleRooms)
				ApplyRoomVisibility(room, true);
		}

		protected virtual void DisableCullingForCamera(Camera camera)
		{
			if (!PerCameraCulling || camera != cameraComponent)
				return;

			foreach (var room in Context.VisibleRooms)
				ApplyRoomVisibility(room, false);
		}

		protected virtual void LateUpdate()
		{
			if (PerCameraCulling)
				return;

			if (Strategy == null && Context.VisibleRooms.Count == CullingGraph.Rooms.Count)
				return; // No strategy set and all rooms are already visible

			// Swap the previous visible rooms with the current ones
			(Context.VisibleRooms, Context.PreviousVisibleRooms) = (Context.PreviousVisibleRooms, Context.VisibleRooms);
			Context.VisibleRooms.Clear();

			// Calculate the visible set of rooms
			if (Strategy != null)
				Strategy.GetVisibleRooms(cameraComponent, CullingGraph.Rooms, ref Context.VisibleRooms);
			else
			{
				// No strategy, so just make everything visible
				foreach (var room in CullingGraph.Rooms)
					Context.VisibleRooms.Add(room);
			}

			Context.NewlyVisibleRooms.Clear();
			Context.NewlyHiddenRooms.Clear();

			// Find newly visible rooms
			foreach (var room in Context.VisibleRooms)
			{
				if (!Context.PreviousVisibleRooms.Contains(room))
				{
					Context.NewlyVisibleRooms.Add(room);
					ApplyRoomVisibility(room, true);
				}
			}

			// Find newly hidden rooms
			foreach (var room in Context.PreviousVisibleRooms)
			{
				if (!Context.VisibleRooms.Contains(room))
				{
					Context.NewlyHiddenRooms.Add(room);
					ApplyRoomVisibility(room, false);
				}
			}

			// If a portal is shared between a visible and hidden room, we need to render it
			GetThresholdPortals(Context.VisibleRooms, ref Context.ThresholdPortals);

			foreach (var portal in Context.ThresholdPortals)
			{
				foreach (var renderer in portal.Renderers)
					ApplyRendererVisibility(renderer, true);
			}
		}

		// Gets the portals that are on the threshold of visibility (between a visible and hidden room)
		protected void GetThresholdPortals(HashSet<Room> visibleRooms, ref HashSet<Portal> thresholdPortals)
		{
			thresholdPortals.Clear();

			foreach (var room in visibleRooms)
			{
				foreach (var portal in room.Portals)
				{
					if (visibleRooms.Contains(portal.To))
						continue; // Adjacent room is already visible

					thresholdPortals.Add(portal);
				}
			}
		}

		private void OnRoomAdded(Room newRoom)
		{
			if (enabled && !PerCameraCulling)
				ApplyRoomVisibility(newRoom, false);
		}

		private void OnRoomChanged(Room room)
		{
			if (enabled && !PerCameraCulling)
				ApplyRoomVisibility(room, Context.VisibleRooms.Contains(room));
		}

		protected virtual void OnDrawGizmos()
		{
			if (enabled && DebugDraw)
				Strategy?.DebugDraw();
		}
	}
}
