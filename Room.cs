using UnityEngine;

public class Room : MonoBehaviour
{

    Vector3 pos;
    Vector3 size;
    Vector3 centerPos;
    public Room nearestRoom;
    public float distanceNearestRoom;

    public void CreateRoom(int minSize, int maxSize, int mapSizeX, int mapSizeY)
    {
        size = new Vector3((int) Random.Range(minSize, maxSize), 1, (int) Random.Range(minSize, maxSize));
        pos = new Vector3((int) Random.Range(0, mapSizeX - size.x), 0.5f, (int) Random.Range(0, mapSizeY - size.z));
        centerPos = new Vector3(pos.x + size.x / 2, pos.y, pos.z + size.z / 2);
        transform.position = centerPos;
    }

    public Vector3 GetSize()
    {
        return size;
    }

    public Vector3 GetPos()
    {
        return pos;
    }

    public Vector3 GetCenter()
    {
        return centerPos;
    }
}
