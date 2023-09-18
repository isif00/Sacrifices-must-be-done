using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Canvas sacrificeMenu;

    public static bool isInsidePortal;
    public static bool isOutsidePortal;

    public static bool wantDisplayMenu;

    public static Image[] sacrificeMenuImages;

    public static string itemName;

    public static Animator portal;


    private void Start()
    {
        sacrificeMenu = GameObject.Find("SacrificeMenu").GetComponent<Canvas>();

        //Setting up the portal
        portal = GameObject.Find("Portal").GetComponent<Animator>();
        portal.enabled = false;

        //check if the player is inside the portal
        isInsidePortal = false;
        isOutsidePortal = true;

        //SacrificeMenu
        sacrificeMenu.enabled = false;

        //SacrificeMenuImages
        foreach (Image image in sacrificeMenu.GetComponentsInChildren<Image>())
        {
            image.enabled = false;
        }
    }

    private void Update()
    {

        if (isInsidePortal)
        {
            sacrificeMenu.enabled = true;
        }
        else sacrificeMenu.enabled = false;

    }

}
