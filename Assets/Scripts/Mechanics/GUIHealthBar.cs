﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Health))]
public class GUIHealthBar : MonoBehaviour {

    private Vector2 size;
    private Vector2 position;
    private Texture2D backgroundTex;
    private Texture2D foregroundTex;

    private Health health;


	void Start() {
        health = GetComponent<Health>();

        size = new Vector2(Mathf.Min(Screen.width / 3.0f, 500), Mathf.Min(Screen.height / 10.0f, 25));
        position = new Vector2(Screen.width - size.x, Screen.height - size.y);

        // Create health bar colours
        backgroundTex = new Texture2D(1, 1);
        backgroundTex.SetPixel(0, 0, Color.red);
        backgroundTex.wrapMode = TextureWrapMode.Repeat;
        backgroundTex.Apply();
        foregroundTex = new Texture2D(1, 1);
        foregroundTex.SetPixel(0, 0, Color.green);
        foregroundTex.wrapMode = TextureWrapMode.Repeat;
        foregroundTex.Apply();
	}
	
	void OnGUI() {
        if (gameObject != null && gameObject == Util.localPlayer) {
            float fillPercent = health.GetCurrentHitPoints() / health.GetMaxHitPoints();
            GUIContent content = new GUIContent();
            content.text = (fillPercent * 100) + "%";
            GUI.contentColor = Color.black;
            // Draw the health bar
            {
                GUI.BeginGroup(new Rect(position.x, position.y, size.x, size.y));
                GUI.skin.box.normal.background = backgroundTex;
                GUI.Box(new Rect(0, 0, size.x, size.y), content);
                {
                    GUI.BeginGroup(new Rect(0, 0, size.x * fillPercent, size.y));
                    GUI.skin.box.normal.background = foregroundTex;
                    GUI.Box(new Rect(0, 0, size.x, size.y), content);
                    GUI.EndGroup();
                }
                GUI.EndGroup();
            }
        }
    }
}
