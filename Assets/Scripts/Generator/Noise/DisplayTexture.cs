using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(TextureGenerator))]
public class DisplayTexture : MonoBehaviour
{
    public Renderer textureRender;
    public TextureGenerator textureGenerator;

    private void Start()
    {
        textureRender = GetComponent<Renderer>();
        textureGenerator = GetComponent<TextureGenerator>();
    }

    public void DrawTexture()
    {
        Texture2D texture = textureGenerator.GenerateTexture(textureGenerator.textureType);

        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, texture.height, 1);
    }
}
