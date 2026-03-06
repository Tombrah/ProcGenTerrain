using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateTerrain : MonoBehaviour
{
    public Terrain terrain;
    [HideInInspector]
    public GenerateNoiseMap noiseMap;
    [HideInInspector]
    public PaintTerrain colourMap;
    [HideInInspector]
    public TreePlacement treeMap;

    public AnimationCurve lerpCurve;

    [Header("Lerp Variables")]
    public int lowestTerrainHeight;
    public int highestTerrainHeight;

    public int lowestOctaveHeight;
    public int highestOctaveHeight;

    public float desiredDuration = 3f;
    private float elapsedTime;
    private float timeBetweenLoop;

    [HideInInspector]
    public bool isChangingDown;
    private bool isChangingUp;
    [HideInInspector]
    public bool doneChanging;

    [Header("Bools")]
    public bool loop;
    public float refreshRate;
    public bool allowTrees;
    public bool autoUpdate;

    private void Awake()
    {
        noiseMap = terrain.GetComponent<GenerateNoiseMap>();
        colourMap = terrain.GetComponent<PaintTerrain>();
        treeMap = terrain.GetComponent<TreePlacement>();
        allowTrees = false;
        loop = false;
        doneChanging = true;
    }

    private void Start()
    {
        noiseMap.xOffset = Random.Range(-100000, 100000);
        noiseMap.zOffset = Random.Range(-100000, 100000);
        noiseMap.EditTerrain(terrain.terrainData);
        colourMap.PaintTerrainColour();
        treeMap.GenerateTrees();
        treeMap.OffsetTrees();
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / desiredDuration;
        Mathf.Clamp01(percentageComplete);

        timeBetweenLoop += Time.deltaTime;

        if (isChangingDown)
        {
            noiseMap.height = Mathf.CeilToInt(Mathf.Lerp(highestTerrainHeight, lowestTerrainHeight, lerpCurve.Evaluate(percentageComplete)));
            noiseMap.octaves = Mathf.CeilToInt(Mathf.Lerp(highestOctaveHeight, lowestOctaveHeight, percentageComplete));
            noiseMap.EditTerrain(terrain.terrainData);
            colourMap.PaintTerrainColour();
        }
        else if (isChangingUp)
        {
            noiseMap.height = Mathf.CeilToInt(Mathf.Lerp(lowestTerrainHeight, highestTerrainHeight, lerpCurve.Evaluate(percentageComplete)));
            noiseMap.octaves = Mathf.CeilToInt(Mathf.Lerp(lowestOctaveHeight, highestOctaveHeight, percentageComplete));
            noiseMap.EditTerrain(terrain.terrainData);
            colourMap.PaintTerrainColour();
        }
        else
        {
            elapsedTime = 0;
        }

        if (doneChanging)
        {
            if (allowTrees)
            {
                for (int i = 0; i < treeMap.gridSquares * treeMap.gridSquares; i++)
                {
                    treeMap.trees[i].GetComponent<Animator>().SetBool("AllowTrees", true);
                }
            }
            else
            {
                for (int i = 0; i < treeMap.gridSquares * treeMap.gridSquares; i++)
                {
                    treeMap.trees[i].GetComponent<Animator>().SetBool("AllowTrees", false);
                }
            }
        }

        if (noiseMap.height < highestTerrainHeight)
        {
            for (int i = 0; i < treeMap.gridSquares * treeMap.gridSquares; i++)
            {
                treeMap.trees[i].GetComponent<Animator>().SetTrigger("Shrunk");
            }
        }

        Debug.Log(timeBetweenLoop);
    }

    IEnumerator ChangeTerrain ()
    {
        doneChanging = false;
        isChangingDown = true;

        yield return new WaitForSeconds(desiredDuration);

        elapsedTime = 0;

        isChangingDown = false;

        noiseMap.xOffset = Random.Range(-100000, 100000);
        noiseMap.zOffset = Random.Range(-100000, 100000);

        isChangingUp = true;

        yield return new WaitForSeconds(desiredDuration);

        isChangingUp = false;
        treeMap.OffsetTrees();
        if (allowTrees)
        {
            treeMap.AdjustTreeHeight();
        }

        doneChanging = true;
        timeBetweenLoop = 0;

        yield return new WaitForSeconds(refreshRate);

        if (loop && timeBetweenLoop >= refreshRate)
        {
            StartCoroutine(ChangeTerrain());
            timeBetweenLoop = 0;
        }

        yield return null;
    }

    //Button Inputs
    public void Generate ()
    {
        if (doneChanging)
        {
            timeBetweenLoop = 0;
            StartCoroutine(ChangeTerrain());
        }
    }
    public void TreeBool ()
    {
        allowTrees = !allowTrees;
        if (doneChanging && allowTrees)
        {
            treeMap.AdjustTreeHeight();
        }
    }
    public void FalloffBool ()
    {
        noiseMap.falloffBool = !noiseMap.falloffBool;
        noiseMap.EditTerrain(terrain.terrainData);
        colourMap.PaintTerrainColour();
        if (allowTrees && noiseMap.height == highestTerrainHeight)
        {
            treeMap.AdjustTreeHeight();
        }
    }
    public void LoopBool ()
    {
        loop = !loop;
        if (loop && doneChanging)
        {
            timeBetweenLoop = 0;
            StartCoroutine(ChangeTerrain());
        }
    }
    public void QuitGame ()
    {
        Application.Quit();
    }
}
