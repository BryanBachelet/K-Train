using GuerhoubaGames.GameEnum;
using SeekerOfSand.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FragmentCornerElemental : MonoBehaviour
{
    [ColorUsage(true, true)]
    public List<Color> ColorList;
    [ColorUsage(true, true)]
    public Color[] ColorElement = new Color[4];

    [Range(1, 4)]
    public int ElementNumber;

    public Image[] imageFragment;
    public Material myMat;
    public Material mat;
    // Start is called before the first frame update
    private void OnEnable()
    {
        if (mat != null) return;
        mat = new Material(myMat);
        for(int i = 0; i < imageFragment.Length;i++)
        {
            imageFragment[i].material = mat;
        }

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeFragmentDisplay(ArtefactsInfos artefactInfo)
    {
        if(mat == null) mat = new Material(myMat);
        for(int i = 0;i<imageFragment.Length; i++)
        {
            imageFragment[i].material = mat;
        }

        GameElement baseElement = GeneralTools.GetFirstBaseElement(artefactInfo.gameElement);
        GameElement[] baseElements = GeneralTools.GetBaseElementsArray(artefactInfo.gameElement);


        //mat.SetTexture("_Alpha", artefactInfo.icon.texture);
        ColorList.Clear();
        for (int i = 0; i < baseElements.Length; i++)
        {
            mat.SetColor("_Color" + (i + 1), ColorElement[GeneralTools.GetElementalArrayIndex(baseElements[i])]);
            ColorList.Add(ColorElement[GeneralTools.GetElementalArrayIndex(baseElements[i])]);
        }

        mat.SetFloat("_ColorNumber", baseElements.Length);


    }
    public void ChangeMultipleFill()
    {
        int colorCount = ColorList.Count;
        for (int i = 0; i < imageFragment.Length; i++)
        {
            if (ElementNumber == 1)
            {
                imageFragment[i].material.SetColor("_Color1", ColorList[0]);
                imageFragment[i].material.SetColor("_Color2", ColorList[0]);
                imageFragment[i].material.SetColor("_Color3", ColorList[0]);
                imageFragment[i].material.SetColor("_Color4", ColorList[0]);
            }
            if (ElementNumber == 2)
            {
                imageFragment[i].material.SetColor("_Color1", ColorList[0]);
                imageFragment[i].material.SetColor("_Color2", ColorList[1]);
                imageFragment[i].material.SetColor("_Color3", ColorList[0]);
                imageFragment[i].material.SetColor("_Color4", ColorList[1]);
            }
            else if (ElementNumber == 3)
            {
                imageFragment[i].material.SetColor("_Color1", ColorList[0]);
                imageFragment[i].material.SetColor("_Color2", ColorList[1]);
                imageFragment[i].material.SetColor("_Color3", ColorList[2]);
                imageFragment[i].material.SetColor("_Color4", Color.Lerp(ColorList[1], ColorList[0], 0.5f));
            }
            else if (ElementNumber == 4)
            {
                imageFragment[i].material.SetColor("_Color1", ColorList[0]);
                imageFragment[i].material.SetColor("_Color2", ColorList[1]);
                imageFragment[i].material.SetColor("_Color3", ColorList[2]);
                imageFragment[i].material.SetColor("_Color4", ColorList[3]);
            }


            imageFragment[i].material.SetFloat("_ColorNumber", ElementNumber);
        }
        
    }
}