namespace HabitableZone.UnityLogic.PlanetTextureGenerators.Generators
{
	public class CircleDiagram
	{
		/*public float minRadius = 0.7f;
		public float maxRadius;
		public int separator_size;
		public float innerCutoutRadius = 0.3f;
		public float decorCircle_innerRadius = 0.4f;
		public float decorCircle_outerRadius = 0.55f;
		public float fillupRadius = 0.6f;

		public Color separator;
		public Color main;
		public Color inner_circle;

		Color[][] sectorColors;
		int[] coords;

		public void GenDiagram(int[] angles)
		{
			var texture = new Texture2D(XSize, size);
			this.colors2D = new Color[size, size];
			angles[0] = 0;
			float[] radii = new float[angles.Length];
			for (int sector = 0; sector < angles.Length - 1; sector++)
			{
				//angles[i] - начальный угол сектора. Т.е. первый - 0, второй - 42, к примеру, последний - НЕ 360.
				radii[sector] = (1 - Mathf.Clamp01((angles[sector + 1] - angles[sector]) / 90f)) * maxRadius + minRadius;
				Debug.Log("Radius of sector #" + sector + " is " + radii[sector]);
				//Т.е. чем меньше сектор, тем больше выпирает
			}
			//То же самое для последнего:
			radii[angles.Length - 1] = (1 - Mathf.Clamp01((360 - angles[angles.Length - 1]) / 90f)) * maxRadius + minRadius;
			Debug.Log("Last sector radius: " + radii[angles.Length - 1]);

			//Теперь рисуем.
			for (int x = 0; x < size; x++)
			{
				for (int y = 0; y < size; y++)
				{
					int sec = 0;
					bool isSep = false;
					Vector2 pos = new Vector2(x + 1, y + 1);
					Vector2 dir = pos - new Vector2(size / 2, size / 2);
					for (int s = 0; s < angles.Length; s++)
					{
						float angle = Vector2.Angle(Vector2.up, dir);
						if (x + 1 > size / 2)
						{
							angle = 360 - angle;
						}
						if (angle > angles[s])
						{
							if ((angle < angles[s] + separator_size)) //||360-angle<separator_size
							{
								isSep = true; break;
							} else
							{
								sec = s;
							}
						}
					}

					dir = dir / size * 2;

					if (dir.magnitude < decorCircle_outerRadius && dir.magnitude > decorCircle_innerRadius)
					{
						colors2D[x, y] = inner_circle; continue;//Рисуем темное кольцо
					}
					//Или же светлое, возле него
					if (dir.magnitude < fillupRadius && dir.magnitude > innerCutoutRadius)
					{
						colors2D[x, y] = main;continue;
					}
					//Также есть случай, когда надо рисовать разделитель
					if (isSep && dir.magnitude > fillupRadius && dir.magnitude < minRadius + maxRadius)
					{
						colors2D[x, y] = separator; continue;
					}
					//И, в конце концов, сами сектора:
					if (dir.magnitude <= radii[sec])
					{
						colors2D[x, y] = main;
					}
					if (dir.magnitude > radii[sec] || dir.magnitude < innerCutoutRadius / 2)
					{
						colors2D[x, y] = Color.clear;
					}//else{colors2D[x,y]=main;}
				}
			}

			this.Colors2DToColors(size, size);
			this.ColorsToTexture();
		} */
	}
}