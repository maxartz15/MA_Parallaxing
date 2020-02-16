using System.Collections.Generic;
using UnityEngine;

namespace MA_Toolbox.Parallaxing
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class ParallaxRepeatedLayer : MonoBehaviour
	{
		public ParallaxLayerSetting layerSetting = null;

		[SerializeField]
		private Sprite[] sprites = new Sprite[0];
		private List<SpriteRenderer> curSprites = new List<SpriteRenderer>();

		private SpriteRenderer spriteRenderer = null;

		private ParallaxCamera paralaxCamera = null;
		private Transform cameraTransform = null;
		private Vector3 previousCameraPosition;
		private bool previousMoveParallax;

		private float speedX = 0.0f;
		private float speedY = 0.0f;

		public void Init()
		{
			if (spriteRenderer == null)
			{
				spriteRenderer = this.GetComponent<SpriteRenderer>();
				spriteRenderer.enabled = false;
			}

			if (layerSetting != null)
			{
				speedX = Mathf.Round(layerSetting.layerSpeedCurveX.Evaluate(spriteRenderer.sortingOrder) * 100f) / 100f;
				speedY = Mathf.Round(layerSetting.layerSpeedCurveY.Evaluate(spriteRenderer.sortingOrder) * 100f) / 100f;
			}

			cameraTransform = Camera.main.transform;
			previousCameraPosition = cameraTransform.position;
			paralaxCamera = cameraTransform.GetComponent<ParallaxCamera>();
		}

		private void OnEnable()
		{
			Init();
		}

		private void Update()
		{
			//Return if we don't need to update.
			if (layerSetting == null || paralaxCamera == null || (speedX == 0.0f && speedY == 0.0f))
			{
				return;
			}

			if (paralaxCamera.Parallaxing && !previousMoveParallax)
			{
				previousCameraPosition = cameraTransform.position;
			}

			previousMoveParallax = paralaxCamera.Parallaxing;

			if (!Application.isPlaying && !paralaxCamera.Parallaxing)
			{
				return;
			}

			//Parallaxing.
			Vector3 distance = cameraTransform.position - previousCameraPosition;
			float direction = (layerSetting.flip) ? -1f : 1f;
			transform.position += Vector3.Scale(distance, new Vector3(speedX, speedY)) * direction;
			previousCameraPosition = cameraTransform.position;

			//Culling and spawning.
			bool isSet = false;
			float maxX = 0;
			float minX = 0;

			for (int i = curSprites.Count - 1; i >= 0; i--)
			{
				if (curSprites[i].bounds.max.x > maxX || !isSet)
				{
					maxX = curSprites[i].bounds.max.x;
				}

				if (curSprites[i].bounds.min.x < minX || !isSet)
				{
					minX = curSprites[i].bounds.min.x;
				}

				isSet = true;

				//Remove sprite when out of bounds.
				if (curSprites[i].bounds.max.x + (curSprites[i].bounds.size.x / 2) < paralaxCamera.cameraBounds.min.x ||
					curSprites[i].bounds.min.x - (curSprites[i].bounds.size.x / 2) > paralaxCamera.cameraBounds.max.x)
				{
					Destroy(curSprites[i].gameObject);
					curSprites.RemoveAt(i);
				}
			}

			//If the camera exceeds the bounds, add a new sprite.
			if (paralaxCamera.cameraBounds.max.x > maxX)
			{
				GameObject go = CreateParallaxSprite();

				SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
				ConfigureSpriteRenderer(ref sr);

				go.transform.position = new UnityEngine.Vector3(maxX + (sr.bounds.size.x / 2), this.transform.position.y, this.transform.position.z);
				curSprites.Add(sr);
			}

			if (paralaxCamera.cameraBounds.min.x < minX)
			{
				GameObject go = CreateParallaxSprite();

				SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
				ConfigureSpriteRenderer(ref sr);

				go.transform.position = new UnityEngine.Vector3(minX - (sr.bounds.size.x / 2), this.transform.position.y, this.transform.position.z);
				curSprites.Add(sr);
			}
		}

		private GameObject CreateParallaxSprite()
		{
			GameObject go = new GameObject("ParallaxRepeatedLayer");
			go.transform.SetParent(this.transform, false);

			return go;
		}

		private void ConfigureSpriteRenderer(ref SpriteRenderer sr)
		{
			sr.sprite = sprites[Random.Range(0, sprites.Length)];
			sr.sortingOrder = spriteRenderer.sortingOrder;
			sr.sortingLayerID = spriteRenderer.sortingLayerID;
			sr.sharedMaterial = spriteRenderer.sharedMaterial;
			sr.color = spriteRenderer.color;
		}
	}
}