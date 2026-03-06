using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreePlacement : MonoBehaviour
{
    public GameObject treePrefab;
    public List<GameObject> trees;
    private List<Vector3> startPos;

    private GenerateNoiseMap noiseMap;
    private CreateTerrain createTerrain;
    public float noiseScale;
    public float noiseCutOff;

    private int gridStep;
    public int gridSquares;

    private void Awake()
    {
        noiseMap = GetComponent<GenerateNoiseMap>();
        createTerrain = GetComponent<CreateTerrain>();
        gridStep = noiseMap.width / gridSquares;
    }

    public void GenerateTrees ()
    {
        startPos = new List<Vector3>();
        for (int x = 0; x < gridSquares; x++)
        {
            for (int z = 0; z < gridSquares; z++)
            {
                int xSample = x * gridStep;
                int zSample = z * gridStep;

                Vector3 pos = new Vector3(xSample, 0, zSample);

                var treeInst = Instantiate(treePrefab, pos, Quaternion.Euler(-90, 0, Random.Range(0, 360)));
                trees.Add(treeInst);
                startPos.Add(pos);
            }
        }

    }

    public void OffsetTrees ()
    {
        for (int i = 0; i < gridSquares * gridSquares; i++)
        {
            Vector3 offset = new Vector3(Random.Range(0, gridStep), 0, Random.Range(0, gridStep));
            trees[i].transform.position = startPos[i] + offset;
        }
    }

    public void AdjustTreeHeight ()
    {
        float xOffset = Random.Range(-100000, 100000);
        float zOffset = Random.Range(-100000, 100000);
        for (int i = 0; i < gridSquares * gridSquares; i++)
        { 
            int height = (int)noiseMap.terrain.terrainData.GetHeight(Mathf.RoundToInt(trees[i].transform.position.x), Mathf.RoundToInt(trees[i].transform.position.z));

            trees[i].transform.position = new Vector3(trees[i].transform.position.x, height, trees[i].transform.position.z);
            if (height < 3 || height > 8 || NoiseOffset((int)trees[i].transform.position.x, (int)trees[i].transform.position.z, xOffset, zOffset) < noiseCutOff)
            {
                trees[i].GetComponent<Animator>().SetTrigger("Shrunk");
            }
            else
            {
                trees[i].GetComponent<Animator>().SetTrigger("Grow");
            }
        }
    }

    public void AdjustScale()
    {
        for (int i = 0; i < gridSquares * gridSquares; i++)
        {
            if (trees[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("treeIdle") || trees[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("treeGrow"))
            {
                trees[i].GetComponent<Animator>().SetTrigger("Shrink");
            }
        }
    }


    private float NoiseOffset (int x, int z, float xOffset, float zOffset)
    {
        float xSample = (float)x / noiseMap.width * noiseScale + xOffset;
        float zSample = (float)z / noiseMap.width * noiseScale + zOffset;

        float noise = Mathf.PerlinNoise(xSample, zSample);
        return noise;
    }
}
