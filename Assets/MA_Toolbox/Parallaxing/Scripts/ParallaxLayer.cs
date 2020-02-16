using UnityEngine;

namespace MA_Toolbox.Parallaxing
{
	[ExecuteInEditMode]
	public class ParallaxLayer : MonoBehaviour
	{
		public ParallaxLayerSetting layerSetting = null;
		public int sortingOrder = 0;

		private ParallaxCamera paralaxCamera = null;
		private Transform cameraTransform = null;
		private Vector3 previousCameraPosition;
		private bool previousMoveParallax;

		private float speedX = 0.0f;
		private float speedY = 0.0f;

		private Bounds layerBounds = new Bounds();

		private SpriteRenderer[] spriteChildren = new SpriteRenderer[0];

		public void Init()
		{
			spriteChildren = this.transform.GetComponentsInChildren<SpriteRenderer>();

			if (layerSetting != null)
			{
				speedX = Mathf.Round(layerSetting.layerSpeedCurveX.Evaluate(sortingOrder) * 100f) / 100f;
				speedY = Mathf.Round(layerSetting.layerSpeedCurveY.Evaluate(sortingOrder) * 100f) / 100f;
			}

			cameraTransform = Camera.main.transform;
			previousCameraPosition = cameraTransform.position;
			paralaxCamera = cameraTransform.GetComponent<ParallaxCamera>();

			UpdateBounds();
		}

		private void OnEnable()
		{
			Init();
		}

		private void Update()
		{
			//Return if we don't need to update.
			if (paralaxCamera == null) { return; }

			//Parallaxing.
			if (layerSetting != null && speedX != 0.0f && speedY != 0.0f)
			{
				if (paralaxCamera.Parallaxing && !previousMoveParallax)
				{
					previousCameraPosition = cameraTransform.position;
				}

				previousMoveParallax = paralaxCamera.Parallaxing;

				if (/*Application.isPlaying &&*/ paralaxCamera.Parallaxing)
				{
					UpdateParallaxing();
				}
			}

			UpdateBounds();

			//Culling.
			if (Application.isPlaying && paralaxCamera.Culling)
			{
				UpdateCulling();
			}
		}

		private void UpdateParallaxing()
		{
			Vector3 distance = cameraTransform.position - previousCameraPosition;
			float direction = (layerSetting.flip) ? -1f : 1f;
			transform.position += Vector3.Scale(distance, new Vector3(speedX, speedY)) * direction;
			previousCameraPosition = cameraTransform.position;
		}

		private void UpdateBounds()
		{
			//Enable renderers.
			foreach (SpriteRenderer sr in spriteChildren)
			{
				sr.enabled = true;
			}

			//Center.
			Vector3 center = new Vector3();

			foreach (SpriteRenderer sr in spriteChildren)
			{
				center += sr.bounds.center;
			}

			layerBounds.center = center / spriteChildren.Length;

			//Min max.
			Vector2 min = layerBounds.center;
			Vector2 max = layerBounds.center;

			foreach (SpriteRenderer sr in spriteChildren)
			{
				if (sr.bounds.min.x < min.x)
				{
					min.x = sr.bounds.min.x;
				}

				if (sr.bounds.min.y < min.y)
				{
					min.y = sr.bounds.min.y;
				}

				if (sr.bounds.max.x > max.x)
				{
					max.x = sr.bounds.max.x;
				}

				if (sr.bounds.max.y > max.y)
				{
					max.y = sr.bounds.max.y;
				}
			}

			layerBounds.SetMinMax(min, max);
		}

		void UpdateCulling()
		{
			if (paralaxCamera.cameraBounds.Intersects(layerBounds))
			{
				foreach (SpriteRenderer sr in spriteChildren)
				{
					sr.enabled = true;
				}
			}
			else
			{
				foreach (SpriteRenderer sr in spriteChildren)
				{
					sr.enabled = false;
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				DrawGizmos();
			}
		}

		private void OnDrawGizmosSelected()
		{
			if (!Application.isPlaying)
			{
				DrawGizmos();
			}
		}

		private void DrawGizmos()
		{
			if (layerBounds != null)
			{
				if (paralaxCamera.cameraBounds.Intersects(layerBounds))
				{
					Gizmos.color = Color.green;
				}
				else
				{
					Gizmos.color = Color.red;
				}

				Gizmos.DrawWireCube(layerBounds.center, layerBounds.size);

				foreach (SpriteRenderer sr in spriteChildren)
				{
					Gizmos.DrawWireCube(sr.bounds.center, sr.bounds.size);
				}

				Gizmos.color = Color.white;
			}
		}
	}
}