// Description : EditorManipulate2DTexture.cs : Methods to manipulate 2D Texture in custom Editor
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EditorManipulate2DTexture{

// --> use to change the GUIStyle in custom editor
	public Texture2D MakeTex(int width, int height, Color col) {					
		Color[] pix = new Color[width * height];
		for (int i = 0; i < pix.Length; ++i) {
			pix[i] = col;
		}
		Texture2D result = new Texture2D(width, height);
		result.SetPixels(pix);
		result.Apply();
		return result;
	}

// --> Generate Texture From a Sprite
	public Texture2D GenerateTextureFromSprite(Sprite aSprite)
	{
		var rect = aSprite.rect;
		var tex = new Texture2D((int)rect.width, (int)rect.height);
		var data = aSprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);

		tex.SetPixels(data);
		tex.Apply(true);


		return tex;
	}

// --> Rotate 2D Texture
	public Texture2D  FlippeTexture270(Texture2D tex){
		Texture2D flipped = new Texture2D(tex.width,tex.height);

		int xN = tex.width;
		int yN = tex.height;


		for(int i=0;i<xN;i++){
			for(int j=0;j<yN;j++){
				//flipped.SetPixel(j, i, tex.GetPixel(i,j));
				flipped.SetPixel(j, i, tex.GetPixel(i, tex.height- j));
			}
		}
		flipped.Apply(true);
		return flipped;
	}

	public Texture2D  FlippeTexture180(Texture2D tex){
		Texture2D flipped = new Texture2D(tex.width,tex.height);

		int xN = tex.width;
		int yN = tex.height;


		for(int i=0;i<xN;i++){
			for(int j=0;j<yN;j++){
				//flipped.SetPixel(xN-i-1, yN-j-1, tex.GetPixel(i,j));
				flipped.SetPixel(j, i, tex.GetPixel(tex.height- j, tex.width-i));
			}
		}
		flipped.Apply(true);
		return flipped;
	}

	public Texture2D  FlippeTexture90(Texture2D tex){
		Texture2D flipped = new Texture2D(tex.width,tex.height);

		int xN = tex.width;
		int yN = tex.height;


		for(int i=0;i<xN;i++){
			for(int j=0;j<yN;j++){
				//flipped.SetPixel(i, j, tex.GetPixel(i,j));
				flipped.SetPixel(j, i, tex.GetPixel(tex.width-i, j));
			}
		}
		flipped.Apply(true);
		return flipped;
	}



}
