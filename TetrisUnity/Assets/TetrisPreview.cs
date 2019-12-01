using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPreview : MonoBehaviourPunCallbacks
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] previewSprites;

    public void changeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
        int i = 0;
        foreach (var sp in previewSprites) {
            if (sp.name == sprite.name) {
                photonView.RPC("UpdateSprite", RpcTarget.Others, i);
            }
            i++;
        }


    }
    [PunRPC]
    void UpdateSprite(int imageIndex)
    {
        spriteRenderer.sprite = previewSprites[imageIndex];
    }
}
