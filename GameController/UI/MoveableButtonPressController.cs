﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveableButtonPressController : MonoBehaviour
{
    public PlaceItemController placeItemController;
    public RemoveItemController removeItemController;

    public char key;
    public KeyCode code;

    Button button;
    Text text;    

    private void Start()
    {
        button = transform.GetComponentInChildren<Button>();
        button.onClick.AddListener(OnButtonPress);

        text = transform.GetComponentInChildren<Text>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(code))
        {
            OnButtonPress();
        }
    }

    private void OnButtonPress()
    {
        if (text.text == "0") return;

        if(placeItemController.key == key)
        {
            removeItemController.enabled = true;
            placeItemController.DisableController();
        }
        else
        {
            removeItemController.Disable();
            placeItemController.SetKey(key);
            placeItemController.enabled = true;
        } 
    }
}
