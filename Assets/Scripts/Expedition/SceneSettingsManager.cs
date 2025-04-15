using UnityEngine;
public static class SceneSettingsManager
{

    public static void SetTerrainDetails(int DetailDistance, int DetailDensity, int TreeDistance)
    {
        Terrain[] terrains = GameObject.FindObjectsByType<Terrain>(sortMode: FindObjectsSortMode.None);
        foreach (Terrain terrain in terrains)
        {
            terrain.detailObjectDistance = DetailDistance;
            terrain.detailObjectDensity = DetailDensity / 1000f;
            terrain.treeDistance = TreeDistance;
        }
    }
}